namespace NVelocity.Runtime.Parser.Node
{
	using System;
	using System.IO;
	using NVelocity.Context;
	using NVelocity.Runtime.Directive;

	/// <summary>
	/// This class is responsible for handling the pluggable
	/// directives in VTL. ex.  #foreach()
	///
	/// Please look at the Parser.jjt file which is
	/// what controls the generation of this class.
	/// </summary>
	/// <author> <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a></author>
	/// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a></author>
	/// <author> <a href="mailto:kav@kav.dk">Kasper Nielsen</a></author>
	/// <version> $Id: ASTDirective.cs,v 1.3 2003/10/27 13:54:10 corts Exp $</version>
	public class ASTDirective : SimpleNode
	{
		public String DirectiveName
		{
			get { return directiveName; }

			set
			{
				directiveName = value;
				return;
			}

		}

		private Directive directive;
		private String directiveName = "";
		private bool isDirective;

		public ASTDirective(int id) : base(id)
		{
		}

		public ASTDirective(Parser p, int id) : base(p, id)
		{
		}


		/// <summary>Accept the visitor. *
		/// </summary>
		public override Object jjtAccept(ParserVisitor visitor, Object data)
		{
			return visitor.visit(this, data);
		}

		public override Object init(InternalContextAdapter context, Object data)
		{
			base.init(context, data);

			/*
	    *  only do things that are not context dependant
	    */

			if (parser.isDirective(directiveName))
			{
				isDirective = true;

				// create a new instance of the directive
				Type t = parser.getDirective(directiveName).GetType();
				Object o = Activator.CreateInstance(t);
				directive = (Directive) o;

				directive.init(rsvc, context, this);
				directive.setLocation(Line, Column);
			}
			else if (rsvc.isVelocimacro(directiveName, context.CurrentTemplateName))
			{
				/*
		*  we seem to be a Velocimacro.
		*/

				isDirective = true;
				directive = (Directive) rsvc.getVelocimacro(directiveName, context.CurrentTemplateName);

				directive.init(rsvc, context, this);
				directive.setLocation(Line, Column);
			}
			else
			{
				isDirective = false;
			}

			return data;
		}

		public override bool render(InternalContextAdapter context, TextWriter writer)
		{
			/*
	    *  normal processing
	    */

			if (isDirective)
			{
				directive.render(context, writer, this);
			}
			else
			{
				writer.Write("#");
				writer.Write(directiveName);
			}

			return true;
		}

		/// <summary>   Sets the directive name.  Used by the parser.  This keeps us from having to
		/// dig it out of the token stream and gives the parse the change to override.
		/// </summary>

		/// <summary>  Gets the name of this directive.
		/// </summary>
	}
}