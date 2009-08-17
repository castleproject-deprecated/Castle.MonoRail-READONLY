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

namespace NVelocity.Runtime.Directive
{
	using System;
	using System.IO;
	using Context;
	using NVelocity.Runtime.Parser.Node;

	/// <summary>
	/// A very simple directive that leverages the Node.literal()
	/// to grab the literal rendition of a node. We basically
	/// grab the literal value on init(), then repeatedly use
	/// that during render().
	/// </summary>
	/// <author> <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a> </author>
	/// <version> $Id: Literal.cs,v 1.3 2003/10/27 13:54:10 corts Exp $ </version>
	public class Literal : Directive
	{
		/// <summary>Return name of this directive.</summary>
		public override String Name
		{
			get { return "literal"; }
			set { throw new NotSupportedException(); }
		}

		/// <summary> Return type of this directive. </summary>
		public override DirectiveType Type
		{
			get { return DirectiveType.BLOCK; }
		}

		internal String literalText;

		/// <summary>
		/// Store the literal rendition of a node using
		/// the Node.literal().
		/// </summary>
		public override void Init(IRuntimeServices rs, IInternalContextAdapter context, INode node)
		{
			base.Init(rs, context, node);

			literalText = node.GetChild(0).Literal;
		}

		/// <summary> Throw the literal rendition of the block between
		/// #literal()/#end into the writer.
		/// </summary>
		public override bool Render(IInternalContextAdapter context, TextWriter writer, INode node)
		{
			writer.Write(literalText);
			return true;
		}
	}
}