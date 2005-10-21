// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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
	using Castle.MonoRail.Framework.Internal;

	/// <summary>
	/// Abstract base class for View Engines.
	/// </summary>
	public abstract class ViewEngineBase : IViewEngine
	{
		private string _viewRootDir;
		private bool _xhtmlRendering;
		private IViewComponentFactory viewComponentFactory;

		/// <summary>
		/// Gets/sets the root directory of views, obtained from the configuration.
		/// </summary>
		public String ViewRootDir
		{
			get { return _viewRootDir; }
			set { _viewRootDir = value; }
		}

		/// <summary>
		/// Gets/sets whether rendering should aim to be XHTML compliant, obtained from the configuration.
		/// </summary>
		public bool XhtmlRendering
		{
			get { return _xhtmlRendering; }
			set { _xhtmlRendering = value; }
		}

		/// <summary>
		/// Gets/sets the factory for <see cref="ViewComponent"/>s
		/// </summary>
		public virtual IViewComponentFactory ViewComponentFactory
		{
			get { return viewComponentFactory; }
			set { viewComponentFactory = value; }
		}

		/// <summary>
		/// Initializes the view engine.
		/// </summary>
		public abstract void Init();

		/// <summary>
		/// Evaluates whether the specified template exists.
		/// </summary>
		/// <returns><c>true</c> if it exists</returns>
		public abstract bool HasTemplate(String templateName);

		/// <summary>
		/// Processes the view - using the templateName to obtain the correct template,
		/// and using the context to output the result.
		/// </summary>
		public abstract void Process(IRailsEngineContext context, Controller controller, String templateName);

		/// <summary>
		/// Wraps the specified content in the layout using the context to output the result.
		/// </summary>
		public abstract void ProcessContents(IRailsEngineContext context, Controller controller, String contents);

		///<summary>
		/// Processes the view - using the templateName to obtain the correct template
		/// and returns the result as a string. No layout is applied!
		/// </summary>
		public virtual String ProcessTemplate(IRailsEngineContext context, Controller controller, String templateName)
		{
			throw new NotImplementedException();
		}

		#region Render Helpers 

		/// <summary>
		/// Sets the HTTP Content-Type header appropriately.
		/// </summary>
		protected virtual void AdjustContentType(IRailsEngineContext context)
		{
			if (XhtmlRendering)
			{
				//Find out what they'll accept
				string httpAccept = context.Request.Headers["Accept"];

				//TODO: Evaluate the q-values of the Accept header

				//Do they accept application/xhtml+xml?
				if(httpAccept != null && httpAccept.IndexOf("application/xhtml+xml") != -1)
				{
					//Send them the proper content type
					context.Response.ContentType = "application/xhtml+xml";

					//TODO: Add the xml prolog for browsers that support it
					//response.Write("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
				}
				else
				{
					//Fall back to text/html for older folk
					context.Response.ContentType = "text/html";
				}

				//Fix up the proxy
				context.Response.AppendHeader("Vary", "Accept");
			}
			else if (context.Response.ContentType == null)
			{
				//Just use HTML
				context.Response.ContentType = "text/html";
			}
		}

		#endregion
	}
}
