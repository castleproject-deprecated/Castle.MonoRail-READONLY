namespace NVelocity.Runtime.Directive
{
	using System;
	using System.IO;
	using NVelocity.Context;
	using NVelocity.Exception;
	using NVelocity.Runtime.Parser;
	using NVelocity.Runtime.Parser.Node;
	using NVelocity.Runtime.Resource;

	/// <summary>
	/// Pluggable directive that handles the #include() statement in VTL.
	/// This #include() can take multiple arguments of either
	/// StringLiteral or Reference.
	/// 
	/// Notes:
	/// -----
	/// 1) The included source material can only come from somewhere in
	/// the TemplateRoot tree for security reasons. There is no way
	/// around this.  If you want to include content from elsewhere on
	/// your disk, use a link from somwhere under Template Root to that
	/// content.
	/// 
	/// 2) By default, there is no output to the render stream in the event of
	/// a problem.  You can override this behavior with two property values :
	/// include.output.errormsg.start
	/// include.output.errormsg.end
	/// If both are defined in velocity.properties, they will be used to
	/// in the render output to bracket the arg string that caused the
	/// problem.
	/// Ex. : if you are working in html then
	/// include.output.errormsg.start=<!-- #include error :
	/// include.output.errormsg.end= -->
	/// might be an excellent way to start...
	/// 
	/// 3) As noted above, #include() can take multiple arguments.
	/// Ex : #include( "foo.vm" "bar.vm" $foo )
	/// will simply include all three if valid to output w/o any
	/// special separator.
	/// 
	/// </summary>
	/// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
	/// </author>
	/// <author> <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a>
	/// </author>
	/// <author> <a href="mailto:kav@kav.dk">Kasper Nielsen</a>
	/// </author>
	/// <version> $Id: Include.cs,v 1.3 2003/10/27 13:54:10 corts Exp $
	///
	/// </version>
	public class Include : Directive
	{
		/// <summary>
		/// Return name of this directive.
		/// </summary>
		public override String Name
		{
			get { return "include"; }
			set { throw new NotSupportedException(); }
		}

		/// <summary> Return type of this directive.</summary>
		public override DirectiveType Type
		{
			get { return DirectiveType.LINE; }
		}

		private String outputMsgStart = "";
		private String outputMsgEnd = "";

		/// <summary>
		/// simple init - init the tree and get the elementKey from
		/// the AST
		/// </summary>
		public override void Init(IRuntimeServices rs, IInternalContextAdapter context, INode node)
		{
			base.Init(rs, context, node);

			// get the msg, and add the space so we don't have to
	    // do it each time
			outputMsgStart = rsvc.GetString(RuntimeConstants.ERRORMSG_START);
			outputMsgStart = outputMsgStart + " ";

			outputMsgEnd = rsvc.GetString(RuntimeConstants.ERRORMSG_END);
			outputMsgEnd = " " + outputMsgEnd;
		}

		/// <summary>
		/// iterates through the argument list and renders every
		/// argument that is appropriate.  Any non appropriate
		/// arguments are logged, but render() continues.
		/// </summary>
		public override bool Render(IInternalContextAdapter context, TextWriter writer, INode node)
		{
			// get our arguments and check them
			int argCount = node.ChildrenCount;

			for (int i = 0; i < argCount; i++)
			{
				// we only handle StringLiterals and References right now
				INode n = node.GetChild(i);

				if (n.Type == ParserTreeConstants.STRING_LITERAL || n.Type == ParserTreeConstants.REFERENCE)
				{
					if (!RenderOutput(n, context, writer))
						OutputErrorToStream(writer, "error with arg " + i + " please see log.");
				}
				else
				{
					//UPGRADE_TODO: The equivalent in .NET for method 'java.Object.toString' may return a different value. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1043"'
					rsvc.Error("#include() error : invalid argument type : " + n.ToString());
					OutputErrorToStream(writer, "error with arg " + i + " please see log.");
				}
			}

			return true;
		}

		/// <summary>
		/// does the actual rendering of the included file
		/// </summary>
		/// <param name="node">AST argument of type StringLiteral or Reference</param>
		/// <param name="context">valid context so we can render References</param>
		/// <param name="writer">output Writer</param>
		/// <returns>boolean success or failure.  failures are logged</returns>
		private bool RenderOutput(INode node, IInternalContextAdapter context, TextWriter writer)
		{
			if (node == null)
			{
				rsvc.Error("#include() error :  null argument");
				return false;
			}

			// does it have a value?  If you have a null reference, then no.
			Object val = node.Value(context);
			if (val == null)
			{
				rsvc.Error("#include() error :  null argument");
				return false;
			}

			// get the path
			String arg = val.ToString();

			Resource resource = null;

			Resource current = context.CurrentResource;

			try
			{
				// get the resource, and assume that we use the encoding of the current template
				// the 'current resource' can be null if we are processing a stream....
				String encoding = null;

				if (current != null)
					encoding = current.Encoding;
				else
					encoding = (String) rsvc.GetProperty(RuntimeConstants.INPUT_ENCODING);

				resource = rsvc.GetContent(arg, encoding);
			}
			catch (ResourceNotFoundException)
			{
				// the arg wasn't found.  Note it and throw
				rsvc.Error("#include(): cannot find resource '" + arg + "', called from template " + context.CurrentTemplateName + " at (" + Line + ", " + Column + ")");
				throw;
			}
			catch (Exception e)
			{
				rsvc.Error("#include(): arg = '" + arg + "',  called from template " + context.CurrentTemplateName + " at (" + Line + ", " + Column + ") : " + e);
			}

			if (resource == null)
				return false;

			writer.Write((String) resource.Data);
			return true;
		}

		/// <summary>
		/// Puts a message to the render output stream if ERRORMSG_START / END
		/// are valid property strings.  Mainly used for end-user template
		/// debugging.
		/// </summary>
		private void OutputErrorToStream(TextWriter writer, String msg)
		{
			if (outputMsgStart != null && outputMsgEnd != null)
			{
				writer.Write(outputMsgStart);
				writer.Write(msg);
				writer.Write(outputMsgEnd);
			}
			return;
		}
	}
}
