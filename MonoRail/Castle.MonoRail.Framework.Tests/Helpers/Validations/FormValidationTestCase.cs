// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.MonoRail.Framework.Tests.Helpers.Validations
{
	using System.Globalization;
	using System.Threading;
	using Castle.Components.Validator;
	using Castle.MonoRail.Framework.Helpers;
	using Castle.MonoRail.Framework.Tests.Controllers;
	using Castle.MonoRail.TestSupport;
	using NUnit.Framework;

	[TestFixture]
	public class FormValidationTestCase : BaseControllerTest
	{
		private FormHelper helper;
		private ModelWithValidation model;

		[SetUp]
		public void Init()
		{
			CultureInfo en = CultureInfo.CreateSpecificCulture("en");

			Thread.CurrentThread.CurrentCulture	= en;
			Thread.CurrentThread.CurrentUICulture = en;

			helper = new FormHelper();
			model = new ModelWithValidation();

			HomeController controller = new HomeController();
			PrepareController(controller, "", "Home", "Index");

			controller.PropertyBag.Add("model", model);

			helper.SetController(controller);
		}

		[Test]
		public void ValidationIsGeneratedForModel()
		{
			helper.FormTag(DictHelper.Create("noaction=true"));

			Assert.AreEqual("<input type=\"text\" id=\"model_nonemptyfield\" " + 
				"name=\"model.nonemptyfield\" value=\"\" class=\"validator-id-prefix-model_ required\" " + 
				"title=\"This is a required field\" />", helper.TextField("model.nonemptyfield"));

			Assert.AreEqual("<input type=\"text\" id=\"model_emailfield\" " +
				"name=\"model.emailfield\" value=\"\" class=\"validate-email\" " +
				"title=\"Email doesnt look right\" />", helper.TextField("model.emailfield"));

			// Attribute order cannot be guaranted, so this test may fail ocasionally
			// Assert.AreEqual("<input type=\"text\" id=\"model_nonemptyemailfield\" " +
			//	"name=\"model.nonemptyemailfield\" value=\"\" class=\"validate-email validator-id-prefix-model_ required\" " +
			//	"title=\"Please enter a valid email address. For example fred@domain.com, This is a required field\" />", helper.TextField("model.nonemptyemailfield"));

			helper.EndFormTag();
		}

		[Test]
		public void UsingScopes()
		{
			helper.FormTag(DictHelper.Create("noaction=true"));
			helper.Push("model");

			Assert.AreEqual("<input type=\"text\" id=\"model_nonemptyfield\" " +
				"name=\"model.nonemptyfield\" value=\"\" class=\"validator-id-prefix-model_ required\" " +
				"title=\"This is a required field\" />", helper.TextField("nonemptyfield"));

			Assert.AreEqual("<input type=\"text\" id=\"model_emailfield\" " +
				"name=\"model.emailfield\" value=\"\" class=\"validate-email\" " +
				"title=\"Email doesnt look right\" />", helper.TextField("emailfield"));

			// Attribute order cannot be guaranted, so this test may fail ocasionally
			// Assert.AreEqual("<input type=\"text\" id=\"model_nonemptyemailfield\" " +
			//	"name=\"model.nonemptyemailfield\" value=\"\" class=\"validate-email validator-id-prefix-model_ required\" " +
			//	"title=\"Please enter a valid email address. For example fred@domain.com, This is a required field\" />", helper.TextField("nonemptyemailfield"));

			helper.Pop();
			helper.EndFormTag();
		}

		[Test]
		public void ValidationForSelects()
		{
			helper.FormTag(DictHelper.Create("noaction=true"));

			Assert.AreEqual("<select id=\"model_city\" " +
				"name=\"model.city\" class=\"validator-id-prefix-model_ validate-selection\" " +
				"title=\"This is a required field\" >\r\n" + 
				"<option value=\"0\">---</option>\r\n" +
				"<option value=\"Sao Paulo\">Sao Paulo</option>\r\n" +
				"<option value=\"Sao Carlos\">Sao Carlos</option>\r\n" +
				"</select>", 
				helper.Select("model.city", 
					new string[] { "Sao Paulo", "Sao Carlos" }, DictHelper.Create("firstoption=---")));

			helper.EndFormTag();
		}

		[Test]
		public void ValidationAreInherited()
		{
			helper.FormTag(DictHelper.Create("noaction=true"));

			Assert.AreEqual("<select id=\"model_city_id\" " +
				"name=\"model.city.id\" class=\"validator-id-prefix-model_ validate-selection\" " +
				"title=\"This is a required field\" >\r\n" +
				"<option value=\"0\">---</option>\r\n" +
				"<option value=\"1\">1</option>\r\n" +
				"<option value=\"2\">2</option>\r\n" +
				"</select>",
				helper.Select("model.city.id",
					new string[] { "1", "2" }, DictHelper.Create("firstoption=---")));

			helper.EndFormTag();
		}
	}

	public class ModelWithValidation
	{
		private string nonEmptyField;
		private string emailField;
		private string nonEmptyEmailField;
		private string city;
		private Country country;

		[ValidateNonEmpty]
		public Country Country
		{
			get { return country; }
			set { country = value; }
		}

		[ValidateNonEmpty]
		public string City
		{
			get { return city; }
			set { city = value; }
		}

		[ValidateNonEmpty]
		public string NonEmptyField
		{
			get { return nonEmptyField; }
			set { nonEmptyField = value; }
		}

		[ValidateEmail("Email doesnt look right")]
		public string EmailField
		{
			get { return emailField; }
			set { emailField = value; }
		}

		[ValidateNonEmpty, ValidateEmail]
		public string NonEmptyEmailField
		{
			get { return nonEmptyEmailField; }
			set { nonEmptyEmailField = value; }
		}
	}

	public class Country
	{
		private int id;

		public int Id
		{
			get { return id; }
			set { id = value; }
		}
	}
}
