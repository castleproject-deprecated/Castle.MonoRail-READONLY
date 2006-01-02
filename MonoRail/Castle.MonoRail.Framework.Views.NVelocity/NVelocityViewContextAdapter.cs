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

using INode = NVelocity.Runtime.Parser.Node.INode;
using InternalContextAdapter = NVelocity.Context.InternalContextAdapter;

namespace Castle.MonoRail.Framework.Views.NVelocity
{
	using System;
	using System.Collections;
	using System.IO;

	public class NVelocityViewContextAdapter : IViewComponentContext
	{
		private readonly INode bodyNode;
		private readonly TextWriter writer;
		private readonly InternalContextAdapter context;
		private readonly String componentName;
		private readonly IDictionary componentParams;
		private readonly IDictionary contextVars = new Hashtable();
		private String viewToRender;

		public NVelocityViewContextAdapter(String componentName, 
			InternalContextAdapter context, TextWriter writer, INode bodyNode, IDictionary componentParams)
		{
			this.context = context;
			this.componentName = componentName;
			this.writer = writer;
			this.bodyNode = bodyNode;
			this.componentParams = componentParams;

			// TODO: Implement IEnumerable on NVelocity context
//			foreach(String key in context.Keys)
//			{
//				contextVars[key] = context.Get(key);
//			}
		}

		public String ComponentName
		{
			get { return componentName; }
		}

		public IDictionary ContextVars
		{
			get { return contextVars; }
		}

		public IDictionary ComponentParameters
		{
			get { return componentParams; }
		}

		public String ViewToRender
		{
			get { return viewToRender; }
			set { viewToRender = value; }
		}

		public TextWriter Writer
		{
			get { return writer; }
		}

		public void RenderBody()
		{
			if (bodyNode == null)
			{
				throw new RailsException("This component does not have a body content to be rendered");
			}

			foreach(DictionaryEntry entry in contextVars)
			{
				context.Put(entry.Key.ToString(), entry.Value);
			}

			bodyNode.render(context, writer);
		}
	}
}
