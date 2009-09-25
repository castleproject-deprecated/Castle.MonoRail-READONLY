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

namespace Castle.MonoRail.Framework.Helpers
{
	using System;
	using System.IO;
	using System.Text;
	using System.Collections;
	using System.Reflection;
	using System.Web.UI;
	using Castle.MonoRail.Framework.Internal;

	/// <summary>
	/// Provides usefull common methods to generate HTML tags.
	/// </summary>
	/// <remarks>This helper provides the means to generate commonly used HTML tags. 
	/// All of it's methods return <see cref="String"/> that holds resulting HTML.
	/// </remarks>
	public class HtmlHelper : AbstractHelper
	{
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="HtmlHelper"/> class.
		/// </summary>
		public HtmlHelper() { }
		/// <summary>
		/// Initializes a new instance of the <see cref="HtmlHelper"/> class.
		/// setting the Controller, Context and ControllerContext.
		/// </summary>
		/// <param name="engineContext">The engine context.</param>
		public HtmlHelper(IEngineContext engineContext) : base(engineContext) { }
		#endregion

		#region Fieldset

		/// <summary>
		/// Creates a <b>fieldset</b> tag with a legend.
		/// <code>
		/// &lt;fieldset&gt;&lt;legend&gt;legendArg&lt;/legend&gt;
		/// </code>
		/// <seealso cref="HtmlHelper.EndFieldSet"/>
		/// </summary>
		/// <param name="legend">Legend to use within the fieldset.</param>
		/// <returns>HTML string opening a fieldset tag, followed by a legend tag.</returns>
		/// <remarks>Calling <c>FieldSet( "legendArg" )</c> results in:
		/// <code>&lt;fieldset&gt;&lt;legend&gt;legendArg&lt;/legend&gt;</code>
		/// </remarks>
		/// <example>This example shows how to use <see cref="FieldSet"/> together with <see cref="EndFieldSet"/>:
		/// <code>
		/// $HtmlHelper.FieldSet( "legendArg" )
		/// ...
		/// $HtmlHelper.EndFieldSet()
		/// </code>
		/// </example>
		public virtual String FieldSet(String legend)
		{
			return String.Format("<fieldset><legend>{0}</legend>", legend);
		}

		/// <summary>
		/// Creates a closing <b>fieldset</b> tag.
		/// <code>
		/// &lt;/fieldset&gt;
		/// </code>
		/// <seealso cref="HtmlHelper.FieldSet"/>
		/// </summary>
		/// <returns>HTML string closing the fieldset.</returns>
		/// <remarks>This method should be invoked after <see cref="FieldSet"/> to close the fieldset.
		/// Calling <c>EndFieldSet()</c> results in:
		/// <code>&lt;/fieldset&gt;</code>
		/// </remarks>
		/// <example>This example shows how to use <see cref="FieldSet"/> together with <see cref="EndFieldSet"/>:
		/// <code>
		/// $HtmlHelper.FieldSet( "legendArg" )
		/// ...
		/// $HtmlHelper.EndFieldSet()
		/// </code>
		/// </example>
		public virtual String EndFieldSet()
		{
			return "</fieldset>";
		}

		#endregion

		#region Form and FormTo

		///<overloads>This method has three overloads.</overloads>
		/// <summary>
		/// Creates a <b>form</b> tag with "<b>post</b>" method and specified <paramref name="action"/>.
		/// <code>
		/// &lt;form method=&quot;post&quot; action=&quot;actionArg&quot;&gt;
		/// </code>
		/// <seealso cref="HtmlHelper.EndForm"/>
		/// </summary>
		/// <param name="action">Target action for the form.</param>
		/// <returns>HTML string with form opening tag.</returns>
		/// <remarks>Calling <c>Form( "actionArg" )</c> results in:
		/// <code>&lt;form method=&quot;post&quot; action=&quot;actionArg&quot;&gt;</code>
		/// </remarks>
		/// <example>This example shows how to use <see cref="Form(String)"/> together with <see cref="EndForm"/>:
		/// <code>
		/// $HtmlHelper.Form( "actionArg" )
		/// ...
		/// $HtmlHelper.EndForm()
		/// </code>
		/// </example>
		public virtual String Form(String action)
		{
			StringBuilder sb = new StringBuilder();

			StringWriter sbWriter = new StringWriter(sb);
			HtmlTextWriter writer = new HtmlTextWriter(sbWriter);

			writer.WriteBeginTag("form");
			writer.WriteAttribute("method", "post");
			writer.WriteAttribute("action", action);
			writer.Write(HtmlTextWriter.TagRightChar);
			writer.WriteLine();

			return sbWriter.ToString();
		}

		/// <summary>
		/// Creates a <b>form</b> tag with the specified <paramref name="method"/>, <paramref name="action"/> and
		/// <paramref name="id"/> attributes.
		/// <code>
		/// &lt;form method=&quot;methodArg&quot; action=&quot;actionArg&quot; id=&quot;idArg&quot;&gt;
		/// </code>
		/// <seealso cref="HtmlHelper.EndForm"/>
		/// </summary>
		/// <param name="action">Target action for the form.</param>
		/// <param name="id">Form HTML ID.</param>
		/// <param name="method">Form method (get, post, etc).</param>
		/// <returns>HTML string with form opening tag.</returns>
		/// <remarks>Calling <c>Form( "actionArg", "idArg", "methodArg" )</c> results in:
		/// <code>&lt;form method=&quot;methodArg&quot; action=&quot;actionArg&quot; id=&quot;idArg&quot;&gt;</code>
		/// </remarks>
		/// <example>This example shows how to use <b>Form</b> together with <see cref="EndForm"/>:
		/// <code>
		/// $HtmlHelper.Form( "actionArg", "idArg", "methodArg" )
		/// ...
		/// $HtmlHelper.EndForm()
		/// </code>
		/// </example>
		public virtual String Form(String action, String id, String method)
		{
			return Form(action, id, method, null);
		}

		/// <summary>
		/// Creates a <b>form</b> tag with the specified <paramref name="method"/> and <paramref name="action"/> attributes,
		/// <paramref name="id"/> and <paramref name="onSubmit"/> event handler. 
		/// <code>
		/// &lt;form method=&quot;methodArg&quot; action=&quot;actionArg&quot; id=&quot;idArg&quot; onsubmit=&quot;onSubmitArg&quot;&gt;
		/// </code>
		/// <seealso cref="HtmlHelper.EndForm"/>
		/// </summary>
		/// <param name="action">Target action for the form.</param>
		/// <param name="id">Form HTML ID.</param>
		/// <param name="method">Form method (get, post, etc).</param>
		/// <param name="onSubmit">JavaScript inline code to be invoked upon form submission.</param>
		/// <returns>HTML string with form opening tag.</returns>
		/// <remarks>Calling <c>Form( "actionArg", "idArg", "methodArg", "onSubmitArg" )</c> results in:
		/// <code>&lt;form method=&quot;methodArg&quot; action=&quot;actionArg&quot; id=&quot;idArg&quot; onsubmit=&quot;onSubmitArg&quot;&gt;</code>
		/// </remarks>
		/// <example>This example shows how to use <b>Form</b> together with <see cref="EndForm"/>:
		/// <code>
		/// $HtmlHelper.Form( "actionArg", "idArg", "methodArg", "submitHandler()" )
		/// ...
		/// $HtmlHelper.EndForm()
		/// </code>
		/// </example>
		public virtual String Form(String action, String id, String method, String onSubmit)
		{
			StringBuilder sb = new StringBuilder();

			StringWriter sbWriter = new StringWriter(sb);
			HtmlTextWriter writer = new HtmlTextWriter(sbWriter);

			writer.WriteBeginTag("form");
			writer.WriteAttribute("method", method);
			writer.WriteAttribute("action", action);
			writer.WriteAttribute("id", id);
			if (onSubmit != null)
				writer.WriteAttribute("onsubmit", onSubmit);
			writer.Write(HtmlTextWriter.TagRightChar);
			writer.WriteLine();

			return sbWriter.ToString();
		}

		/// <summary>
		/// Creates a <b>form</b> tag with the specified <paramref name="action"/> attribute.
		/// <code>
		/// &lt;form action=&quot;actionArg&quot;&gt;
		/// </code>
		/// <seealso cref="HtmlHelper.EndForm"/>
		/// </summary>
		/// <param name="action">Target action for the form.</param>
		/// <param name="attributes">Html Attributes for the form tag</param>
		/// <returns>HTML string with form opening tag.</returns>
		public virtual String Form(String action, IDictionary attributes)
		{
			return String.Format("<form action=\"{0}\" {1}>", action, GetAttributes(attributes));
		}

		/// <summary>
		/// Creates a <b>form</b> tag targeting a URL in the style of the <see cref="LinkTo(String, String)"/> methods.
		/// </summary>
		/// <param name="action">An action on the current controller.</param>
		/// <returns>HTML string with form opening tag.</returns>
		public virtual String FormTo(String action)
		{
			return FormTo(ControllerContext.Name, action, null);
		}

		/// <summary>
		/// Creates a <b>form</b> tag targeting a URL in the style of the <see cref="LinkTo(String, String)"/> methods.
		/// </summary>
		/// <param name="controller">A controller name.</param>
		/// <param name="action">An action on <paramref name="controller"/>.</param>
		/// <returns>HTML string with form opening tag.</returns>
		public virtual String FormTo(String controller, String action)
		{
			return FormTo(controller, action, null);
		}

		/// <summary>
		/// Creates a <b>form</b> tag targeting a URL in the style of the <see cref="LinkTo(String, String)"/> methods.
		/// </summary>
		/// <param name="controller">A controller name.</param>
		/// <param name="action">An action on <paramref name="controller"/>.</param>
		/// <param name="id">Object to use for the action ID argument.</param>
		/// <returns>HTML string with form opening tag.</returns>
		public virtual String FormTo(String controller, String action, object id)
		{
			return FormToAttributed(controller, action, id, null);
		}

		/// <summary>
		/// Creates a <b>form</b> tag targeting a URL in the style of the <see cref="LinkToAttributed(String, String, String, IDictionary)"/> methods.
		/// </summary>
		/// <param name="controller">A controller name.</param>
		/// <param name="action">An action on <paramref name="controller"/>.</param>
		/// <param name="attributes">Additional attributes for the <b>form</b> tag.</param>
		/// <returns>HTML string with form opening tag.</returns>
		public virtual String FormToAttributed(String controller, String action, IDictionary attributes)
		{
			return FormToAttributed(controller, action, null, attributes);
		}

		/// <summary>
		/// Creates a <b>form</b> tag targeting a URL in the style of the <see cref="LinkToAttributed(String, String, String, IDictionary)"/> methods.
		/// </summary>
		/// <param name="controller">A controller name.</param>
		/// <param name="action">An action on <paramref name="controller"/>.</param>
		/// <param name="id">Object to use for the action ID argument.</param>
		/// <param name="attributes">Additional attributes for the <b>form</b> tag.</param>
		/// <returns>HTML string with form opening tag.</returns>
		public virtual String FormToAttributed(String controller, String action, object id, IDictionary attributes)
		{
			return FormToAttributed(controller, action, id, null, attributes);
		}

		/// <summary>
		/// Creates a <b>form</b> tag targeting a URL in the style of the <see cref="LinkToAttributed(String, String, String, IDictionary)"/> methods.
		/// </summary>
		/// <param name="controller">A controller name.</param>
		/// <param name="action">An action on <paramref name="controller"/>.</param>
		/// <param name="id">Object to use for the action ID argument.</param>
		/// <param name="method">Form method (get, post, etc).</param>
		/// <param name="attributes">Additional attributes for the <b>form</b> tag.</param>
		/// <returns>HTML string with form opening tag.</returns>
		public virtual String FormToAttributed(String controller, String action, object id, string method, IDictionary attributes)
		{
			string formAction = UrlHelper.For(DictHelper.Create("controller=" + controller, "action=" + action));

			if (id != null)
			{
				formAction += "?id=" + id;
			}

			if (method == null)
			{
				method = "post";
			}

			if (attributes == null)
			{
				attributes = DictHelper.Create("method=" + method);
			}
			else
			{
				attributes["method"] = method;
			}

			return Form(formAction, attributes);
		}

		/// <summary>
		/// Creates a closing <b>form</b> tag.
		/// <code>
		/// &lt;/form&gt;
		/// </code>
		/// <seealso cref="Form(String)"/>
		/// </summary>
		/// <returns>HTML string with form closing tag.</returns>
		/// <remarks>
		/// Calling <c>EndForm()</c> results in:
		/// <code>&lt;/form&gt;</code>
		/// </remarks>
		/// <example>This example shows how to use <see cref="Form(String,String,String,String)"/> together with <see cref="EndForm"/>:
		/// <code>
		/// $HtmlHelper.Form( "actionArg", "idArg", "methodArg", "submitHandler()" )
		/// ...
		/// $HtmlHelper.EndForm()
		/// </code>
		/// </example>
		public virtual String EndForm()
		{
			return "</form>";
		}

		#endregion

		#region Link and LinkTo and LinkToWithPost

		///<overloads>This method has two overloads.</overloads>
		/// <summary>
		/// Creates an anchor (link) to the <paramref name="target"/> 
		/// <code>
		/// &lt;a href=&quot;/sometarget.html&quot;&gt;linkText&lt;/a&gt;
		/// </code>
		/// </summary>
		/// <param name="target">link's target.</param>
		/// <param name="linkText">Text of the link.</param>
		/// <returns>HTML string with anchor to the specified <paramref name="target"/>.</returns>
		/// <remarks>Calling <c>Link( "something.html", "to something" )</c> results in:
		/// <code>&lt;a href=&quot;something.html&quot;&gt;something&lt;/a&gt;</code>
		/// </remarks>
		/// <example>This example shows how to use <b>Link</b>:
		/// <code>
		/// $HtmlHelper.Link( "mypage.html", "This is a link to my page" )
		/// </code>
		/// </example>
		public virtual String Link(String target, String linkText)
		{
			return Link(target, linkText, null);
		}

		/// <summary>
		/// Creates an anchor (link) to the <paramref name="target"/> 
		/// <code>
		/// &lt;a href=&quot;/sometarget.html&quot;&gt;linkText&lt;/a&gt;
		/// </code>
		/// </summary>
		/// <param name="target">link's target.</param>
		/// <param name="linkText">Text of the link.</param>
		/// <param name="attributes">Additional attributes for the <b>a</b> tag.</param>
		/// <returns>HTML string with anchor to the specified <paramref name="target"/>.</returns>
		/// <remarks>Calling <c>Link( "something.html", "to something", $DictHelper.CreateDict("class=mylinkclass") )</c> results in:
		/// <code>&lt;a href=&quot;something.html&quot; class=&quot;mylinkclass&quot;&gt;something&lt;/a&gt;</code>
		/// </remarks>
		/// <example>This example shows how to use <b>Link</b>:
		/// <code>
		/// $HtmlHelper.Link( "mypage.html", "This is a link to my page", $DictHelper.CreateDict("class=mylinkclass") )
		/// </code>
		/// </example>
		public virtual String Link(String target, String linkText, IDictionary attributes)
		{
			return String.Format("<a href=\"{0}\" {1}>{2}</a>",
			                     target, GetAttributes(attributes), linkText);
		}

		///<overloads>This method has three overloads.</overloads>
		/// <summary>
		/// Creates an anchor (link) to the <paramref name="action"/> on the current controller.
		/// <code>
		/// &lt;a href=&quot;/website/currentController/actionArg.rails&quot;&gt;nameArg&lt;/a&gt;
		/// </code>
		/// </summary>
		/// <param name="name">Name for the link.</param>
		/// <param name="action">Action to link to.</param>
		/// <returns>HTML string with anchor to the specified <paramref name="action"/>.</returns>
		/// <remarks>Calling <c>LinkTo( "nameArg", "actionArg" )</c> results in:
		/// <code>&lt;a href=&quot;/websiter/currentController/actionArg.rails&quot;&gt;nameArg&lt;/a&gt;</code>
		/// </remarks>
		/// <example>This example shows how to use <b>LinkTo</b>:
		/// <code>
		/// $HtmlHelper.LinkTo( "linkName", "requiredAction" )
		/// </code>
		/// </example>
		public virtual String LinkTo(String name, String action)
		{
			string url = UrlHelper.For(DictHelper.Create("action=" + action));

			return String.Format("<a href=\"{0}\">{1}</a>", url, name);
		}

		///<overloads>This method has three overloads.</overloads>
		/// <summary>
		/// Creates an anchor (link) to the <paramref name="action"/> on the current controller.
		/// <code>
		/// &lt;a href=&quot;/website/currentController/actionArg.rails&quot;&gt;nameArg&lt;/a&gt;
		/// </code>
		/// </summary>
		/// <param name="name">Name for the link.</param>
		/// <param name="options">link options</param>
		/// <returns>HTML string with anchor to the specified <paramref name="options"/>.</returns>
		/// <remarks>Calling <c>LinkTo( "nameArg", DictHelper.CreateDict("controller=home","action=index") )</c> results in:
		/// <code>&lt;a href=&quot;/websiter/home/index.rails&quot;&gt;nameArg&lt;/a&gt;</code>
		/// </remarks>
		/// <example>This example shows how to use <b>LinkTo</b>:
		/// <code>
		/// $HtmlHelper.LinkTo( "linkName", DictHelper.CreateDict("controller=home","action=index") )
		/// </code>
		/// </example>
		public virtual String LinkTo(String name, IDictionary options)
		{
			string url = UrlHelper.For(options);
			// remove the common action attribute 
			CommonUtils.ObtainEntryAndRemove(options, "area");
			CommonUtils.ObtainEntryAndRemove(options, "controller");
			CommonUtils.ObtainEntryAndRemove(options, "action");

			// and consider the other options to be link attributes
			return String.Format("<a href=\"{0}\" {2}>{1}</a>", url, name, GetAttributes(options));
		}

		/// <summary>
		/// Creates an anchor (link) to the <paramref name="action"/> on the specified <paramref name="controller"/>.
		/// <code>
		/// &lt;a href=&quot;/website/controllerArg/actionArg.rails&quot;&gt;nameArg&lt;/a&gt;
		/// </code>
		/// </summary>
		/// <param name="name">Name for the link.</param>
		/// <param name="controller">Controller to link to.</param>
		/// <param name="action">Action to link to.</param>
		/// <returns>HTML string with anchor to the specified <paramref name="controller"/>
		/// and <paramref name="action"/>.</returns>
		/// <remarks>Calling <c>LinkTo( "nameArg", options )</c> results in:
		/// <code>&lt;a href=&quot;/website/controllerArg/actionArg.rails&quot;&gt;nameArg&lt;/a&gt;</code>
		/// </remarks>
		/// <example>This example shows how to use <b>LinkTo</b>:
		/// <code>
		/// $HtmlHelper.LinkTo( "linkName", {@action:} )
		/// </code>
		/// </example>
		public virtual String LinkTo(String name, String controller, String action)
		{
			string url = UrlHelper.For(DictHelper.Create("controller=" + controller, "action=" + action));

			return String.Format("<a href=\"{0}\">{1}</a>", url, name);
		}

		/// <summary>
		/// Creates an anchor (link) to the <paramref name="action"/> on the specified <paramref name="controller"/>
		/// passing provided <paramref name="id"/>.
		/// <code>
		/// &lt;a href=&quot;/website/controllerArg/actionArg.rails?id=objectId&quot;&gt;nameArg&lt;/a&gt;
		/// </code>
		/// </summary>
		/// <param name="name">Name for the link.</param>
		/// <param name="controller">Controller to link to.</param>
		/// <param name="action">Action to link to.</param>
		/// <param name="id">Object to use for the action ID argument.</param>
		/// <returns>HTML string with anchor to the specified <paramref name="controller"/>, 
		/// <paramref name="action"/> and <paramref name="id"/>.</returns>
		/// <remarks>Calling <c>LinkTo( "nameArg", "controllerArg", "actionArg", object )</c> results in:
		/// <code>&lt;a href=&quot;/website/controllerArg/actionArg.rails?id=object&quot;&gt;nameArg&lt;/a&gt;</code>
		/// <para>
		/// <see cref="String.Format(String, object)"/> is used to convert <paramref name="id"/> to the actual <see cref="String"/>.</para>
		/// </remarks>
		/// <example>This example shows how to use <b>LinkTo</b>:
		/// <code>
		/// $HtmlHelper.LinkTo( "linkName", "someController", "requiredAction", objectToRefByID )
		/// </code>
		/// </example>
		public virtual String LinkTo(String name, String controller, String action, object id)
		{
			string url = UrlHelper.For(DictHelper.Create("controller=" + controller, "action=" + action));

			return String.Format("<a href=\"{0}?id={1}\">{2}</a>", url, id, name);
		}

		/// <summary>
		/// Creates an anchor (link) to the <paramref name="action"/> on the specified <paramref name="controller"/>
		/// <code>
		/// &lt;a href=&quot;/website/controllerArg/actionArg.rails&quot;&gt;nameArg&lt;/a&gt;
		/// </code>
		/// </summary>
		/// <param name="name">Name for the link.</param>
		/// <param name="controller">Controller to link to.</param>
		/// <param name="action">Action to link to.</param>
		/// <param name="attributes">Additional attributes for the <b>a</b> tag.</param>
		/// <returns>HTML string with anchor to the specified <paramref name="controller"/></returns>
		/// <remarks>Calling <c>LinkToAttributed( "nameArg", "controllerArg", "actionArg", IDictionary )</c> results in:
		/// <code>&lt;a href=&quot;/website/controllerArg/actionArg.rails&quot;&gt;nameArg&lt;/a&gt;</code>
		/// </remarks>
		/// <example>This example shows how to use <b>LinkToAttributed</b>:
		/// <code>
		/// $HtmlHelper.LinkToAttributed( "linkName", "someController", "requiredAction", $DictHelper.CreateDict("class=something") )
		/// </code>
		/// </example>
		public virtual String LinkToAttributed(String name, String controller, String action, IDictionary attributes)
		{
			string url = UrlHelper.For(DictHelper.Create("controller=" + controller, "action=" + action));

			return String.Format("<a href=\"{0}\" {2}>{1}</a>", url, name, GetAttributes(attributes));
		}

		/// <summary>
		/// Creates an anchor (link) to the <paramref name="action"/> on the specified <paramref name="controller"/>
		/// <code>
		/// &lt;a href=&quot;/website/controllerArg/actionArg.rails?id=x&quot;&gt;nameArg&lt;/a&gt;
		/// </code>
		/// </summary>
		/// <param name="name">Name for the link.</param>
		/// <param name="controller">Controller to link to.</param>
		/// <param name="action">Action to link to.</param>
		/// <param name="id">The ID to be passed as a parameter for the action</param>
		/// <param name="attributes">Additional attributes for the <b>a</b> tag.</param>
		/// <returns>HTML string with anchor to the specified <paramref name="controller"/></returns>
		/// <remarks>Calling <c>LinkToAttributed( "nameArg", "controllerArg", "actionArg", IDictionary )</c> results in:
		/// <code>&lt;a href=&quot;/website/controllerArg/actionArg.rails&quot;&gt;nameArg&lt;/a&gt;</code>
		/// </remarks>
		/// <example>This example shows how to use <b>LinkToAttributed</b>:
		/// <code>
		/// $HtmlHelper.LinkToAttributed( "linkName", "someController", "requiredAction", $DictHelper.CreateDict("class=something") )
		/// </code>
		/// </example>
		public virtual String LinkToAttributed(String name, String controller, String action, object id, IDictionary attributes)
		{
			string url = UrlHelper.For(DictHelper.Create("controller=" + controller, "action=" + action));

			return String.Format("<a href=\"{0}?id={1}\" {3}>{2}</a>", url, id, name, GetAttributes(attributes));
		}

		/// <summary>
		/// Creates an anchor (link) to the <paramref name="action"/> on the current controller that posts
		/// using a hidden form element.
		/// </summary>
		/// <param name="name">Name for the link.</param>
		/// <param name="action">Action to link to on the current controller.</param>
		/// <returns>HTML string with anchor that posts to the current controller</returns>
		public virtual String LinkToWithPost(String name, String action)
		{
			return LinkToWithPost(name, action, null);
		}

		/// <summary>
		/// Creates an anchor (link) to the <paramref name="action"/> on the current controller that posts
		/// using a hidden form element.
		/// </summary>
		/// <param name="name">Name for the link.</param>
		/// <param name="action">Action to link to on the current controller.</param>
		/// <param name="id">The ID to be passed as a parameter for the action.</param>
		/// <returns>HTML string with anchor that posts to the current controller</returns>
		public virtual String LinkToWithPost(String name, String action, object id)
		{
			return LinkToWithPost(name, ControllerContext.Name, action, id);
		}

		/// <summary>
		/// Creates an anchor (link) to the <paramref name="action"/> on the current controller that posts
		/// using a hidden form element.
		/// </summary>
		/// <param name="name">Name for the link.</param>
		/// <param name="action">Action to link to on the current controller.</param>
		/// <param name="confirm">Guards the form submission with a javascript confirm popup.</param>
		/// <returns>HTML string with anchor that posts to the current controller</returns>
		public virtual String LinkToWithPost(String name, String action, String confirm)
		{
			return LinkToWithPost(name, ControllerContext.Name, action, confirm);
		}

		/// <summary>
		/// Creates an anchor (link) to the <paramref name="action"/> on the current controller that posts
		/// using a hidden form element.
		/// </summary>
		/// <param name="name">Name for the link.</param>
		/// <param name="action">Action to link to on the current controller.</param>
		/// <param name="id">The ID to be passed as a parameter for the action.</param>
		/// <param name="confirm">Guards the form submission with a javascript confirm popup.</param>
		/// <returns>HTML string with anchor that posts to the current controller</returns>
		public virtual String LinkToWithPost(String name, String action, object id, string confirm)
		{
			return LinkToWithPost(name, ControllerContext.Name, action, id, confirm);
		}

		/// <summary>
		/// Creates an anchor (link) to the <paramref name="action"/> on the specified <paramref name="controller"/> that posts
		/// using a hidden form element.
		/// </summary>
		/// <param name="name">Name for the link.</param>
		/// <param name="controller">Controller to link to.</param>
		/// <param name="action">Action to link to.</param>
		/// <param name="id">The ID to be passed as a parameter for the action.</param>
		/// <returns>HTML string with anchor that posts to the specified <paramref name="controller"/></returns>
		public virtual String LinkToWithPost(String name, String controller, String action, object id)
		{
			return LinkToWithPost(name, controller, action, id, null);
		}

		/// <summary>
		/// Creates an anchor (link) to the <paramref name="action"/> on the specified <paramref name="controller"/> that posts
		/// using a hidden form element.
		/// </summary>
		/// <param name="name">Name for the link.</param>
		/// <param name="controller">Controller to link to.</param>
		/// <param name="action">Action to link to.</param>
		/// <param name="confirm">Guards the form submission with a javascript confirm popup.</param>
		/// <returns>HTML string with anchor that posts to the specified <paramref name="controller"/></returns>
		public virtual String LinkToWithPost(String name, String controller, String action, String confirm)
		{
			return LinkToWithPost(name, controller, action, null, confirm);
		}

		/// <summary>
		/// Creates an anchor (link) to the <paramref name="action"/> on the specified <paramref name="controller"/> that posts
		/// using a hidden form element.
		/// </summary>
		/// <param name="name">Name for the link.</param>
		/// <param name="controller">Controller to link to.</param>
		/// <param name="action">Action to link to.</param>
		/// <param name="id">The ID to be passed as a parameter for the action.</param>
		/// <param name="confirm">Guards the form submission with a javascript confirm popup.</param>
		/// <returns>HTML string with anchor that posts to the specified <paramref name="controller"/></returns>
		public virtual String LinkToWithPost(String name, String controller, String action, object id, String confirm)
		{
			return LinkToWithPostAttributed(name, controller, action, id, confirm, null);
		}

		/// <summary>
		/// Creates an anchor (link) to the <paramref name="action"/> on the specified <paramref name="controller"/> that posts
		/// using a hidden form element.
		/// </summary>
		/// <param name="name">Name for the link.</param>
		/// <param name="controller">Controller to link to.</param>
		/// <param name="action">Action to link to.</param>
		/// <param name="confirm">Guards the form submission with a javascript confirm popup.</param>
		/// <param name="attributes">Additional attributes for the <b>a</b> tag.</param>
		/// <returns>HTML string with anchor that posts to the specified <paramref name="controller"/></returns>
		public virtual String LinkToWithPostAttributed(String name, String controller, String action, String confirm,
		                                       IDictionary attributes)
		{
			return LinkToWithPostAttributed(name, controller, action, null, confirm, attributes);
		}

		/// <summary>
		/// Creates an anchor (link) to the <paramref name="action"/> 
		/// on the specified <paramref name="controller"/> that posts
		/// using a hidden form element.
		/// </summary>
		/// <param name="name">Name for the link.</param>
		/// <param name="controller">Controller to link to.</param>
		/// <param name="action">Action to link to.</param>
		/// <param name="id">The ID to be passed as a parameter for the action.</param>
		/// <param name="confirm">Guards the form submission with a javascript confirm popup.</param>
		/// <param name="attributes">Additional attributes for the <b>a</b> tag.</param>
		/// <returns>HTML string with anchor that posts to the specified <paramref name="controller"/></returns>
		public virtual String LinkToWithPostAttributed(String name, String controller, String action, object id, String confirm,
		                                       IDictionary attributes)
		{
			IDictionary formAttributes = DictHelper.Create("style=display:inline;margin:0;");

			string onclickAttribute = "this.parentNode.submit();return false;";

			if (confirm != null && confirm != "")
			{
				onclickAttribute = "if(confirm('" + confirm + "')){{this.parentNode.submit();}};return false;";
			}

			if (attributes == null)
			{
				attributes = DictHelper.Create("onclick=" + onclickAttribute);
			}
			else
			{
				attributes["onclick"] = onclickAttribute;
			}

			StringBuilder stringBuilder = new StringBuilder();

			if (id != null)
			{
				stringBuilder.Append(FormToAttributed(controller, action, id, formAttributes));
				stringBuilder.Append(LinkToAttributed(name, controller, action, id, attributes));
			}
			else
			{
				stringBuilder.Append(FormToAttributed(controller, action, formAttributes));
				stringBuilder.Append(LinkToAttributed(name, controller, action, attributes));
			}

			stringBuilder.Append(EndForm());

			return stringBuilder.ToString();
		}

		#endregion

		#region MapToVirtual

		/// <summary>
		/// Maps <paramref name="target"/> to website virtual path.
		/// <code>/website/targetArg</code>
		/// </summary>
		/// <param name="target">Target path to map.</param>
		/// <returns>URL string pointing to the <paramref name="target"/> in the context of the website.</returns>
		/// <remarks>Calling <c>MapToVirtual( "targetArg" )</c> results in:
		/// <code>/website/targetArg</code>
		/// </remarks>
		/// <example>This example shows how to use <see cref="MapToVirtual"/>:
		/// <code>
		/// $HtmlHelper.MapToVirtual( "targetFolder/targetFile.html" )
		/// </code>
		/// </example>
		public virtual String MapToVirtual(String target)
		{
			String appPath = Context.ApplicationPath.EndsWith("/") ? Context.ApplicationPath : Context.ApplicationPath + "/";

			String targetPath = target.StartsWith("/") ? target.Substring(1) : target;

			return String.Concat(appPath, targetPath);
		}

		#endregion

		#region LabelFor

		///<overloads>This method has two overloads.</overloads>
		/// <summary>
		/// Creates a label for the element indicated with
		/// <paramref name="forId"/>.
		/// <code>
		/// &lt;label  for=&quot;forIdArg&quot;&gt;labelArg&lt;/label&gt;
		/// </code>
		/// </summary>
		/// <param name="forId">ID of the element for which to create the lable.</param>
		/// <param name="label">Label name.</param>
		/// <returns>HTML string with generated label.</returns>
		/// <remarks>Calling <c>LabelFor( "forIdArg", "labelArg" )</c> results in:
		/// <code>&lt;label  for=&quot;forIdArg&quot;&gt;labelArg&lt;/label&gt;</code>
		/// </remarks>
		/// <example>This example shows how to use <see cref="LabelFor(String,String)"/>:
		/// <code>
		/// $HtmlHelper.LabelFor( "forIdArg", "labelArg" )
		/// </code>
		/// </example>
		public virtual String LabelFor(String forId, String label)
		{
			return LabelFor(forId, label, null);
		}

		/// <summary>
		/// Creates a label for the element indicated with
		/// <paramref name="forId"/>.
		/// <code>
		/// &lt;label key1=&quot;value1&quot; key2=&quot;value2&quot;  for=&quot;forIdArg&quot;&gt;labelArg&lt;/label&gt;
		/// </code>
		/// </summary>
		/// <param name="forId">ID of the element for which to create the label.</param>
		/// <param name="label">Label name.</param>
		/// <param name="attributes">Additional attributes to add to the label.</param>
		/// <returns>HTML string with generated label.</returns>
		/// <remarks>Calling <c>LabelFor( "forIdArg", "labelArg", IDictionary )</c> results in:
		/// <code>&lt;label key5=&quot;value5&quot; key4=&quot;value4&quot; key1=&quot;value1&quot; key3=&quot;value3&quot; key2=&quot;value2&quot;  for=&quot;forIdArg&quot;&gt;labelArg&lt;/label&gt;</code>
		/// </remarks>
		/// <para>
		/// <paramref name="attributes"/> is used to generate additional attributes for the <b>label</b> tag.
		/// <see cref="IDictionary.Keys"/> are used to name attributes.
		/// <see cref="IDictionary.Values"/> are used to assign those attributes values.
		/// </para>
		/// <example>This example shows how to use <see cref="LabelFor(String,String,IDictionary)"/>:
		/// <code>
		/// $HtmlHelper.LabelFor( "forIdArg", "labelArg", IDictionary )
		/// </code>
		/// </example>
		public virtual String LabelFor(String forId, String label, IDictionary attributes)
		{
			StringBuilder sb = new StringBuilder();
			StringWriter sbWriter = new StringWriter(sb);
			HtmlTextWriter writer = new HtmlTextWriter(sbWriter);

			writer.WriteBeginTag("label");
			writer.Write(" ");
			writer.Write(GetAttributes(attributes));
			writer.WriteAttribute("for", forId);
			writer.Write(HtmlTextWriter.TagRightChar);
			writer.Write(label);
			writer.WriteEndTag("label");
			writer.WriteLine();

			return sbWriter.ToString();
		}

		#endregion

		#region DateTime

		///<overloads>This method has two overloads.</overloads>
		/// <summary>
		/// Creates three <b>select</b> tags to input day, month and year.
		/// <code>
		/// &lt;select name=&quot;nameArgday&quot; id=&quot;nameArgday&quot; &gt; ... &lt;/select&gt;
		/// &lt;select name=&quot;nameArgmonth&quot; id=&quot;nameArgmonth&quot; &gt; ... &lt;/select&gt;
		/// &lt;select name=&quot;nameArgyear&quot; id=&quot;nameArgyear&quot; &gt; ... &lt;/select&gt;
		/// </code>
		/// </summary>
		/// <param name="name">Name to use with <b>name</b> and <b>id</b> arguments of the <b>select</b> tag.</param>
		/// <param name="value"><see cref="System.DateTime"/> to use for default selected date.</param>
		/// <returns>A long HTML string with three <b>select</b> tag input date.</returns>
		/// <remarks>Calling <c>DateTime( "nameArg", new DateTime( 2005, 07, 15 ) )</c> results in:
		/// <code>
		/// &lt;select name=&quot;nameArgday&quot; id=&quot;nameArgday&quot; &gt;	&lt;option&gt;1&lt;/option&gt;
		/// 	&lt;option&gt;2&lt;/option&gt;
		/// 	...
		/// 	&lt;option&gt;14&lt;/option&gt;
		///		&lt;option selected&gt;15&lt;/option&gt;
		///		&lt;option&gt;16&lt;/option&gt;
		///		...
		///		&lt;option&gt;30&lt;/option&gt;
		///		&lt;option&gt;31&lt;/option&gt;
		///	 &lt;/select&gt; &lt;select name=&quot;nameArgmonth&quot; id=&quot;nameArgmonth&quot; &gt;	&lt;option&gt;1&lt;/option&gt;
		///		&lt;option&gt;2&lt;/option&gt;
		///		...
		///		&lt;option&gt;6&lt;/option&gt;
		///		&lt;option selected&gt;7&lt;/option&gt;
		///		&lt;option&gt;8&lt;/option&gt;
		///		...
		///		&lt;option&gt;11&lt;/option&gt;
		///		&lt;option&gt;12&lt;/option&gt;
		///	 &lt;/select&gt; &lt;select name=&quot;nameArgyear&quot; id=&quot;nameArgyear&quot; &gt;	&lt;option&gt;1930&lt;/option&gt;
		///		&lt;option&gt;1931&lt;/option&gt;
		///		...
		///		&lt;option&gt;2004&lt;/option&gt;
		///		&lt;option selected&gt;2005&lt;/option&gt;
		///		&lt;option&gt;2006&lt;/option&gt;
		///		...
		///		&lt;option&gt;2029&lt;/option&gt;
		/// &lt;/select&gt;</code>
		/// As above example shows the year range is hardcoded between 1930 and 2029.
		/// <para>
		/// <paramref name="name"/> is used to generate <b>name</b> and <b>id</b> for each <b>select</b> tag.
		/// Supplied <see cref="String"/> is concatenated with "day", "month", or "year" to create
		/// <see cref="String"/> for the tag attributes.
		/// </para>
		/// </remarks>
		/// <example>This example shows how to use <b>DateTime</b>:
		/// <code>
		/// $HtmlHelper.DateTime( "nameArg", new DateTime( 2005, 07, 15 ) )
		/// </code>
		/// </example>
		public virtual String DateTime(String name, DateTime value)
		{
			return DateTime(name, value, null);
		}

		/// <summary>
		/// Creates three <b>select</b> tags to input day, month and year.
		/// <code>
		/// &lt;select name=&quot;nameArgday&quot; id=&quot;nameArgday&quot; key1=&quot;value1&quot; key3=&quot;value3&quot; key2=&quot;value2&quot; &gt; ... &lt;/select&gt;
		/// &lt;select name=&quot;nameArgmonth&quot; id=&quot;nameArgmonth&quot; key1=&quot;value1&quot; key3=&quot;value3&quot; key2=&quot;value2&quot; &gt; ... &lt;/select&gt;
		/// &lt;select name=&quot;nameArgyear&quot; id=&quot;nameArgyear&quot; key1=&quot;value1&quot; key3=&quot;value3&quot; key2=&quot;value2&quot; &gt; ... &lt;/select&gt;
		/// </code>
		/// </summary>
		/// <param name="name">Name to use with <b>name</b> and <b>id</b> arguments of the <b>select</b> tag.</param>
		/// <param name="value"><see cref="System.DateTime"/> to use for default selected date.</param>
		/// <param name="attributes">Additional attributes for <b>select</b> tags.</param>
		/// <returns>A long HTML string with three <b>select</b> tag input date.</returns>
		/// <remarks>Calling <c>DateTime( "nameArg", new DateTime( 2005, 07, 15 ), IDictionary )</c> results in:
		/// <code>
		/// &lt;select name=&quot;nameArgday&quot; id=&quot;nameArgday&quot; key1=&quot;value1&quot; key2=&quot;value2&quot; &gt;	&lt;option&gt;1&lt;/option&gt;
		/// 	&lt;option&gt;2&lt;/option&gt;
		/// 	...
		/// 	&lt;option&gt;14&lt;/option&gt;
		///		&lt;option selected&gt;15&lt;/option&gt;
		///		&lt;option&gt;16&lt;/option&gt;
		///		...
		///		&lt;option&gt;30&lt;/option&gt;
		///		&lt;option&gt;31&lt;/option&gt;
		///	 &lt;/select&gt; &lt;select name=&quot;nameArgmonth&quot; id=&quot;nameArgmonth&quot; key1=&quot;value1&quot; key2=&quot;value2&quot; &gt;	&lt;option&gt;1&lt;/option&gt;
		///		&lt;option&gt;2&lt;/option&gt;
		///		...
		///		&lt;option&gt;6&lt;/option&gt;
		///		&lt;option selected&gt;7&lt;/option&gt;
		///		&lt;option&gt;8&lt;/option&gt;
		///		...
		///		&lt;option&gt;11&lt;/option&gt;
		///		&lt;option&gt;12&lt;/option&gt;
		///	 &lt;/select&gt; &lt;select name=&quot;nameArgyear&quot; id=&quot;nameArgyear&quot; key1=&quot;value1&quot; key2=&quot;value2&quot; &gt;	&lt;option&gt;1930&lt;/option&gt;
		///		&lt;option&gt;1931&lt;/option&gt;
		///		...
		///		&lt;option&gt;2004&lt;/option&gt;
		///		&lt;option selected&gt;2005&lt;/option&gt;
		///		&lt;option&gt;2006&lt;/option&gt;
		///		...
		///		&lt;option&gt;2029&lt;/option&gt;
		/// &lt;/select&gt;</code>
		/// As above example shows the year range is hardcoded between 1930 and 2029.
		/// <para>
		/// <paramref name="name"/> is used to generate <b>name</b> and <b>id</b> for each <b>select</b> tag.
		/// Supplied <see cref="String"/> is concatenated with "day", "month", or "year" to create
		/// <see cref="String"/> for the tag attributes.
		/// </para>
		/// <para>
		/// <paramref name="attributes"/> is used to generate additional attributes for each of the <b>select</b> tags.
		/// <see cref="IDictionary.Keys"/> are used to name attributes.
		/// <see cref="IDictionary.Values"/> are used to assign those attributes values.
		/// </para>
		/// </remarks>
		/// <example>This example shows how to use <b>DateTime</b>:
		/// <code>
		/// $HtmlHelper.DateTime( "nameArg", new DateTime( 2005, 07, 15 ), IDictionary )
		/// </code>
		/// </example>
		public virtual String DateTime(String name, DateTime value, IDictionary attributes)
		{
			String[] days = new String[31];
			int index = 0;
			for(int i = 1; i < 32; i++)
				days[index++] = i.ToString();
			var monthFriendlyNames = CommonUtils.ObtainEntryAndRemove(attributes, "monthFriendly",
			                                                          "Jan,Feb,Mar,Apr,May,Jun,Jul,Aug,Sept,Oct,Nov,Dec").Split(',');
			if( monthFriendlyNames.Length != 12)
			{
				throw new MonoRailException("The monthFriendly parameter must specify exactly 12 month names. It currently specifies {0} name(s).", monthFriendlyNames.Length);
			}
			String[] months = new String[12];
			index = 0;
			for (int i = 1; i < 13; i++)
				months[index++] = i.ToString();

			for (int i = 1; i < 13; i++)
				monthFriendlyNames[i - 1] = monthFriendlyNames[i - 1].Trim();

			int startYear = int.Parse(CommonUtils.ObtainEntryAndRemove(attributes, "startYear", "1930"));
			int finishYear = int.Parse(CommonUtils.ObtainEntryAndRemove(attributes, "endYear", "2030"));
			int tmp = 2000;
			if(startYear > finishYear) {
				tmp = finishYear;
				finishYear = startYear;
				startYear = tmp;
			}
			String[] years = new String[finishYear - startYear + 1];
			index = 0;
			for(int i = startYear; i <= finishYear; i++)
				years[index++] = i.ToString();

			StringBuilder sb = new StringBuilder(1024);
			sb.Append(Select(name + "day", attributes));
			sb.Append(CreateOption("--", "1"));
			sb.Append(CreateOptionsFromPrimitiveArray(days, value.Day.ToString()));
			sb.Append(EndSelect());
			sb.Append(' ');

			// accessibility
			if(attributes.Contains("tabindex"))
			{
				attributes["tabindex"] = string.Format("{0}", int.Parse(CommonUtils.ObtainEntry(attributes, "tabindex", "0")) + 1);
			}
			sb.Append(Select(name + "month", attributes));
			sb.Append(CreateOption("--", "1"));
	
			if(monthFriendlyNames.Length == 12)
			{
				index = 0;
				foreach (string monthName in monthFriendlyNames)
				{
					sb.AppendFormat("\t<option{0} value={1}>{2}</option>\r\n",
													++index == value.Month ? " selected=\"selected\"" : "", index, monthName);
				}
			}
			else
			{
				sb.Append(CreateOptionsFromPrimitiveArray(months, value.Month.ToString()));	
			}
			
			sb.Append(EndSelect());
			sb.Append(' ');
			// accessibility
			if (attributes.Contains("tabindex"))
			{
				attributes["tabindex"] = string.Format("{0}", int.Parse(CommonUtils.ObtainEntry(attributes, "tabindex", "0")) + 1);
			}
			sb.Append(Select(name + "year", attributes));
			sb.Append(CreateOption("----", "1"));
			sb.Append(CreateOptionsFromPrimitiveArray(years, value.Year.ToString()));
			sb.Append(EndSelect());

			return sb.ToString();
		}

		#endregion

		#region TextArea

		/// <summary>
		/// Creates a text area element.
		/// <code>&lt;textarea id=&quot;nameArg&quot; name=&quot;nameArg&quot; cols=&quot;10&quot; rows=&quot;10&quot;&gt;valueArg&lt;/textarea&gt;</code>
		/// </summary>
		/// <param name="name">Value for <b>name</b> and <b>id</b> attributes.</param>
		/// <param name="cols"><b>cols</b> attribute value.</param>
		/// <param name="rows"><b>rows</b> attribute value.</param>
		/// <param name="value">Text to place inside of the text area.</param>
		/// <returns>HTML string with closed <b>textarea</b> tag.</returns>
		/// <remarks>Calling <c>TextArea( "nameArg", 10, 10, "valueArg" )</c> results in:
		/// <code>&lt;textarea id=&quot;nameArg&quot; name=&quot;nameArg&quot; cols=&quot;10&quot; rows=&quot;10&quot;&gt;valueArg&lt;/textarea&gt;</code>
		/// </remarks>
		/// <example>This example shows how to use <see cref="TextArea"/>:
		/// <code>
		/// $HtmlHelper.TextArea( "nameArg", 10, 20, "Text inside text area." )
		/// </code>
		/// </example>
		public virtual String TextArea(String name, int cols, int rows, String value)
		{
			return String.Format("<textarea id=\"{0}\" name=\"{0}\" cols=\"{1}\" rows=\"{2}\">{3}</textarea>",
			                     name, cols, rows, value);
		}

		#endregion

		#region InputButton

		/// <overloads>This method has three overloads.</overloads>
		/// <summary>
		/// Creates an input element of the button type.
		/// <code>&lt;input type=&quot;button&quot; value=&quot;valueArg&quot; /&gt;</code>
		/// </summary>
		/// <param name="value"><see cref="String"/> for <b>value</b> attribute.</param>
		/// <returns>HTML string with button type <b>input</b> tag.</returns>
		/// <remarks>Calling <c>InputButton( "valueArg" )</c> results in:
		/// <code>&lt;input type=&quot;button&quot; name=&quot;valueArg&quot; value=&quot;valueArg&quot; /&gt;</code>
		/// </remarks>
		/// <example>This example shows how to use <b>InputButton</b>:
		/// <code>
		/// $HtmlHelper.InputButton( "valueArg" )
		/// </code>
		/// </example>
		public virtual String InputButton(String value)
		{
			return InputButton(value, value);
		}

		/// <summary>
		/// Creates an input element of the button type.
		/// <code>&lt;input type=&quot;button&quot; name=&quot;nameArg&quot; id=&quot;nameArg&quot; value=&quot;valueArg&quot; /&gt;</code>
		/// </summary>
		/// <param name="name">Value for <b>name</b> and <b>id</b> attributes.</param>
		/// <param name="value"><see cref="String"/> for <b>value</b> attribute.</param>
		/// <returns>HTML string with button type <b>input</b> tag.</returns>
		public virtual String InputButton(String name, String value)
		{
			return InputButton(name, value, null);
		}

		/// <summary>
		/// Creates an input element of the button type.
		/// <code>&lt;input type=&quot;button&quot; name=&quot;nameArg&quot; id=&quot;nameArg&quot; value=&quot;valueArg&quot; /&gt;</code>
		/// </summary>
		/// <param name="name">Value for <b>name</b> and <b>id</b> attributes.</param>
		/// <param name="value"><see cref="String"/> for <b>value</b> attribute.</param>
		/// <param name="attributes">Additional attributes for the <b>input</b> tag.</param>
		/// <returns>HTML string with button type <b>input</b> tag.</returns>
		public virtual String InputButton(String name, String value, IDictionary attributes)
		{
			return String.Format("<input type=\"button\" name=\"{0}\" value=\"{1}\" {2} />",
			                     name, value, GetAttributes(attributes));
		}

		#endregion

		#region InputCheckbox

		/// <overloads>This method has three overloads.</overloads>
		/// <summary>
		/// Creates an input element of the checkbox type.
		/// <code>&lt;input type=&quot;checkbox&quot; name=&quot;nameArg&quot; id=&quot;nameArg&quot; value=&quot;valueArg&quot; /&gt;</code>
		/// </summary>
		/// <param name="name">Value for <c>name</c> and <c>id</c> attributes.</param>
		/// <param name="value"><see cref="String"/> for <c>value</c> attribute.</param>
		/// <returns>HTML string with checkbox type <c>input</c> tag.</returns>
		/// <remarks>Calling <c>InputCheckbox( "name", "1" )</c> results in:
		/// <code>&lt;input type=&quot;checkbox&quot; name=&quot;name&quot; id=&quot;name&quot; value=&quot;1&quot; /&gt;</code>
		/// </remarks>
		public virtual String InputCheckbox(String name, Object value)
		{
			return InputCheckbox(name, value, null);
		}

		/// <summary>
		/// Creates an input element of the checkbox type.
		/// <code>&lt;input type=&quot;checkbox&quot; name=&quot;nameArg&quot; id=&quot;nameArg&quot; value=&quot;valueArg&quot; /&gt;</code>
		/// </summary>
		/// <param name="name">Value for <b>name</b> and <b>id</b> attributes.</param>
		/// <param name="value"><see cref="String"/> for <b>value</b> attribute.</param>
		/// <param name="isChecked">If true, adds the <c>checked</c> attributed to the tag</param>
		/// <returns>HTML string with checkbox type <b>input</b> tag.</returns>
		public virtual String InputCheckbox(String name, Object value, bool isChecked)
		{
			IDictionary attributes = null;

			if (isChecked)
			{
				attributes = new Hashtable();
				attributes["checked"] = null;
			}

			return InputCheckbox(name, value, attributes);
		}

		/// <summary>
		/// Creates an input element of the checkbox type.
		/// <code>&lt;input type=&quot;checkbox&quot; name=&quot;nameArg&quot; id=&quot;nameArg&quot; value=&quot;valueArg&quot; /&gt;</code>
		/// </summary>
		/// <param name="name">Value for <b>name</b> and <b>id</b> attributes.</param>
		/// <param name="value"><see cref="String"/> for <b>value</b> attribute.</param>
		/// <param name="attributes">Additional attributes for the <b>input</b> tag.</param>
		/// <returns>HTML string with checkbox type <b>input</b> tag.</returns>
		public virtual String InputCheckbox(String name, Object value, IDictionary attributes)
		{
			return String.Format("<input type=\"checkbox\" name=\"{0}\" id=\"{0}\" value=\"{1}\" {2} />",
			                     name, value, GetAttributes(attributes));
		}

		#endregion

		#region InputRadio

		/// <overloads>This method has two overloads.</overloads>
		/// <summary>
		/// Creates an input element of the radio type.
		/// <code>&lt;input type=&quot;radio&quot; name=&quot;nameArg&quot; value=&quot;valueArg&quot; /&gt;</code>
		/// </summary>
		/// <param name="name">Value for <b>name</b> attribute.</param>
		/// <param name="value"><see cref="String"/> for <b>value</b> attribute.</param>
		/// <returns>HTML string with radio type <b>input</b> tag.</returns>
		/// <remarks>Calling <c>InputRadio( "name", "1" )</c> results in:
		/// <code>&lt;input type=&quot;radio&quot; name=&quot;name&quot; value=&quot;1&quot; /&gt;</code>
		/// </remarks>
		public virtual String InputRadio(String name, Object value)
		{
			return InputRadio(name, value, null);
		}

		/// <summary>
		/// Creates an input element of the radio type.
		/// <code>&lt;input type=&quot;radio&quot; name=&quot;nameArg&quot; value=&quot;valueArg&quot; /&gt;</code>
		/// </summary>
		/// <param name="name">Value for <b>name</b> attribute.</param>
		/// <param name="value"><see cref="String"/> for <b>value</b> attribute.</param>
		/// <param name="attributes">Additional attributes for the <b>input</b> tag.</param>
		/// <returns>HTML string with radio type <b>input</b> tag.</returns>
		public virtual String InputRadio(String name, Object value, IDictionary attributes)
		{
			return String.Format("<input type=\"radio\" name=\"{0}\" value=\"{1}\" {2} />",
			                     name, value, GetAttributes(attributes));
		}

		#endregion

		#region InputFile

		/// <overloads>This method has two overloads.</overloads>
		/// <summary>
		/// Creates an input element of the file type.
		/// <code>&lt;input type=&quot;file&quot; name=&quot;nameArg&quot; /&gt;</code>
		/// </summary>
		/// <param name="name">Value for <b>name</b> attribute.</param>
		/// <returns>HTML string with file type <b>input</b> tag.</returns>
		/// <remarks>Calling <c>InputFile( "name" )</c> results in:
		/// <code>&lt;input type=&quot;file&quot; name=&quot;name&quot; /&gt;</code>
		/// </remarks>
		public virtual String InputFile(String name)
		{
			return InputFile(name, null);
		}

		/// <summary>
		/// Creates an input element of the file type.
		/// <code>&lt;input type=&quot;file&quot; name=&quot;nameArg&quot; /&gt;</code>
		/// </summary>
		/// <param name="name">Value for <b>name</b> attribute.</param>
		/// <param name="attributes">Additional attributes for the <b>input</b> tag.</param>
		/// <returns>HTML string with file type <b>input</b> tag.</returns>
		public virtual String InputFile(String name, IDictionary attributes)
		{
			return String.Format("<input type=\"file\" name=\"{0}\" {1} />",
			                     name, GetAttributes(attributes));
		}

		#endregion

		#region InputText

		/// <overloads>This method has four overloads.</overloads>
		/// <summary>
		/// Creates an input element of the text type.
		/// <code>&lt;input type=&quot;text&quot; name=&quot;nameArg&quot; id=&quot;nameArg&quot; value=&quot;valueArg&quot; /&gt;</code>
		/// </summary>
		/// <param name="name">Value for <b>name</b> and <b>id</b> attributes.</param>
		/// <param name="value"><see cref="String"/> for <b>value</b> attribute.</param>
		/// <returns>HTML string with text type <b>input</b> tag.</returns>
		/// <remarks>Calling <c>InputText( "nameArg", "valueArg" )</c> results in:
		/// <code>&lt;input type=&quot;text&quot; name=&quot;nameArg&quot; id=&quot;nameArg&quot; value=&quot;valueArg&quot; /&gt;</code>
		/// </remarks>
		/// <example>This example shows how to use <b>InputText</b>:
		/// <code>
		/// $HtmlHelper.InputText( "nameArg", "valueArg" )
		/// </code>
		/// </example>
		public virtual String InputText(String name, String value)
		{
			return InputText(name, value, name);
		}

		/// <summary>
		/// Creates an input element of the text type of specified
		/// <paramref name="size"/> and <paramref name="maxlength"/>.
		/// <code>&lt;input type=&quot;text&quot; name=&quot;nameArg&quot; id=&quot;nameArg&quot; value=&quot;valueArg&quot; size=&quot;10&quot; maxlength=&quot;10&quot; /&gt;</code>
		/// </summary>
		/// <param name="name">Value for <b>name</b> and <b>id</b> attributes.</param>
		/// <param name="value"><see cref="String"/> for <b>value</b> attribute.</param>
		/// <param name="size"><b>size</b> attribute value.</param>
		/// <param name="maxlength"><b>maxlength</b> attribute value.</param>
		/// <returns>HTML string with text type <b>input</b> tag.</returns>
		/// <remarks>Calling <c>InputText( "nameArg", "valueArg", 10, 10 )</c> results in:
		/// <code>&lt;input type=&quot;text&quot; name=&quot;nameArg&quot; id=&quot;nameArg&quot; value=&quot;valueArg&quot; size=&quot;10&quot; maxlength=&quot;10&quot; /&gt;</code>
		/// </remarks>
		/// <example>This example shows how to use <b>InputText</b>:
		/// <code>
		/// $HtmlHelper.InputText( "nameArg", "valueArg", 10, 10 )
		/// </code>
		/// </example>
		public virtual String InputText(String name, String value, int size, int maxlength)
		{
			return String.Format("<input type=\"text\" name=\"{0}\" id=\"{0}\" value=\"{1}\" size=\"{2}\" maxlength=\"{3}\" />",
			                     name, value, size, maxlength);
		}

		/// <summary>
		/// Creates an input element of the text type with specified
		/// <paramref name="size"/>, <paramref name="maxlength"/> and <paramref name="attributes"/>.
		/// <code>&lt;input type=&quot;text&quot; name=&quot;nameArg&quot; id=&quot;nameArg&quot; value=&quot;valueArg&quot; size=&quot;10&quot; maxlength=&quot;10&quot; /&gt;</code>
		/// </summary>
		/// <param name="name">Value for <b>name</b> and <b>id</b> attributes.</param>
		/// <param name="value"><see cref="String"/> for <b>value</b> attribute.</param>
		/// <param name="size"><b>size</b> attribute value.</param>
		/// <param name="maxlength"><b>maxlength</b> attribute value.</param>
		/// <param name="attributes">Additional attributes for the <b>input</b> tag.</param>
		/// <returns>HTML string with text type <b>input</b> tag.</returns>
		/// <remarks>Calling <c>InputText( "nameArg", "valueArg", 10, 10, IDictionary )</c> results in:
		/// <code>&lt;input type=&quot;text&quot; name=&quot;nameArg&quot; id=&quot;nameArg&quot; value=&quot;valueArg&quot; size=&quot;10&quot; maxlength=&quot;10&quot; key1=&quot;value1&quot; key2=&quot;value2&quot; /&gt;</code>
		/// <para>
		/// <paramref name="attributes"/> is used to generate additional attributes for the <b>label</b> tag.
		/// <see cref="IDictionary.Keys"/> are used to name attributes.
		/// <see cref="IDictionary.Values"/> are used to assign those attributes values.
		/// </para>
		/// </remarks>
		/// <example>This example shows how to use <b>InputText</b>:
		/// <code>
		/// $HtmlHelper.InputText( "nameArg", "valueArg", 10, 10, IDictionary )
		/// </code>
		/// </example>
		public virtual String InputText(String name, String value, int size, int maxlength, IDictionary attributes)
		{
			return
				String.Format("<input type=\"text\" name=\"{0}\" id=\"{0}\" value=\"{1}\" size=\"{2}\" maxlength=\"{3}\" {4}/>",
				              name, value, size, maxlength, GetAttributes(attributes));
		}

		/// <summary>
		/// Creates an input element of the text type with custom <paramref name="name"/> and <paramref name="id"/>.
		/// <code>&lt;input type=&quot;text&quot; name=&quot;nameArg&quot; id=&quot;idArg&quot; value=&quot;valueArg&quot; /&gt;</code>
		/// </summary>
		/// <param name="name"><b>name</b> attribute value.</param>
		/// <param name="value"><see cref="String"/> for <b>value</b> attribute.</param>
		/// <param name="id"><b>id</b> attribute value.</param>
		/// <returns>HTML string with text type <b>input</b> tag.</returns>
		/// <remarks>Calling <c>InputText( "nameArg", "valueArg", "idArg" )</c> results in:
		/// <code>&lt;input type=&quot;text&quot; name=&quot;nameArg&quot; id=&quot;idArg&quot; value=&quot;valueArg&quot; /&gt;</code>
		/// </remarks>
		/// <example>This example shows how to use <b>InputText</b>:
		/// <code>
		/// $HtmlHelper.InputText( "nameArg", "valueArg", "idArg" )
		/// </code>
		/// </example>
		public virtual String InputText(String name, String value, String id)
		{
			if (id == null) id = name;

			return String.Format("<input type=\"text\" name=\"{0}\" id=\"{1}\" value=\"{2}\" />", name, id, value);
		}

		/// <overloads>This method has two overloads.</overloads>
		/// <summary>
		/// Creates a hidden type input element.
		/// <code>&lt;input type=&quot;hidden&quot; name=&quot;nameArg&quot; id=&quot;nameArg&quot; value=&quot;valueArg&quot; /&gt;</code>
		/// </summary>
		public virtual String InputText(String name, String value, IDictionary attributes)
		{
			return String.Format("<input type=\"text\" name=\"{0}\" id=\"{0}\" value=\"{1}\" {2}/>",
			                     name, value, GetAttributes(attributes));
		}

		#endregion

		#region InputPassword

		/// <summary>
		/// Creates an input element of password type
		/// </summary>
		public virtual String InputPassword(String name)
		{
			return InputPassword(name, null);
		}

		/// <summary>
		/// Creates an input element of password type
		/// </summary>
		public virtual String InputPassword(String name, String value)
		{
			return InputPassword(name, value, null);
		}

		/// <summary>
		/// Creates an input element of password type
		/// </summary>
		public virtual String InputPassword(String name, String value, IDictionary attributes)
		{
			if (value == null) value = String.Empty;

			return String.Format("<input type=\"password\" name=\"{0}\" id=\"{0}\" value=\"{1}\" {2}/>",
			                     name, value, GetAttributes(attributes));
		}

		#endregion

		#region InputHidden

		/// <summary>
		/// Creates an input hidden element
		/// </summary>
		/// <param name="name">Value for <b>name</b> and <b>id</b> attributes.</param>
		/// <param name="value"><see cref="String"/> for <b>value</b> attribute.</param>
		/// <returns>HTML string with hidden type <b>input</b> tag.</returns>
		/// <remarks>Calling <c>InputHidden( "nameArg", "valueArg" )</c> results in:
		/// <code>&lt;input type=&quot;hidden&quot; name=&quot;nameArg&quot; id=&quot;nameArg&quot; value=&quot;valueArg&quot; /&gt;</code>
		/// </remarks>
		/// <example>This example shows how to use <b>InputHidden</b>:
		/// <code>
		/// $HtmlHelper.InputHidden( "nameArg", "valueArg" )
		/// </code>
		/// </example>
		public virtual String InputHidden(String name, String value)
		{
			return String.Format("<input type=\"hidden\" name=\"{0}\" id=\"{0}\" value=\"{1}\" />", name, value);
		}

		/// <summary>
		/// Creates a hidden type input element.
		/// <code>&lt;input type=&quot;hidden&quot; name=&quot;nameArg&quot; id=&quot;nameArg&quot; value=&quot;object&quot; /&gt;</code>
		/// </summary>
		/// <param name="name">Value for <b>name</b> and <b>id</b> attributes.</param>
		/// <param name="value"><see cref="object"/> to supply <see cref="String"/> for <b>value</b> attribute.</param>
		/// <returns>HTML string with hidden type <b>input</b> tag.</returns>
		/// <remarks>Calling <c>InputHidden( "nameArg", object )</c> results in:
		/// <code>&lt;input type=&quot;hidden&quot; name=&quot;nameArg&quot; id=&quot;nameArg&quot; value=&quot;object&quot; /&gt;</code>
		/// <para>
		/// <see cref="String"/> for <b>value</b> attribute is retrieved from <paramref name="value"/>
		/// via <see cref="object.ToString"/>.
		/// </para>
		/// <para>If <paramref name="value"/> is <c>null</c> <see cref="String.Empty"/> is used as the <b>value</b>
		/// <see cref="String"/>.</para>
		/// </remarks>
		/// <example>This example shows how to use <b>InputHidden</b>:
		/// <code>
		/// $HtmlHelper.InputHidden( "nameArg", object  )
		/// </code>
		/// </example>
		public virtual String InputHidden(String name, object value)
		{
			return InputHidden(name, value != null ? value.ToString() : String.Empty);
		}

		#endregion

		#region SubmitButton

		///<overloads>This method has two overloads.</overloads>
		/// <summary>
		/// Creates a submit button.
		/// <code>&lt;input type=&quot;submit&quot; value=&quot;valueArg&quot;  /&gt;</code>
		/// </summary>
		/// <param name="value"><see cref="String"/> for <b>value</b> attribute.</param>
		/// <returns>HTML string with submit type <b>input</b> tag.</returns>
		/// <remarks>Calling <c>SubmitButton( "valueArg" )</c> results in:
		/// <code>&lt;input type=&quot;submit&quot; value=&quot;valueArg&quot;  /&gt;</code>
		/// </remarks>
		/// <example>This example shows how to use <b>SubmitButton</b>:
		/// <code>
		/// $HtmlHelper.SubmitButton( "valueArg" )
		/// </code>
		/// </example>
		public virtual String SubmitButton(String value)
		{
			return SubmitButton(value, null);
		}

		/// <summary>
		/// Creates a submit button.
		/// <code>&lt;input type=&quot;submit&quot; value=&quot;valueArg&quot; key1=&quot;value1&quot; key2=&quot;value2&quot;  /&gt;</code>
		/// </summary>
		/// <param name="value"><see cref="String"/> for <b>value</b> attribute.</param>
		/// <param name="attributes">Additional attributes for the <b>input</b> tag.</param>
		/// <remarks>Calling <c>SubmitButton( "valueArg", IDictionary )</c> results in:
		/// <code>&lt;input type=&quot;submit&quot; value=&quot;valueArg&quot; key1=&quot;value1&quot; key2=&quot;value2&quot;  /&gt;</code>
		/// <para>
		/// <paramref name="attributes"/> is used to generate additional attributes for the <b>label</b> tag.
		/// <see cref="IDictionary.Keys"/> are used to name attributes.
		/// <see cref="IDictionary.Values"/> are used to assign those attributes values.
		/// </para>
		/// </remarks>
		/// <example>This example shows how to use <b>SubmitButton</b>:
		/// <code>
		/// $HtmlHelper.SubmitButton( "valueArg", IDictionary )
		/// </code>
		/// </example>
		public virtual String SubmitButton(String value, IDictionary attributes)
		{
			StringBuilder sb = new StringBuilder();
			StringWriter sbWriter = new StringWriter(sb);
			HtmlTextWriter writer = new HtmlTextWriter(sbWriter);

			writer.WriteBeginTag("input");
			writer.WriteAttribute("type", "submit");
			writer.WriteAttribute("value", value);
			writer.Write(" ");
			writer.Write(GetAttributes(attributes));
			writer.Write(HtmlTextWriter.SelfClosingTagEnd);
			writer.WriteLine();

			return sbWriter.ToString();
		}

		#endregion

		#region Select

		///<overloads>This method has two overloads.</overloads>
		/// <summary>
		/// Creates opening <b>select</b> tag.
		/// <code>&lt;select name=&quot;nameArg&quot; id=&quot;nameArg&quot;&gt;</code>
		/// </summary>
		/// <param name="name">Value for <b>name</b> and <b>id</b> attributes.</param>
		/// <returns>HTML string with opening <b>select</b> tag.</returns>
		/// <remarks>Calling <c>Select( "nameArg" )</c> results in:
		/// <code>&lt;select name=&quot;nameArg&quot; id=&quot;nameArg&quot;&gt;</code>
		/// </remarks>
		/// <example>This example shows how to use <b>Select</b> together with <see cref="EndSelect"/>:
		/// <code>
		/// $HtmlHelper.Select( "nameArg" )
		/// ...
		/// $HtmlHelper.EndSelect()
		/// </code>
		/// </example>
		public virtual String Select(String name)
		{
			return String.Format("<select name=\"{0}\" id=\"{0}\">", name);
		}

		/// <summary>
		/// Creates opening <b>select</b> tag.
		/// <code>&lt;select name=&quot;nameArg&quot; id=&quot;nameArg&quot; key1=&quot;value1&quot; key2=&quot;value2&quot; &gt;</code>
		/// </summary>
		/// <param name="name">Value for <b>name</b> and <b>id</b> attributes.</param>
		/// <param name="attributes">Additional attributes for the <b>select</b> tag.</param>
		/// <remarks>Calling <c>Select( "nameArg", IDictionary )</c> results in:
		/// <code>&lt;select name=&quot;nameArg&quot; id=&quot;nameArg&quot; key1=&quot;value1&quot; key2=&quot;value2&quot; &gt;</code>
		/// <para>
		/// <paramref name="attributes"/> is used to generate additional attributes for the <b>label</b> tag.
		/// <see cref="IDictionary.Keys"/> are used to name attributes.
		/// <see cref="IDictionary.Values"/> are used to assign those attributes values.
		/// </para>
		/// </remarks>
		/// <example>This example shows how to use <b>Select</b> together with <see cref="EndSelect"/>:
		/// <code>
		/// $HtmlHelper.Select( "nameArg", IDictionary )
		/// ...
		/// $HtmlHelper.EndSelect()
		/// </code>
		/// </example>
		public virtual String Select(String name, IDictionary attributes)
		{
			String htmlAttrs = GetAttributes(attributes);

			return String.Format("<select name=\"{0}\" id=\"{0}\" {1}>", name, htmlAttrs);
		}

		/// <summary>
		/// Creates a closing <b>select</b> tag.
		/// </summary>
		/// <remarks>Calling <c>EndSelect()</c> results in:
		/// <code>&lt;/select&gt;</code>
		/// </remarks>
		/// <example>This example shows how to use <see cref="Select(String)"/> together with <b>EndSelect</b>:
		/// <code>
		/// $HtmlHelper.Select( "nameArg" )
		/// ...
		/// $HtmlHelper.EndSelect()
		/// </code>
		/// </example>
		public virtual String EndSelect()
		{
			return String.Format("</select>");
		}

		#endregion

		#region Create options

		/// <summary>
		/// Creates an opening <b>optgroup</b> element.
		/// </summary>
		/// <param name="label">The label attribute.</param>
		/// <returns>An opening <b>optgroup</b> element.</returns>
		public virtual String OptionGroup(String label)
		{
			return String.Format("<optgroup label=\"{0}\">", label);
		}

		/// <summary>
		/// Creates a closing <b>optgroup</b> element.
		/// </summary>
		/// <returns>A closing <b>optgroup</b> element.</returns>
		public virtual String EndOptionGroup()
		{
			return String.Format("</optgroup>");
		}

		/// <summary>
		/// TODO: Document this!
		/// </summary>
		public virtual String CreateOption(String text, object value)
		{
			return CreateOption(text, value, null);
		}

		/// <summary>
		/// TODO: Document this!
		/// </summary>
		/// <remarks>
		/// Valid html attributes include: selected and disabled
		/// </remarks>
		public virtual String CreateOption(String text, object value, IDictionary htmlAttributes)
		{
			if (value != null)
			{
				return String.Format("<option {0} value=\"{1}\">{2}</option>", GetAttributes(htmlAttributes), value, text);
			}
			else
			{
				return String.Format("<option {0}>{1}</option>", GetAttributes(htmlAttributes), text);
			}
		}

		/// <summary>
		/// Creates <b>option</b> elements from <see cref="Array"/>. Marks the
		/// option that matches the <paramref name="selected"/> argument (if provided).
		/// <code>
		/// &lt;option&gt;0&lt;/option&gt;
		/// &lt;option&gt;1&lt;/option&gt;
		/// ...
		/// &lt;option&gt;5&lt;/option&gt;
		/// &lt;option selected&gt;selectedArg&lt;/option&gt;
		/// &lt;option&gt;object&lt;/option&gt;
		/// </code>
		/// </summary>
		/// <param name="elems">Array of values for each <b>option</b> tag.</param>
		/// <param name="selected">Name of the <b>option</b> tag to mark selected.</param>
		/// <returns>HTML string with array of <b>option</b> tags.</returns>
		/// <remarks>Calling <c>CreateOptionsFromPrimitiveArray( Array, "selectedArg" )</c> results in:
		/// <code>
		/// &lt;option&gt;0&lt;/option&gt;
		/// &lt;option&gt;1&lt;/option&gt;
		/// &lt;option&gt;2&lt;/option&gt;
		/// &lt;option&gt;3&lt;/option&gt;
		/// &lt;option&gt;4&lt;/option&gt;
		/// &lt;option&gt;5&lt;/option&gt;
		/// &lt;option selected&gt;selectedArg&lt;/option&gt;
		/// &lt;option&gt;object&lt;/option&gt;
		/// </code>
		/// <para>
		/// Elements in the array are converted to <see cref="String"/> using <see cref="StringBuilder.AppendFormat(String,object)"/>.
		/// </para>
		/// </remarks>
		/// <example>This example shows how to use <b>CreateOptionsFromPrimitiveArray</b>:
		/// <code>
		/// $HtmlHelper.CreateOptionsFromPrimitiveArray( Array, "selectedArg" )
		/// </code>
		/// </example>
		public virtual String CreateOptionsFromPrimitiveArray(Array elems, String selected)
		{
			if (elems.GetLength(0) == 0) return String.Empty;

			StringBuilder sb = new StringBuilder();

			foreach(object elem in elems)
			{
				sb.AppendFormat("\t<option{0}>{1}</option>\r\n",
				                elem.ToString() == selected ? " selected=\"selected\"" : "", elem);
			}

			return sb.ToString();
		}

		///<overloads>This method has two overloads.</overloads>
		/// <summary>
		/// Creates options elements from an <see cref="Array"/>.
		/// <code>
		/// &lt;option value=&quot;valueProp&quot;&gt;textProp&lt;/option&gt;
		/// &lt;option value=&quot;0&quot;&gt;textProp2&lt;/option&gt;
		/// &lt;option value=&quot;5&quot;&gt;textProp3&lt;/option&gt;
		/// </code>
		/// <seealso cref="CreateOptions(ICollection,String,String)"/>
		/// </summary>
		/// <param name="elems">Collection of objects each of which describes an <b>option</b> tag.</param>
		/// <param name="textProperty">Name of the <paramref name="elems"/>
		/// objects property with the value for each <b>option</b> tag's
		/// text.</param>
		/// <param name="valueProperty">Name of the <paramref
		/// name="elems"/> objects property with the value for each
		/// <b>option</b> tag's <b>value</b> attribute value.</param>
		/// <returns>HTML string with array of <b>option</b> tags.</returns>
		/// 
		/// <remarks>Calling <c>CreateOptionsFromArray( Array, "textPropertyArg",
		/// "valuePropertyArg", object )</c> with specific type objects results in:
		/// <code>
		/// &lt;option value=&quot;valueProp&quot;&gt;textProp&lt;/option&gt;
		/// &lt;option value=&quot;0&quot;&gt;textProp2&lt;/option&gt;
		/// &lt;option value=&quot;5&quot;&gt;textProp3&lt;/option&gt;
		/// </code>
		/// <para>Calling <c>CreateOptionsFromArray( Array, "textPropertyArg",
		/// "valuePropertyArg", object )</c> with random type objects results in:
		/// <code>
		/// &lt;option&gt;0&lt;/option&gt;
		/// &lt;option&gt;1&lt;/option&gt;
		/// &lt;option&gt;2&lt;/option&gt;
		/// &lt;option&gt;3&lt;/option&gt;
		/// &lt;option&gt;4&lt;/option&gt;
		/// &lt;option&gt;5&lt;/option&gt;
		/// &lt;option&gt;object&lt;/option&gt;
		/// &lt;option&gt;MR.Logic.Controllers.HtmlHelperController+SampleClass&lt;/option&gt;
		/// </code>
		/// Notice that the last <b>option</b> was generated from an object of the type
		/// with the properties specified by <paramref
		/// name="textProperty"/> and <pararef name="valueProperty"/>, but the method
		/// is already in the mode of working with random type objects.
		/// <note>Explanation bellow describes two different modes of working with the method.</note>
		/// </para>
		/// <para>There are two possible usages of the method depending on
		/// the types of <see cref="object"/>s which can be present in
		/// <paramref name="elems"/>:
		/// <list type="definition">
		/// <item>
		///		<term>Random type objects</term>
		///		<description>Array is full of
		/// random type objects. Properties specified by <paramref
		/// name="textProperty"/> and <pararef name="valueProperty"/> aren't
		/// used. Instead <b>value</b> argument is ommited and <see
		/// cref="Object.ToString"/> is invoked on each item in <paramref
		/// name="elems"/> to retrieve text for an <b>option</b> tag.
		///		</description>
		///	</item>
		/// <item>
		///		<term>Single type objects</term>
		///		<description>Array is objects
		/// of one time. In this case <paramref name="textProperty"/> and
		/// <paramref name="valueProperty"/> can specify the names of the
		/// properties of that type to use for <b>option</b> tags
		/// generation.
		///		</description>
		///	</item>
		/// </list>
		/// <note>You cannot mix <i>random type objects</i> and <i>specific type objects</i>.
		/// <b>CreateOptionsFromArray</b>
		/// is looking at the first item in the <paramref name="elems"/>
		/// collection to get <see cref="MethodInfo"/> to access specified
		/// properties. If usage is mixed either an unexpected exception will be
		/// thrown or options will have unexpected strings.
		/// </note>
		/// </para>
		/// <para><b>CreateOptionsFromArray</b> relies on <see cref="CreateOptions(ICollection,String,String)"/> to generate
		/// all <b>option</b> tags.</para>
		/// </remarks>
		/// <example>This example shows how to use <b>CreateOptions</b>:
		/// <code>
		/// $HtmlHelper.CreateOptionsFromArray( ICollection, "textPropertyArg", "valuePropertyArg" )
		/// </code>
		/// </example>
		public virtual String CreateOptionsFromArray(Array elems, String textProperty, String valueProperty)
		{
			return CreateOptions(elems, textProperty, valueProperty);
		}

		/// <summary>
		/// Creates options elements from an <see cref="Array"/>.
		/// <code>
		/// &lt;option value=&quot;valueProp&quot; selected&gt;textProp&lt;/option&gt;
		/// &lt;option value=&quot;0&quot;&gt;textProp2&lt;/option&gt;
		/// &lt;option value=&quot;5&quot;&gt;textProp3&lt;/option&gt;
		/// </code>
		/// <seealso cref="CreateOptions(ICollection,String,String,object)"/>
		/// </summary>
		/// <param name="elems">Collection of objects each of which describes an <b>option</b> tag.</param>
		/// <param name="textProperty">Name of the <paramref name="elems"/>
		/// objects property with the value for each <b>option</b> tag's
		/// text.</param>
		/// <param name="valueProperty">Name of the <paramref
		/// name="elems"/> objects property with the value for each
		/// <b>option</b> tag's <b>value</b> attribute value.</param>
		/// <param name="selectedValue"><see cref="object"/> indicating which
		/// <b>option</b> tag is to be marked with <b>selected</b>
		/// attribute.</param>
		/// <returns>HTML string with array of <b>option</b> tags.</returns>
		/// 
		/// <remarks>Calling <c>CreateOptionsFromArray( Array, "textPropertyArg",
		/// "valuePropertyArg", object )</c> with specific type objects results in:
		/// <code>
		/// &lt;option value=&quot;valueProp&quot; selected&gt;textProp&lt;/option&gt;
		/// &lt;option value=&quot;0&quot;&gt;textProp2&lt;/option&gt;
		/// &lt;option value=&quot;5&quot;&gt;textProp3&lt;/option&gt;
		/// </code>
		/// <para>Calling <c>CreateOptionsFromArray( Array, "textPropertyArg",
		/// "valuePropertyArg", object )</c> with random type objects results in:
		/// <code>
		/// &lt;option&gt;0&lt;/option&gt;
		/// &lt;option&gt;1&lt;/option&gt;
		/// &lt;option&gt;2&lt;/option&gt;
		/// &lt;option&gt;3&lt;/option&gt;
		/// &lt;option&gt;4&lt;/option&gt;
		/// &lt;option&gt;5&lt;/option&gt;
		/// &lt;option selected&gt;object&lt;/option&gt;
		/// &lt;option&gt;MR.Logic.Controllers.HtmlHelperController+SampleClass&lt;/option&gt;
		/// </code>
		/// Notice that the last <b>option</b> was generated from an object of the type
		/// with the properties specified by <paramref
		/// name="textProperty"/> and <pararef name="valueProperty"/>, but the method
		/// is already in the mode of working with random type objects.
		/// <note>Explanation bellow describes two different modes of working with the method.</note>
		/// </para>
		/// <para>There are two possible usages of the method depending on
		/// the types of <see cref="object"/>s which can be present in
		/// <paramref name="elems"/>:
		/// <list type="definition">
		/// <item>
		///		<term>Random type objects</term>
		///		<description>Array is full of
		/// random type objects. Properties specified by <paramref
		/// name="textProperty"/> and <pararef name="valueProperty"/> aren't
		/// used. Instead <b>value</b> argument is ommited and <see
		/// cref="Object.ToString"/> is invoked on each item in <paramref
		/// name="elems"/> to retrieve text for an <b>option</b> tag.
		///		</description>
		///	</item>
		/// <item>
		///		<term>Single type objects</term>
		///		<description>Array is objects
		/// of one time. In this case <paramref name="textProperty"/> and
		/// <paramref name="valueProperty"/> can specify the names of the
		/// properties of that type to use for <b>option</b> tags
		/// generation.
		///		</description>
		///	</item>
		/// </list>
		/// <note>You cannot mix <i>random type objects</i> and <i>specific type objects</i>.
		/// <b>CreateOptionsFromArray</b>
		/// is looking at the first item in the <paramref name="elems"/>
		/// collection to get <see cref="MethodInfo"/> to access specified
		/// properties. If usage is mixed either an unexpected exception will be
		/// thrown or options will have unexpected strings.
		/// </note>
		/// </para>
		/// <para><b>CreateOptionsFromArray</b> relies on <see cref="CreateOptions(ICollection,String,String,object)"/> to generate
		/// all <b>option</b> tags.</para>
		/// </remarks>
		/// <example>This example shows how to use <b>CreateOptions</b>:
		/// <code>
		/// $HtmlHelper.CreateOptionsFromArray( ICollection, "textPropertyArg", "valuePropertyArg", object )
		/// </code>
		/// </example>
		public virtual String CreateOptionsFromArray(Array elems, String textProperty, String valueProperty, object selectedValue)
		{
			return CreateOptions(elems, textProperty, valueProperty, selectedValue);
		}

		///<overloads>This method has two overloads.</overloads>
		/// <summary>
		/// Creates options elements from an <see cref="ICollection"/>.
		/// <code>
		/// &lt;option value=&quot;valueProp&quot;&gt;textProp&lt;/option&gt;
		/// &lt;option value=&quot;0&quot;&gt;textProp2&lt;/option&gt;
		/// &lt;option value=&quot;5&quot;&gt;textProp3&lt;/option&gt;
		/// </code>
		/// </summary>
		/// <param name="elems">Collection of objects each of which describes an <b>option</b> tag.</param>
		/// <param name="textProperty">Name of the <paramref name="elems"/>
		/// objects property with the value for each <b>option</b> tag's
		/// text.</param>
		/// <param name="valueProperty">Name of the <paramref
		/// name="elems"/> objects property with the value for each
		/// <b>option</b> tag's <b>value</b> attribute value.</param>
		/// <returns>HTML string with array of <b>option</b> tags.</returns>
		/// 
		/// <remarks>Calling <c>CreateOptions( ICollection, "textPropertyArg",
		/// "valuePropertyArg", object )</c> with specific type objects results in:
		/// <code>
		/// &lt;option value=&quot;valueProp&quot;&gt;textProp&lt;/option&gt;
		/// &lt;option value=&quot;0&quot;&gt;textProp2&lt;/option&gt;
		/// &lt;option value=&quot;5&quot;&gt;textProp3&lt;/option&gt;
		/// </code>
		/// <para>Calling <c>CreateOptions( ICollection, "textPropertyArg",
		/// "valuePropertyArg", object )</c> with random type objects results in:
		/// <code>
		/// &lt;option&gt;0&lt;/option&gt;
		/// &lt;option&gt;1&lt;/option&gt;
		/// &lt;option&gt;2&lt;/option&gt;
		/// &lt;option&gt;3&lt;/option&gt;
		/// &lt;option&gt;4&lt;/option&gt;
		/// &lt;option&gt;5&lt;/option&gt;
		/// &lt;option&gt;object&lt;/option&gt;
		/// &lt;option&gt;MR.Logic.Controllers.HtmlHelperController+SampleClass&lt;/option&gt;
		/// </code>
		/// Notice that the last <b>option</b> was generated from an object of the type
		/// with the properties specified by <paramref
		/// name="textProperty"/> and <pararef name="valueProperty"/>, but the method
		/// is already in the mode of working with random type objects.
		/// <note>Explanation bellow describes two different modes of working with the method.</note>
		/// </para>
		/// <para>There are two possible usages of the method depending on
		/// the types of <see cref="object"/>s which can be present in
		/// <paramref name="elems"/>:
		/// <list type="definition">
		/// <item>
		///		<term>Random type objects</term>
		///		<description>Array is full of
		/// random type objects. Properties specified by <paramref
		/// name="textProperty"/> and <pararef name="valueProperty"/> aren't
		/// used. Instead <b>value</b> argument is ommited and <see
		/// cref="Object.ToString"/> is invoked on each item in <paramref
		/// name="elems"/> to retrieve text for an <b>option</b> tag.
		///		</description>
		///	</item>
		/// <item>
		///		<term>Single type objects</term>
		///		<description>Array is objects
		/// of one time. In this case <paramref name="textProperty"/> and
		/// <paramref name="valueProperty"/> can specify the names of the
		/// properties of that type to use for <b>option</b> tags
		/// generation.
		///		</description>
		///	</item>
		/// </list>
		/// <note>You cannot mix <i>random type objects</i> and <i>specific type objects</i>. <b>CreateOptions</b>
		/// is looking at the first item in the <paramref name="elems"/>
		/// collection to get <see cref="MethodInfo"/> to access specified
		/// properties. If usage is mixed either an unexpected exception will be
		/// thrown or options will have unexpected strings.
		/// </note>
		/// </para>
		/// </remarks>
		/// <example>This example shows how to use <b>CreateOptions</b>:
		/// <code>
		/// $HtmlHelper.CreateOptions( ICollection, "textPropertyArg", "valuePropertyArg" )
		/// </code>
		/// </example>
		public virtual String CreateOptions(ICollection elems, String textProperty, String valueProperty)
		{
			return CreateOptions(elems, textProperty, valueProperty, null);
		}

		/// <summary>
		/// Creates options elements from an <see cref="ICollection"/>.
		/// <code>
		/// &lt;option value=&quot;valueProp&quot; selected&gt;textProp&lt;/option&gt;
		/// &lt;option value=&quot;0&quot;&gt;textProp2&lt;/option&gt;
		/// &lt;option value=&quot;5&quot;&gt;textProp3&lt;/option&gt;
		/// </code>
		/// </summary>
		/// <param name="elems">Collection of objects each of which describes an <b>option</b> tag.</param>
		/// <param name="textProperty">Name of the <paramref name="elems"/>
		/// objects property with the value for each <b>option</b> tag's
		/// text.</param>
		/// <param name="valueProperty">Name of the <paramref
		/// name="elems"/> objects property with the value for each
		/// <b>option</b> tag's <b>value</b> attribute value.</param>
		/// <param name="selectedValue"><see cref="object"/> indicating which
		/// <b>option</b> tag is to be marked with <b>selected</b>
		/// attribute.</param>
		/// <returns>HTML string with array of <b>option</b> tags.</returns>
		/// 
		/// <remarks>Calling <c>CreateOptions( ICollection, "textPropertyArg",
		/// "valuePropertyArg", object )</c> with specific type objects results in:
		/// <code>
		/// &lt;option value=&quot;valueProp&quot; selected&gt;textProp&lt;/option&gt;
		/// &lt;option value=&quot;0&quot;&gt;textProp2&lt;/option&gt;
		/// &lt;option value=&quot;5&quot;&gt;textProp3&lt;/option&gt;
		/// </code>
		/// <para>Calling <c>CreateOptions( ICollection, "textPropertyArg",
		/// "valuePropertyArg", object )</c> with random type objects results in:
		/// <code>
		/// &lt;option&gt;0&lt;/option&gt;
		/// &lt;option&gt;1&lt;/option&gt;
		/// &lt;option&gt;2&lt;/option&gt;
		/// &lt;option&gt;3&lt;/option&gt;
		/// &lt;option&gt;4&lt;/option&gt;
		/// &lt;option&gt;5&lt;/option&gt;
		/// &lt;option selected&gt;object&lt;/option&gt;
		/// &lt;option&gt;MR.Logic.Controllers.HtmlHelperController+SampleClass&lt;/option&gt;
		/// </code>
		/// Notice that the last <b>option</b> was generated from an object of the type
		/// with the properties specified by <paramref
		/// name="textProperty"/> and <pararef name="valueProperty"/>, but the method
		/// is already in the mode of working with random type objects.
		/// <note>Explanation bellow describes two different modes of working with the method.</note>
		/// </para>
		/// <para>There are two possible usages of the method depending on
		/// the types of <see cref="object"/>s which can be present in
		/// <paramref name="elems"/>:
		/// <list type="definition">
		/// <item>
		///		<term>Random type objects</term>
		///		<description>Array is full of
		/// random type objects. Properties specified by <paramref
		/// name="textProperty"/> and <pararef name="valueProperty"/> aren't
		/// used. Instead <b>value</b> argument is ommited and <see
		/// cref="Object.ToString"/> is invoked on each item in <paramref
		/// name="elems"/> to retrieve text for an <b>option</b> tag.
		///		</description>
		///	</item>
		/// <item>
		///		<term>Single type objects</term>
		///		<description>Array is objects
		/// of one time. In this case <paramref name="textProperty"/> and
		/// <paramref name="valueProperty"/> can specify the names of the
		/// properties of that type to use for <b>option</b> tags
		/// generation.
		///		</description>
		///	</item>
		/// </list>
		/// <note>You cannot mix <i>random type objects</i> and <i>specific type objects</i>.
		/// <b>CreateOptions</b>
		/// is looking at the first item in the <paramref name="elems"/>
		/// collection to get <see cref="MethodInfo"/> to access specified
		/// properties. If usage is mixed either an unexpected exception will be
		/// thrown or options will have unexpected strings.
		/// </note>
		/// </para>
		/// </remarks>
		/// <example>This example shows how to use <b>CreateOptions</b>:
		/// <code>
		/// $HtmlHelper.CreateOptions( ICollection, "textPropertyArg", "valuePropertyArg", object )
		/// </code>
		/// </example>
		public virtual String CreateOptions(ICollection elems, String textProperty, String valueProperty, object selectedValue)
		{
			if (elems == null) throw new ArgumentNullException("elems");

			if (elems.Count == 0) return String.Empty;

			IEnumerator enumerator = elems.GetEnumerator();
			enumerator.MoveNext();
			object guidanceElem = enumerator.Current;

			bool isMultiple = (selectedValue != null && selectedValue.GetType().IsArray);

			MethodInfo defaultValueMethodInfo = GetMethod(guidanceElem, valueProperty);
			MethodInfo defaultTextMethodInfo = null;

			if (textProperty != null)
			{
				defaultTextMethodInfo = GetMethod(guidanceElem, textProperty);
			}

			StringBuilder sb = new StringBuilder();

			foreach(object elem in elems)
			{
				if (elem == null) continue;

				object value = null;

				MethodInfo valueMethodInfo = defaultValueMethodInfo;
				MethodInfo textMethodInfo = defaultTextMethodInfo;

				if (elem.GetType() != guidanceElem.GetType())
				{
					if (valueProperty != null)
					{
						valueMethodInfo = GetMethod(elem, valueProperty);
					}
					if (textProperty != null)
					{
						textMethodInfo = GetMethod(elem, textProperty);
					}
				}

				if (valueMethodInfo != null) value = valueMethodInfo.Invoke(elem, null);

				object text = textMethodInfo != null ? textMethodInfo.Invoke(elem, null) : elem.ToString();

				if (value != null)
				{
					bool selected = IsSelected(value, selectedValue, isMultiple);

					sb.AppendFormat("\t<option {0} value=\"{1}\">{2}</option>\r\n",
					                selected ? "selected=\"selected\"" : "", value, text);
				}
				else
				{
					bool selected = IsSelected(text, selectedValue, isMultiple);

					sb.AppendFormat("\t<option {0} >{1}</option>\r\n",
					                selected ? "selected=\"selected\"" : "", text);
				}
			}

			return sb.ToString();
		}

		/// <summary>
		/// Determines whether the specified value is selected.
		/// </summary>
		/// <param name="value">Value to be tested.</param>
		/// <param name="selectedValue">Selected value.</param>
		/// <param name="isMultiple"><see langword="true"/> if <paramref name="selectedValue"/> is
		/// <see cref="Type.IsArray"/>; otherwise, <see langword="false"/>.</param>
		/// <returns>
		/// 	<see langword="true"/> if the specified <paramref name="value"/> is selected; otherwise, <see langword="false"/>.
		/// </returns>
		/// <remarks>Specified <paramref name="value"/> is selected if it <see cref="Object.Equals(object)"/>
		/// to the <paramref name="selectedValue"/>. Or if <paramref name="selectedValue"/> is an
		/// array <paramref name="value"/> is selected if <see cref="Array.IndexOf(Array, object)"/> can find it
		/// in <paramref name="selectedValue"/>.</remarks>
		private static bool IsSelected(object value, object selectedValue, bool isMultiple)
		{
			if (!isMultiple)
			{
				return value.Equals(selectedValue);
			}
			else
			{
				return Array.IndexOf((Array) selectedValue, value) != -1;
			}
		}

		/// <summary>
		/// Gets the property get method.
		/// </summary>
		/// <param name="elem">Object specifying the type for which to get the method.</param>
		/// <param name="property">Property name.</param>
		/// <returns><see cref="MethodInfo"/> to be used to retrieve the property value.
		/// If <paramref name="property"/> is <c>null</c> <c>null</c> is returned.</returns>
		/// <remarks>This method is used to get the <see cref="MethodInfo"/> to retrieve
		/// specified property from the specified type.</remarks>
		/// <exception cref="ArgumentNullException">Thrown is <paramref name="elem"/> is <c>null</c>.</exception>
		private static MethodInfo GetMethod(object elem, String property)
		{
			if (elem == null) throw new ArgumentNullException("elem");
			if (property == null) return null;

			return
				elem.GetType().GetMethod("get_" + property, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
		}

		#endregion

		#region List

		///<overloads>This method has two overloads.</overloads>
		/// <summary>
		/// Builds an unordered <b>ul</b> list from supplied <see cref="ICollection"/>.
		/// <code>
		/// &lt;ul&gt;
		/// &lt;li&gt;0&lt;/li&gt;
		/// ...
		/// &lt;li&gt;object&lt;/li&gt;
		/// &lt;/ul&gt;
		/// </code>
		/// </summary>
		/// <param name="elements">Collection with items to use for the list generation.</param>
		/// <returns>HTML string with <b>ul</b> tag list.</returns>
		/// <remarks>Calling <c>BuildUnorderedList( ICollection )</c> results in:
		/// <code>
		/// &lt;ul&gt;
		/// &lt;li&gt;0&lt;/li&gt;
		/// &lt;li&gt;1&lt;/li&gt;
		/// &lt;li&gt;2&lt;/li&gt;
		/// &lt;li&gt;3&lt;/li&gt;
		/// &lt;li&gt;4&lt;/li&gt;
		/// &lt;li&gt;5&lt;/li&gt;
		/// &lt;li&gt;object&lt;/li&gt;
		/// &lt;/ul&gt;
		/// </code>
		/// <para>Items in <paramref name="elements"/> are converted to string through
		/// <see cref="Object.ToString"/>.</para>
		/// </remarks>
		/// <example>This example shows how to use <b>BuildUnorderedList</b>:
		/// <code>
		/// $HtmlHelper.BuildUnorderedList( ICollection )
		/// </code>
		/// </example>
		public virtual String BuildUnorderedList(ICollection elements)
		{
			return BuildUnorderedList(elements, null, null);
		}

		/// <summary>
		/// Builds an unordered <b>ul</b> list from supplied <see cref="ICollection"/> with
		/// <b>ul</b> and <b>li</b> tags CSS class set to supplied attributes.
		/// <code>
		/// &lt;ol class=&quot;styleClassArg&quot;&gt;
		/// &lt;li class=&quot;itemClassArg&quot;&gt;0&lt;/li&gt;
		/// ...
		/// &lt;li class=&quot;itemClassArg&quot;&gt;object&lt;/li&gt;
		/// &lt;/ol&gt;
		/// </code>
		/// </summary>
		/// <param name="elements">Collection with items to use for the list generation.</param>
		/// <param name="styleClass">CSS class name of the list <b>ul</b> tag.</param>
		/// <param name="itemClass">CSS class name of the list item <b>li</b> tag.</param>
		/// <returns>HTML string with <b>ul</b> tag list.</returns>
		/// <remarks>Calling <c>BuildUnorderedList( ICollection, "styleClassArg", "itemClassArg" )</c> results in:
		/// <code>
		/// &lt;ol class=&quot;styleClassArg&quot;&gt;
		/// &lt;li class=&quot;itemClassArg&quot;&gt;0&lt;/li&gt;
		/// &lt;li class=&quot;itemClassArg&quot;&gt;1&lt;/li&gt;
		/// &lt;li class=&quot;itemClassArg&quot;&gt;2&lt;/li&gt;
		/// &lt;li class=&quot;itemClassArg&quot;&gt;3&lt;/li&gt;
		/// &lt;li class=&quot;itemClassArg&quot;&gt;4&lt;/li&gt;
		/// &lt;li class=&quot;itemClassArg&quot;&gt;5&lt;/li&gt;
		/// &lt;li class=&quot;itemClassArg&quot;&gt;object&lt;/li&gt;
		/// &lt;/ol&gt;
		/// </code>
		/// <para>Items in <paramref name="elements"/> are converted to string through
		/// <see cref="Object.ToString"/>.</para>
		/// </remarks>
		/// <example>This example shows how to use <b>BuildOrderedList</b>:
		/// <code>
		/// $HtmlHelper.BuildUnorderedList( ICollection, "styleClassArg", "itemClassArg" )
		/// </code>
		/// </example>
		public virtual String BuildUnorderedList(ICollection elements, String styleClass, String itemClass)
		{
			return BuildList("ul", elements, styleClass, itemClass);
		}

		///<overloads>This method has two overloads.</overloads>
		/// <summary>
		/// Builds an ordered <b>ol</b> list from supplied <see cref="ICollection"/>.
		/// <code>
		/// &lt;ol&gt;
		/// &lt;li&gt;0&lt;/li&gt;
		/// ...
		/// &lt;li&gt;object&lt;/li&gt;
		/// &lt;/ol&gt;
		/// </code>
		/// </summary>
		/// <param name="elements">Collection with items to use for the list generation.</param>
		/// <returns>HTML string with <b>ol</b> tag list.</returns>
		/// <remarks>Calling <c>BuildOrderedList( ICollection )</c> results in:
		/// <code>
		/// &lt;ol&gt;
		/// &lt;li&gt;0&lt;/li&gt;
		/// &lt;li&gt;1&lt;/li&gt;
		/// &lt;li&gt;2&lt;/li&gt;
		/// &lt;li&gt;3&lt;/li&gt;
		/// &lt;li&gt;4&lt;/li&gt;
		/// &lt;li&gt;5&lt;/li&gt;
		/// &lt;li&gt;object&lt;/li&gt;
		/// &lt;/ol&gt;
		/// </code>
		/// <para>Items in <paramref name="elements"/> are converted to string through
		/// <see cref="Object.ToString"/>.</para>
		/// </remarks>
		/// <example>This example shows how to use <b>BuildOrderedList</b>:
		/// <code>
		/// $HtmlHelper.BuildOrderedList( ICollection )
		/// </code>
		/// </example>
		public virtual String BuildOrderedList(ICollection elements)
		{
			return BuildOrderedList(elements, null, null);
		}

		/// <summary>
		/// Builds an ordered <b>ol</b> list from supplied <see cref="ICollection"/> with
		/// <b>ol</b> and <b>li</b> tags CSS class set to supplied attributes.
		/// <code>
		/// &lt;ol class=&quot;styleClassArg&quot;&gt;
		/// &lt;li class=&quot;itemClassArg&quot;&gt;0&lt;/li&gt;
		/// ...
		/// &lt;li class=&quot;itemClassArg&quot;&gt;object&lt;/li&gt;
		/// &lt;/ol&gt;
		/// </code>
		/// </summary>
		/// <param name="elements">Collection with items to use for the list generation.</param>
		/// <param name="styleClass">CSS class name of the list <b>ol</b> tag.</param>
		/// <param name="itemClass">CSS class name of the list item <b>li</b> tag.</param>
		/// <returns>HTML string with <b>ol</b> tag list.</returns>
		/// <remarks>Calling <c>BuildOrderedList( ICollection, "styleClassArg", "itemClassArg" )</c> results in:
		/// <code>
		/// &lt;ol class=&quot;styleClassArg&quot;&gt;
		/// &lt;li class=&quot;itemClassArg&quot;&gt;0&lt;/li&gt;
		/// &lt;li class=&quot;itemClassArg&quot;&gt;1&lt;/li&gt;
		/// &lt;li class=&quot;itemClassArg&quot;&gt;2&lt;/li&gt;
		/// &lt;li class=&quot;itemClassArg&quot;&gt;3&lt;/li&gt;
		/// &lt;li class=&quot;itemClassArg&quot;&gt;4&lt;/li&gt;
		/// &lt;li class=&quot;itemClassArg&quot;&gt;5&lt;/li&gt;
		/// &lt;li class=&quot;itemClassArg&quot;&gt;object&lt;/li&gt;
		/// &lt;/ol&gt;
		/// </code>
		/// <para>Items in <paramref name="elements"/> are converted to string through
		/// <see cref="Object.ToString"/>.</para>
		/// </remarks>
		/// <example>This example shows how to use <b>BuildOrderedList</b>:
		/// <code>
		/// $HtmlHelper.BuildOrderedList( ICollection, "styleClassArg", "itemClassArg" )
		/// </code>
		/// </example>
		public virtual String BuildOrderedList(ICollection elements, String styleClass, String itemClass)
		{
			return BuildList("ol", elements, styleClass, itemClass);
		}

		/// <summary>
		/// Builds a list with list tag specified by <paramref name="tag"/>
		/// from supplied <see cref="ICollection"/> with
		/// list tag and <b>li</b> tags CSS class set to supplied attributes.
		/// <code>
		/// &lt;listTag class=&quot;styleClassArg&quot;&gt;
		/// &lt;li class=&quot;itemClassArg&quot;&gt;0&lt;/li&gt;
		/// ...
		/// &lt;li class=&quot;itemClassArg&quot;&gt;object&lt;/li&gt;
		/// &lt;/listTag&gt;
		/// </code>
		/// </summary>
		/// <param name="tag">List tag name.</param>
		/// <param name="elements">Collection with items to use for the list generation.</param>
		/// <param name="styleClass">CSS class name of the list <b>ol</b> tag.</param>
		/// <param name="itemClass">CSS class name of the list item <b>li</b> tag.</param>
		/// <returns>HTML string with list of the requested type.</returns>
		/// <remarks>This method is can be used to generate custom type HTML list.
		/// Currently HTML support only two types of lists ordered (<b>ol</b> tag) and unodered
		/// (<b>ul</b>tag). In general this method should be used by other methods responsible
		/// for constructing some specific list.
		/// <para>
		/// Calling <c>BuildList( "listTag", ICollection, "styleClassArg", "itemClassArg" )</c> results in:
		/// <code>
		/// &lt;listTag class=&quot;styleClassArg&quot;&gt;
		/// &lt;li class=&quot;itemClassArg&quot;&gt;0&lt;/li&gt;
		/// &lt;li class=&quot;itemClassArg&quot;&gt;1&lt;/li&gt;
		/// &lt;li class=&quot;itemClassArg&quot;&gt;2&lt;/li&gt;
		/// &lt;li class=&quot;itemClassArg&quot;&gt;3&lt;/li&gt;
		/// &lt;li class=&quot;itemClassArg&quot;&gt;4&lt;/li&gt;
		/// &lt;li class=&quot;itemClassArg&quot;&gt;5&lt;/li&gt;
		/// &lt;li class=&quot;itemClassArg&quot;&gt;object&lt;/li&gt;
		/// &lt;/listTag&gt;
		/// </code>
		/// </para>
		/// <para>Items in <paramref name="elements"/> are converted to string through
		/// <see cref="Object.ToString"/>.</para>
		/// </remarks>
		/// <example>This example shows how to use <b>BuildList</b>:
		/// <code>
		/// BuildList("ol", elements, styleClass, itemClass);
		/// </code>
		/// </example>
		private static String BuildList(String tag, ICollection elements, String styleClass, String itemClass)
		{
			StringBuilder sb = new StringBuilder();
			StringWriter sbWriter = new StringWriter(sb);
			HtmlTextWriter writer = new HtmlTextWriter(sbWriter);

			writer.WriteBeginTag(tag);

			if (styleClass != null)
			{
				writer.WriteAttribute("class", styleClass);
			}

			writer.Write(HtmlTextWriter.TagRightChar);
			writer.WriteLine();

			foreach(object item in elements)
			{
				if (item == null) continue;

				writer.WriteLine(BuildListItem(item.ToString(), itemClass));
			}

			writer.WriteEndTag(tag);
			writer.WriteLine();

			return sbWriter.ToString();
		}

		/// <summary>
		/// Generates a list item <b>li</b> tag.
		/// <code>
		/// &lt;li class=&quot;itemClassArg&quot;&gt;object&lt;/li&gt;
		/// </code>
		/// </summary>
		/// <param name="item">Item text.</param>
		/// <param name="itemClass">Item CSS class name.</param>
		/// <returns>HTML string with a single <b>li</b> tag.</returns>
		/// <remarks>This method should be used to assist list generation.
		/// <para>
		/// Calling <c>BuildListItem( "object", "itemClassArg" )</c> results in:
		/// <code>
		/// &lt;li class=&quot;itemClassArg&quot;&gt;object&lt;/li&gt;
		/// </code>
		/// </para>
		/// </remarks>
		/// <example>This example shows how to use <b>BuildListItem</b>:
		/// <code>
		/// BuildListItem(item.ToString(), itemClass);
		/// </code>
		/// </example>
		private static String BuildListItem(String item, String itemClass)
		{
			if (itemClass == null)
			{
				return String.Format("<li>{0}</li>", item);
			}
			else
			{
				return String.Format("<li class=\"{0}\">{1}</li>", itemClass, item);
			}
		}

		#endregion
	}
}
