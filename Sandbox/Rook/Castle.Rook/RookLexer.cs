// $ANTLR 2.7.5 (20050128): "lang.g" -> "RookLexer.cs"$

	using CommonAST					= antlr.CommonAST; 
	using System.Collections;
	using Castle.Rook.AST;

namespace Castle.Rook.Parse
{
	// Generate header specific to lexer CSharp file
	using System;
	using Stream                          = System.IO.Stream;
	using TextReader                      = System.IO.TextReader;
	using Hashtable                       = System.Collections.Hashtable;
	using Comparer                        = System.Collections.Comparer;
	
	using TokenStreamException            = antlr.TokenStreamException;
	using TokenStreamIOException          = antlr.TokenStreamIOException;
	using TokenStreamRecognitionException = antlr.TokenStreamRecognitionException;
	using CharStreamException             = antlr.CharStreamException;
	using CharStreamIOException           = antlr.CharStreamIOException;
	using ANTLRException                  = antlr.ANTLRException;
	using CharScanner                     = antlr.CharScanner;
	using InputBuffer                     = antlr.InputBuffer;
	using ByteBuffer                      = antlr.ByteBuffer;
	using CharBuffer                      = antlr.CharBuffer;
	using Token                           = antlr.Token;
	using IToken                          = antlr.IToken;
	using CommonToken                     = antlr.CommonToken;
	using SemanticException               = antlr.SemanticException;
	using RecognitionException            = antlr.RecognitionException;
	using NoViableAltForCharException     = antlr.NoViableAltForCharException;
	using MismatchedCharException         = antlr.MismatchedCharException;
	using TokenStream                     = antlr.TokenStream;
	using LexerSharedInputState           = antlr.LexerSharedInputState;
	using BitSet                          = antlr.collections.impl.BitSet;
	
	public 	class RookLexer : antlr.CharScanner	, TokenStream
	 {
		public const int EOF = 1;
		public const int NULL_TREE_LOOKAHEAD = 3;
		public const int CLASS = 4;
		public const int EOS = 5;
		public const int INTEGER_LITERAL = 6;
		public const int IDENTIFIER = 7;
		public const int SEMI = 8;
		public const int COMMA = 9;
		public const int ASSIGN = 10;
		public const int NEW_LINE = 11;
		public const int NEW_LINE_CHARACTER = 12;
		public const int NOT_NEW_LINE = 13;
		public const int WHITESPACE = 14;
		public const int SINGLE_LINE_COMMENT = 15;
		public const int IDENTIFIER_START_CHARACTER = 16;
		public const int IDENTIFIER_PART_CHARACTER = 17;
		public const int NUMERIC_LITERAL = 18;
		public const int DECIMAL_DIGIT = 19;
		
		public RookLexer(Stream ins) : this(new ByteBuffer(ins))
		{
		}
		
		public RookLexer(TextReader r) : this(new CharBuffer(r))
		{
		}
		
		public RookLexer(InputBuffer ib)		 : this(new LexerSharedInputState(ib))
		{
		}
		
		public RookLexer(LexerSharedInputState state) : base(state)
		{
			initialize();
		}
		private void initialize()
		{
			caseSensitiveLiterals = true;
			setCaseSensitive(true);
			literals = new Hashtable(100, (float) 0.4, null, Comparer.Default);
			literals.Add("class", 4);
		}
		
		override public IToken nextToken()			//throws TokenStreamException
		{
			IToken theRetToken = null;
tryAgain:
			for (;;)
			{
				IToken _token = null;
				int _ttype = Token.INVALID_TYPE;
				setCommitToPath(false);
				resetText();
				try     // for char stream error handling
				{
					try     // for lexical error handling
					{
						switch ( cached_LA1 )
						{
						case '=':
						{
							mASSIGN(true);
							theRetToken = returnToken_;
							break;
						}
						case ';':
						{
							mSEMI(true);
							theRetToken = returnToken_;
							break;
						}
						case ',':
						{
							mCOMMA(true);
							theRetToken = returnToken_;
							break;
						}
						case '\t':  case '\n':  case '\u000b':  case '\u000c':
						case '\r':  case ' ':  case '\u2028':  case '\u2029':
						{
							mWHITESPACE(true);
							theRetToken = returnToken_;
							break;
						}
						case '#':
						{
							mSINGLE_LINE_COMMENT(true);
							theRetToken = returnToken_;
							break;
						}
						case '$':  case '@':  case 'A':  case 'B':
						case 'C':  case 'D':  case 'E':  case 'F':
						case 'G':  case 'H':  case 'I':  case 'J':
						case 'K':  case 'L':  case 'M':  case 'N':
						case 'O':  case 'P':  case 'Q':  case 'R':
						case 'S':  case 'T':  case 'U':  case 'V':
						case 'W':  case 'X':  case 'Y':  case 'Z':
						case '_':  case 'a':  case 'b':  case 'c':
						case 'd':  case 'e':  case 'f':  case 'g':
						case 'h':  case 'i':  case 'j':  case 'k':
						case 'l':  case 'm':  case 'n':  case 'o':
						case 'p':  case 'q':  case 'r':  case 's':
						case 't':  case 'u':  case 'v':  case 'w':
						case 'x':  case 'y':  case 'z':
						{
							mIDENTIFIER(true);
							theRetToken = returnToken_;
							break;
						}
						case '0':  case '1':  case '2':  case '3':
						case '4':  case '5':  case '6':  case '7':
						case '8':  case '9':
						{
							mNUMERIC_LITERAL(true);
							theRetToken = returnToken_;
							break;
						}
						default:
						{
							if (cached_LA1==EOF_CHAR) { uponEOF(); returnToken_ = makeToken(Token.EOF_TYPE); }
								else				{					consume();					goto tryAgain;				}
						}
						break; }
						if ( null==returnToken_ ) goto tryAgain; // found SKIP token
						_ttype = returnToken_.Type;
						returnToken_.Type = _ttype;
						return returnToken_;
					}
					catch (RecognitionException e) {
						if (!getCommitToPath())
						{
							consume();
							goto tryAgain;
						}
							throw new TokenStreamRecognitionException(e);
					}
				}
				catch (CharStreamException cse) {
					if ( cse is CharStreamIOException ) {
						throw new TokenStreamIOException(((CharStreamIOException)cse).io);
					}
					else {
						throw new TokenStreamException(cse.Message);
					}
				}
			}
		}
		
	public void mASSIGN(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = ASSIGN;
		
		match('=');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mSEMI(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = SEMI;
		
		match(';');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mCOMMA(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = COMMA;
		
		match(',');
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	protected void mNEW_LINE(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = NEW_LINE;
		
		{
			switch ( cached_LA1 )
			{
			case '\n':
			{
				match('\u000A');
				break;
			}
			case '\u2028':
			{
				match('\u2028');
				break;
			}
			case '\u2029':
			{
				match('\u2029');
				break;
			}
			default:
				if (((cached_LA1=='\r') && (cached_LA2=='\n'))&&( LA(2)=='\u000A' ))
				{
					match('\u000D');
					match('\u000A');
				}
				else if ((cached_LA1=='\r') && (true)) {
					match('\u000D');
				}
			else
			{
				throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
			}
			break; }
		}
		newline();
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	protected void mNEW_LINE_CHARACTER(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = NEW_LINE_CHARACTER;
		
		{
			switch ( cached_LA1 )
			{
			case '\r':
			{
				match('\u000D');
				break;
			}
			case '\n':
			{
				match('\u000A');
				break;
			}
			case '\u2028':
			{
				match('\u2028');
				break;
			}
			case '\u2029':
			{
				match('\u2029');
				break;
			}
			default:
			{
				throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
			}
			 }
		}
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	protected void mNOT_NEW_LINE(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = NOT_NEW_LINE;
		
		{
			match(tokenSet_0_);
		}
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mWHITESPACE(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = WHITESPACE;
		
		{ // ( ... )+
			int _cnt39=0;
			for (;;)
			{
				switch ( cached_LA1 )
				{
				case ' ':
				{
					match(' ');
					break;
				}
				case '\t':
				{
					match('\u0009');
					break;
				}
				case '\u000b':
				{
					match('\u000B');
					break;
				}
				case '\u000c':
				{
					match('\u000C');
					break;
				}
				case '\n':  case '\r':  case '\u2028':  case '\u2029':
				{
					mNEW_LINE(false);
					break;
				}
				default:
				{
					if (_cnt39 >= 1) { goto _loop39_breakloop; } else { throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());; }
				}
				break; }
				_cnt39++;
			}
_loop39_breakloop:			;
		}    // ( ... )+
		_ttype = Token.SKIP;
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mSINGLE_LINE_COMMENT(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = SINGLE_LINE_COMMENT;
		
		match("#");
		{    // ( ... )*
			for (;;)
			{
				if ((tokenSet_0_.member(cached_LA1)))
				{
					mNOT_NEW_LINE(false);
				}
				else
				{
					goto _loop42_breakloop;
				}
				
			}
_loop42_breakloop:			;
		}    // ( ... )*
		{
			if ((tokenSet_1_.member(cached_LA1)))
			{
				mNEW_LINE(false);
			}
			else {
			}
			
		}
		_ttype = Token.SKIP;
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mIDENTIFIER(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = IDENTIFIER;
		
		mIDENTIFIER_START_CHARACTER(false);
		{    // ( ... )*
			for (;;)
			{
				if ((tokenSet_2_.member(cached_LA1)))
				{
					mIDENTIFIER_PART_CHARACTER(false);
				}
				else
				{
					goto _loop46_breakloop;
				}
				
			}
_loop46_breakloop:			;
		}    // ( ... )*
		_ttype = testLiteralsTable(_ttype);
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	protected void mIDENTIFIER_START_CHARACTER(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = IDENTIFIER_START_CHARACTER;
		
		{
			switch ( cached_LA1 )
			{
			case '@':
			{
				match('@');
				break;
			}
			case 'a':  case 'b':  case 'c':  case 'd':
			case 'e':  case 'f':  case 'g':  case 'h':
			case 'i':  case 'j':  case 'k':  case 'l':
			case 'm':  case 'n':  case 'o':  case 'p':
			case 'q':  case 'r':  case 's':  case 't':
			case 'u':  case 'v':  case 'w':  case 'x':
			case 'y':  case 'z':
			{
				matchRange('a','z');
				break;
			}
			case 'A':  case 'B':  case 'C':  case 'D':
			case 'E':  case 'F':  case 'G':  case 'H':
			case 'I':  case 'J':  case 'K':  case 'L':
			case 'M':  case 'N':  case 'O':  case 'P':
			case 'Q':  case 'R':  case 'S':  case 'T':
			case 'U':  case 'V':  case 'W':  case 'X':
			case 'Y':  case 'Z':
			{
				matchRange('A','Z');
				break;
			}
			case '_':
			{
				match('_');
				break;
			}
			case '$':
			{
				match('$');
				break;
			}
			default:
			{
				throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
			}
			 }
		}
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	protected void mIDENTIFIER_PART_CHARACTER(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = IDENTIFIER_PART_CHARACTER;
		
		{
			switch ( cached_LA1 )
			{
			case 'a':  case 'b':  case 'c':  case 'd':
			case 'e':  case 'f':  case 'g':  case 'h':
			case 'i':  case 'j':  case 'k':  case 'l':
			case 'm':  case 'n':  case 'o':  case 'p':
			case 'q':  case 'r':  case 's':  case 't':
			case 'u':  case 'v':  case 'w':  case 'x':
			case 'y':  case 'z':
			{
				matchRange('a','z');
				break;
			}
			case 'A':  case 'B':  case 'C':  case 'D':
			case 'E':  case 'F':  case 'G':  case 'H':
			case 'I':  case 'J':  case 'K':  case 'L':
			case 'M':  case 'N':  case 'O':  case 'P':
			case 'Q':  case 'R':  case 'S':  case 'T':
			case 'U':  case 'V':  case 'W':  case 'X':
			case 'Y':  case 'Z':
			{
				matchRange('A','Z');
				break;
			}
			case '_':
			{
				match('_');
				break;
			}
			case '0':  case '1':  case '2':  case '3':
			case '4':  case '5':  case '6':  case '7':
			case '8':  case '9':
			{
				matchRange('0','9');
				break;
			}
			default:
			{
				throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());
			}
			 }
		}
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	public void mNUMERIC_LITERAL(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = NUMERIC_LITERAL;
		
		{ // ( ... )+
			int _cnt53=0;
			for (;;)
			{
				if (((cached_LA1 >= '0' && cached_LA1 <= '9')))
				{
					mDECIMAL_DIGIT(false);
				}
				else
				{
					if (_cnt53 >= 1) { goto _loop53_breakloop; } else { throw new NoViableAltForCharException(cached_LA1, getFilename(), getLine(), getColumn());; }
				}
				
				_cnt53++;
			}
_loop53_breakloop:			;
		}    // ( ... )+
		
				_ttype = INTEGER_LITERAL;
			
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	protected void mDECIMAL_DIGIT(bool _createToken) //throws RecognitionException, CharStreamException, TokenStreamException
{
		int _ttype; IToken _token=null; int _begin=text.Length;
		_ttype = DECIMAL_DIGIT;
		
		{
			matchRange('0','9');
		}
		if (_createToken && (null == _token) && (_ttype != Token.SKIP))
		{
			_token = makeToken(_ttype);
			_token.setText(text.ToString(_begin, text.Length-_begin));
		}
		returnToken_ = _token;
	}
	
	
	private static long[] mk_tokenSet_0_()
	{
		long[] data = new long[1024];
		data[0]=-9224L;
		for (int i = 1; i<=127; i++) { data[i]=-1L; }
		data[128]=-3298534883329L;
		for (int i = 129; i<=511; i++) { data[i]=-1L; }
		for (int i = 512; i<=1023; i++) { data[i]=0L; }
		return data;
	}
	public static readonly BitSet tokenSet_0_ = new BitSet(mk_tokenSet_0_());
	private static long[] mk_tokenSet_1_()
	{
		long[] data = new long[513];
		data[0]=9216L;
		for (int i = 1; i<=127; i++) { data[i]=0L; }
		data[128]=3298534883328L;
		for (int i = 129; i<=512; i++) { data[i]=0L; }
		return data;
	}
	public static readonly BitSet tokenSet_1_ = new BitSet(mk_tokenSet_1_());
	private static long[] mk_tokenSet_2_()
	{
		long[] data = new long[513];
		data[0]=287948901175001088L;
		data[1]=576460745995190270L;
		for (int i = 2; i<=512; i++) { data[i]=0L; }
		return data;
	}
	public static readonly BitSet tokenSet_2_ = new BitSet(mk_tokenSet_2_());
	
}
}
