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
	using System.Collections;

	using Castle.Components.Common.EmailSender;

	/// <summary>
	/// Represents the disacoupled service to use 
	/// MonoRail's view engine to process email templates.
	/// </summary>
	public interface IEmailTemplateService
	{
		/// <summary>
		/// Creates an instance of <see cref="Message"/>
		/// using the specified template for the body
		/// </summary>
		/// <param name="templateName">
		/// Name of the template to load. 
		/// Will look in <c>Views/mail</c> for that template file.
		/// </param>
		/// <param name="parameters">
		/// Dictionary with parameters 
		/// that you can use on the email template
		/// </param>
		/// <returns>An instance of <see cref="Message"/></returns>
		Message RenderMailMessage(String templateName, IDictionary parameters);

		/// <summary>
		/// Creates an instance of <see cref="Message"/>
		/// using the specified template for the body
		/// </summary>
		/// <param name="templateName">
		/// Name of the template to load. 
		/// Will look in <c>Views/mail</c> for that template file.
		/// </param>
		/// <param name="context"></param>
		/// <param name="controller"></param>
		/// <returns>An instance of <see cref="Message"/></returns>
		Message RenderMailMessage(String templateName, IRailsEngineContext context, Controller controller);
	}
}
