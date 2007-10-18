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

namespace Castle.MonoRail.Framework.Routing
{
	using System;
	using System.Collections.Generic;
	using System.Text.RegularExpressions;

	/// <summary>
	/// Pendent
	/// </summary>
	public class PatternRule : IRoutingRule
	{
		private readonly string ruleName;
		private readonly string path;
		private readonly Type controllerType;
		private readonly string action;
		private UrlPathNode[] nodes;

		/// <summary>
		/// Initializes a new instance of the <see cref="PatternRule"/> class.
		/// </summary>
		/// <param name="ruleName">Name of the rule.</param>
		/// <param name="path">The path.</param>
		/// <param name="controllerType">Type of the controller.</param>
		/// <param name="action">The action.</param>
		protected PatternRule(string ruleName, string path, Type controllerType, string action)
		{
			this.ruleName = ruleName;
			this.path = path;
			this.controllerType = controllerType;
			this.action = action;
		}

		/// <summary>
		/// Gets the name of the rule.
		/// </summary>
		/// <value>The name of the rule.</value>
		public string RuleName
		{
			get { return ruleName; }
		}

		/// <summary>
		/// Gets the type of the controller.
		/// </summary>
		/// <value>The type of the controller.</value>
		public Type ControllerType
		{
			get { return controllerType; }
		}

		/// <summary>
		/// Gets the action.
		/// </summary>
		/// <value>The action.</value>
		public string Action
		{
			get { return action; }
		}

		/// <summary>
		/// Matcheses the specified URL.
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <param name="match">The match.</param>
		/// <returns></returns>
		public bool Matches(string url, RouteMatch match)
		{
			string[] pieces = url.Split('/');

			if (pieces.Length != nodes.Length) // This breaks the optional nodes
			{
				return false;
			}

			int curPiece = 0;

			foreach(UrlPathNode node in nodes)
			{
				if (!node.Matches(pieces[curPiece], match))
				{
					return false;
				}
				curPiece++;
			}

			return true;
		}

		/// <summary>
		/// Builds the specified rule name.
		/// </summary>
		/// <param name="ruleName">Name of the rule.</param>
		/// <param name="path">The path.</param>
		/// <param name="controllerType">Type of the controller.</param>
		/// <param name="action">The action.</param>
		/// <returns></returns>
		public static PatternRule Build(string ruleName, string path, Type controllerType, string action)
		{
			if (string.IsNullOrEmpty(ruleName))
			{
				throw new ArgumentNullException("ruleName");
			}
			if (string.IsNullOrEmpty(path))
			{
				throw new ArgumentNullException("path");
			}
			if (string.IsNullOrEmpty(action))
			{
				throw new ArgumentNullException("action");
			}
			if (controllerType.IsAssignableFrom(typeof(Controller)))
			{
				throw new ArgumentException("The specified type does not inherit from the Controller class", "controllerType");
			}

			PatternRule rule = new PatternRule(ruleName, path, controllerType, action);
			rule.Prepare();
			return rule;
		}

		/// <summary>
		/// Prepares this instance.
		/// </summary>
		private void Prepare()
		{
			// TODO: Optional rules must be always at the end. Add check for those

			List<UrlPathNode> tempTokens = new List<UrlPathNode>();

			foreach(string part in path.Split('/'))
			{
				tempTokens.Add(UrlPathNodeFactory.Create(part));
			}

			nodes = tempTokens.ToArray();
		}

		/// <summary>
		/// Pendent
		/// </summary>
		public enum UrlNodeType
		{
			/// <summary>
			/// Pendent
			/// </summary>
			Number,
			/// <summary>
			/// Pendent
			/// </summary>
			String
		}

		/// <summary>
		/// Pendent
		/// </summary>
		public abstract class UrlPathNode
		{
			/// <summary>
			/// Matches the specified piece.
			/// </summary>
			/// <param name="piece">The piece.</param>
			/// <param name="match">The match.</param>
			/// <returns></returns>
			public abstract bool Matches(string piece, RouteMatch match);
		}

		/// <summary>
		/// Pendent
		/// </summary>
		public class LiteralNode : UrlPathNode
		{
			private readonly string token;

			/// <summary>
			/// Initializes a new instance of the <see cref="LiteralNode"/> class.
			/// </summary>
			/// <param name="token">The token.</param>
			public LiteralNode(string token)
			{
				this.token = token;
			}

			/// <summary>
			/// Matches the specified piece.
			/// </summary>
			/// <param name="piece">The piece.</param>
			/// <param name="match">The match.</param>
			/// <returns></returns>
			public override bool Matches(string piece, RouteMatch match)
			{
				if (string.Compare(piece, token, StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					match.Add(token);
					return true;
				}

				return false;
			}
		}

		/// <summary>
		/// Pendent
		/// </summary>
		public class RequiredNode : UrlPathNode
		{
			private readonly static Regex NumberPattern = new Regex("\\d", RegexOptions.Compiled|RegexOptions.Singleline);
			private readonly string nodeName;
			private readonly UrlNodeType type;

			/// <summary>
			/// Initializes a new instance of the <see cref="RequiredNode"/> class.
			/// </summary>
			/// <param name="nodeName">Name of the node.</param>
			/// <param name="type">The type.</param>
			public RequiredNode(string nodeName, UrlNodeType type)
			{
				this.nodeName = nodeName;
				this.type = type;
			}

			/// <summary>
			/// Matches the specified piece.
			/// </summary>
			/// <param name="piece">The piece.</param>
			/// <param name="match">The match.</param>
			/// <returns></returns>
			public override bool Matches(string piece, RouteMatch match)
			{
				if (type == UrlNodeType.String)
				{
					match.AddNamed(nodeName, piece);
					return !string.IsNullOrEmpty(piece);
				}
				else if (type == UrlNodeType.Number)
				{
					if (NumberPattern.Match(piece).Success)
					{
						match.AddNamed(nodeName, piece);
						return true;
					}
				}

				return false;
			}
		}

		/// <summary>
		/// Pendent
		/// </summary>
		public class UrlPathNodeFactory
		{
			/// <summary>
			/// Creates the specified token.
			/// </summary>
			/// <param name="token">The token.</param>
			/// <returns></returns>
			public static UrlPathNode Create(string token)
			{
				if (string.IsNullOrEmpty(token))
				{
					throw new ArgumentNullException("token");
				}

				if (token[0] == '<' || token[0] == '[')
				{
					return ProcessPattern(token);
				}
				else
				{
					return new LiteralNode(token);
				}
			}

			private static UrlPathNode ProcessPattern(string token)
			{
				bool isOptional = token[0] == '[';
				char lastChar = token[token.Length - 1];
				bool isWellFormed = lastChar == (isOptional ? ']' : '>');
				bool hasSpaces = token.IndexOf(' ') != -1;

				if (!isWellFormed)
				{
					throw new ArgumentException("Token is not wellformed. It should end with '>' or ']'");
				}
				if (hasSpaces)
				{
					throw new ArgumentException("Spaces are not allowed on a pattern token. Please check the pattern '" + token + "'");
				}

				token = token.Substring(0, token.Length - 1);

				// Has type?

				UrlNodeType type = UrlNodeType.String;

				int index = token.IndexOf(':');

				if (index != -1)
				{
					// Yeah, so let's check it

					string typeLiteral = token.Substring(index + 1);

					switch(typeLiteral)
					{
						case "number":
							type = UrlNodeType.Number;
							break;
						case "string":
							type = UrlNodeType.String;
							break;
						default:
							throw new ArgumentException("token has invalid value '" + typeLiteral + "'. Expected 'int' or 'string'");
					}
				}

				index = index == -1 ? token.Length : index;

				if (isOptional)
				{
					throw new NotImplementedException("Support for optional nodes not implemented yet");
				}
				else
				{
					return new RequiredNode(token.Substring(1, index - 1), type);
				}
			}
		}
	}
}