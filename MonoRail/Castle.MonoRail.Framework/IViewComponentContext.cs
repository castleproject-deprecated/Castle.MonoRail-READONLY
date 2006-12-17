// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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
	using System.IO;
	using System.Collections;

	/// <summary>
	/// Pendent
	/// </summary>
	public interface IViewComponentContext
	{
		/// <summary>
		/// Gets the name of the component.
		/// </summary>
		/// <value>The name of the component.</value>
		String ComponentName { get; }

		/// <summary>
		/// Determines whether the current component declaration on the view
		/// has the specified section.
		/// </summary>
		/// <param name="sectionName">Name of the section.</param>
		/// <returns>
		/// <c>true</c> if the specified section exists; otherwise, <c>false</c>.
		/// </returns>
		bool HasSection(String sectionName);

		/// <summary>
		/// Renders the component body.
		/// </summary>
		void RenderBody();

		/// <summary>
		/// Renders the body into the specified <see cref="TextWriter"/>
		/// </summary>
		/// <param name="writer">The writer.</param>
		void RenderBody(TextWriter writer);

		/// <summary>
		/// Renders the the specified section
		/// </summary>
		/// <param name="sectionName">Name of the section.</param>
		void RenderSection(String sectionName);

		/// <summary>
		/// Gets the writer used to render the component
		/// </summary>
		/// <value>The writer.</value>
		TextWriter Writer { get; }

		/// <summary>
		/// Gets the dictionary that holds variables for the
		/// view and for the view component
		/// </summary>
		/// <value>The context vars.</value>
		IDictionary ContextVars { get; }

		/// <summary>
		/// Gets the component parameters that the view has passed
		/// to the component
		/// </summary>
		/// <value>The component parameters.</value>
		IDictionary ComponentParameters { get; }

		/// <summary>
		/// Gets or sets the view to render.
		/// </summary>
		/// <value>The view to render.</value>
		String ViewToRender { get; set; }

		/// <summary>
		/// Gets the view engine instance.
		/// </summary>
		/// <value>The view engine.</value>
		IViewEngine ViewEngine { get; }
	}
}
