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

using Directive = NVelocity.Runtime.Directive.Directive;
using DirectiveManager = NVelocity.Runtime.Directive.DirectiveManager;

namespace Castle.MonoRail.Framework.Views.NVelocity
{
	using System;
	using System.Collections;
	using Castle.MonoRail.Framework.Views.NVelocity.CustomDirectives;

	/// <summary>
	/// Pendent
	/// </summary>
	public class CustomDirectiveManager : DirectiveManager
	{
		private static readonly String DirectiveSuffix = "directive";

		private Hashtable directives = new Hashtable();

		/// <summary>
		/// Initializes a new instance of the <see cref="CustomDirectiveManager"/> class.
		/// </summary>
		public CustomDirectiveManager()
		{
			RegisterCustomDirectives();
		}

		public override bool Contains(string name)
		{
			return directives.Contains(name) ? true : base.Contains(name);
		}

		public override Directive Create(string name, Stack directiveStack)
		{
			Type customDirective = directives[name] as Type;

			if (customDirective != null)
			{
				object[] args = null;

				if (typeof(AbstractComponentDirective).IsAssignableFrom(customDirective))
				{
					args = new object[] { GetViewComponentFactory(), GetViewEngine() };
				}

				return Activator.CreateInstance(customDirective, args) as Directive;
			}
			else
			{
				return base.Create(name, directiveStack);
			}
		}

		protected void RegisterCustomDirectives()
		{
			RegisterCustomDirective(typeof(BlockComponentDirective));
			RegisterCustomDirective(typeof(ComponentDirective));
			RegisterCustomDirective(typeof(CaptureForDirective));
		}

		private void RegisterCustomDirective(Type type)
		{
			if (!typeof(Directive).IsAssignableFrom(type))
			{
				throw new MonoRailException("{0} is not a subclass of directive", type.FullName);
			}

			String name = type.Name.ToLower(System.Globalization.CultureInfo.InvariantCulture);

			if (name.EndsWith(DirectiveSuffix))
			{
				name = name.Substring(0, name.Length - DirectiveSuffix.Length);
			}

			directives.Add(name, type);
		}

		private IViewComponentFactory GetViewComponentFactory()
		{
			IViewComponentFactory compFactory = (IViewComponentFactory) 
				MonoRailHttpHandler.CurrentContext.GetService(typeof(IViewComponentFactory));

			if (compFactory == null)
			{
				throw new MonoRailException("NVelocityViewEngine: Could not obtain ViewComponentFactory instance");
			}

			return compFactory;
		}

		private IViewEngine GetViewEngine()
		{
			IViewEngine viewEngine = (IViewEngine)
				MonoRailHttpHandler.CurrentContext.GetService(typeof(IViewEngine));

			if (viewEngine == null)
			{
				throw new MonoRailException("NVelocityViewEngine: could not obtain ViewEngine instance");
			}

			return viewEngine;
		}
	}
}
