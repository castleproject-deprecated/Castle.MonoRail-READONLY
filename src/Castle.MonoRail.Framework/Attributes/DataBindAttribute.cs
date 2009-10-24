// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework
{
	using System;
	using System.Reflection;

	using Castle.Components.Binder;

	/// <summary>
	/// The DataBind Attribute is used to indicate that an Action methods parameter 
	/// is to be intercepted and handled by the <see cref="Castle.Components.Binder.DataBinder"/>.
	/// </summary>
	/// <remarks>
	/// Allowed usage is one per method parameter, and is not inherited.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple=false, Inherited=false)]
	public class DataBindAttribute : Attribute, IParameterBinder
	{
		private ParamStore from = ParamStore.Params;
		private string exclude = string.Empty;
		private string allow = string.Empty;
		private string prefix;
		private bool validate;

		/// <summary>
		/// Creates a <see cref="DataBindAttribute"/>
		/// with an associated prefix. The prefix must be present 
		/// in the form data and is used to avoid name clashes.
		/// </summary>
		/// <param name="prefix"></param>
		public DataBindAttribute(string prefix)
		{
			this.prefix = prefix;
		}

		/// <summary>
		/// Gets or sets the property names to exclude.
		/// </summary>
		/// <remarks>The property name should include the <i>prefix</i>.</remarks>
		/// <value>A comma separated list 
		/// of property names to exclude from databinding.</value>
		public string Exclude
		{
			get { return exclude; }
			set { exclude = value; }
		}

		/// <summary>
		/// Gets or sets the property names to allow.
		/// </summary>
		/// <remarks>The property name should include the <i>prefix</i>.</remarks>
		/// <value>A comma separated list 
		/// of property names to allow from databinding.</value>
		public string Allow
		{
			get { return allow; }
			set { allow = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether 
		/// the target should be validate during binding.
		/// </summary>
		/// <value><c>true</c> if should be validated; otherwise, <c>false</c>.</value>
		public bool Validate
		{
			get { return validate; }
			set { validate = value; }
		}

		/// <summary>
		/// Gets or sets <see cref="ParamStore"/> used to 
		/// indicate where to get the values from
		/// </summary>
		/// <value>The <see cref="ParamStore"/> type.  
		/// Typically <see cref="ParamStore.Params"/>.</value>
		public ParamStore From
		{
			get { return from; }
			set { from = value; }
		}

		/// <summary>
		/// Gets the databinding prefix.
		/// </summary>
		/// <remarks>
		/// The prefix is a name followed by a 
		/// dot that prefixes the entries names 
		/// on the source http request.
		/// </remarks>
		/// <value>The databinding prefix.</value>
		public string Prefix
		{
			get { return prefix; }
		}

		/// <summary>
		/// Implementation of <see cref="IParameterBinder.CalculateParamPoints"/>
		/// and it is used to give the method a weight when overloads are available.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="controller">The controller instance</param>
		/// <param name="controllerContext">The controller context.</param>
		/// <param name="parameterInfo">The parameter info</param>
		/// <returns>
		/// Positive value if the parameter can be bound
		/// </returns>
		public int CalculateParamPoints(IEngineContext context, IController controller, IControllerContext controllerContext, ParameterInfo parameterInfo)
		{
			CompositeNode node = context.Request.ObtainParamsNode(From);

			IDataBinder binder = CreateBinder();

			return binder.CanBindObject(parameterInfo.ParameterType, prefix, node) ? 10 : 0;
		}

		/// <summary>
		/// Implementation of <see cref="IParameterBinder.Bind"/>
		/// and it is used to read the data available and construct the
		/// parameter type accordingly.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="controller">The controller instance</param>
		/// <param name="controllerContext">The controller context.</param>
		/// <param name="parameterInfo">The parameter info</param>
		/// <returns>The bound instance</returns>
		public virtual object Bind(IEngineContext context, IController controller, IControllerContext controllerContext, ParameterInfo parameterInfo)
		{
			IDataBinder binder = CreateBinder();
			IValidatorAccessor validatorAccessor = controller as IValidatorAccessor;

			ConfigureValidator(validatorAccessor, binder);

			CompositeNode node = context.Request.ObtainParamsNode(From);

			object instance = binder.BindObject(parameterInfo.ParameterType, prefix, exclude, allow, node);

			BindInstanceErrors(validatorAccessor, binder, instance);
			PopulateValidatorErrorSummary(validatorAccessor, binder, instance);

			return instance;
		}

		/// <summary>
		/// Creates the binder.
		/// </summary>
		/// <returns></returns>
		protected virtual IDataBinder CreateBinder()
		{
			return new DataBinder();
		}

		/// <summary>
		/// Configures the validator.
		/// </summary>
		/// <param name="validatorExposer">The validator exposer.</param>
		/// <param name="binder">The binder.</param>
		protected virtual void ConfigureValidator(IValidatorAccessor validatorExposer, IDataBinder binder)
		{
			if (validate && validatorExposer == null)
			{
				throw new MonoRailException("DataBind wants to validate, but controller does not seem to implement IValidatorAccessor interface");
			}

			if (validate)
			{
				binder.Validator = validatorExposer.Validator;
			}
			else
			{
				binder.Validator = null;
			}
		}

		/// <summary>
		/// Populates the validator error summary.
		/// </summary>
		/// <param name="validatorExposer">The validator exposer.</param>
		/// <param name="binder">The binder.</param>
		/// <param name="instance">The instance.</param>
		protected virtual void PopulateValidatorErrorSummary(IValidatorAccessor validatorExposer, IDataBinder binder, object instance)
		{
			if (validate)
			{
				validatorExposer.PopulateValidatorErrorSummary(instance, binder.GetValidationSummary(instance));
			}
		}

		/// <summary>
		/// Binds the instance errors.
		/// </summary>
		/// <param name="validatorExposer">The validator exposer.</param>
		/// <param name="binder">The binder.</param>
		/// <param name="instance">The instance.</param>
		protected virtual void BindInstanceErrors(IValidatorAccessor validatorExposer, IDataBinder binder, object instance)
		{
			if (instance != null)
			{
				validatorExposer.BoundInstanceErrors[instance] = binder.ErrorList;
			}
		}
	}
}
