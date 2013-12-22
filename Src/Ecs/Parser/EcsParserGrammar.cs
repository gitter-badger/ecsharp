// Generated from EcsParserGrammar.les by LLLPG custom tool. LLLPG version: 0.9.3.0
// Note: you can give command-line arguments to the tool via 'Custom Tool Namespace':
// --macros=FileName.dll Load macros from FileName.dll, path relative to this file 
// --no-out-header       Suppress this message
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Loyc;
using Loyc.Syntax;
using Loyc.Syntax.Les;
using Loyc.Syntax.Lexing;
using Loyc.Collections;
using Loyc.Collections.Impl;
namespace Ecs.Parser
{
	using TT = TokenType;
	using S = CodeSymbols;
	using EP = EcsPrecedence;
	#pragma warning disable 162
	partial class EcsParser
	{
		static readonly Symbol _trait = GSymbol.Get("trait");
		static readonly Symbol _alias = GSymbol.Get("alias");
		static readonly Symbol _where = GSymbol.Get("where");
		static readonly Symbol _assembly = GSymbol.Get("assembly");
		static readonly Symbol _module = GSymbol.Get("module");
		Symbol _spaceName;
		LNode AutoRemoveParens(LNode node)
		{
			int i = node.Attrs.IndexWithName(S.TriviaInParens);
			if ((i > -1))
				 return node.WithAttrs(node.Attrs.RemoveAt(i));
			return node;
		}
		int count;
		[Flags] enum Ambiguity
		{
			AllowUnassignedVarDecl = 1, StatementLevel = 2, StopAtArgumentList = 8, ExpectCast = 4, ExpectType = 8, BlankIndexed = 64, TypeSuffix = 128, IsExpr = 256, IsCall = 512, HasAttrs = 1024, IsTuple = 2048, Error = 4096, NotAType = IsExpr | IsCall, Type = BlankIndexed | TypeSuffix
		}
		static readonly int MinPrec = Precedence.MinValue.Lo;
		public static readonly Precedence StartStmt = new Precedence(MinPrec, MinPrec, MinPrec);
		public static readonly Precedence StartExpr = new Precedence(MinPrec + 1, MinPrec + 1, MinPrec + 1);
		public static readonly Precedence ContinueExpr = new Precedence(MinPrec + 2, MinPrec + 2, MinPrec + 2);
		RWList<LNode> _attrs = new RWList<LNode>();
		bool Is(int li, Symbol value)
		{
			return LT(li).Value == value;
		}
		LNode DataType(bool allowDimensions = false)
		{
			var e = ComplexId();
			TypeSuffixOpt(allowDimensions, ref e);
			return e;
		}
		bool Try_Scan_DataType(int lookaheadAmt, bool allowDimensions = false)
		{
			using (new SavePosition(this, lookaheadAmt))
				return Scan_DataType(allowDimensions);
		}
		bool Scan_DataType(bool allowDimensions = false)
		{
			if (!Scan_ComplexId())
				return false;
			if (!Scan_TypeSuffixOpt(allowDimensions))
				return false;
			return true;
		}
		LNode ComplexId()
		{
			TokenType la0, la1;
			var e = IdAtom();
			la0 = LA0;
			if (la0 == TT.ColonColon) {
				la1 = LA(1);
				if (la1 == TT.Id || la1 == TT.Substitute || la1 == TT.TypeKeyword) {
					Skip();
					var e2 = IdAtom();
					e = F.Call(S.ColonColon, e, e2, e.Range.StartIndex, e2.Range.EndIndex);
				}
			}
			RestOfId(ref e);
			return e;
		}
		bool Scan_ComplexId()
		{
			TokenType la0, la1;
			if (!Scan_IdAtom())
				return false;
			la0 = LA0;
			if (la0 == TT.ColonColon) {
				la1 = LA(1);
				if (la1 == TT.Id || la1 == TT.Substitute || la1 == TT.TypeKeyword) {
					if (!TryMatch((int) TT.ColonColon))
						return false;
					if (!Scan_IdAtom())
						return false;
				}
			}
			if (!Scan_RestOfId())
				return false;
			return true;
		}
		LNode IdAtom()
		{
			TokenType la0;
			LNode r;
			la0 = LA0;
			if (la0 == TT.Substitute) {
				var t = MatchAny();
				var e = Atom();
				e = AutoRemoveParens(e);
				r = F.Call(S.Substitute, e, t.StartIndex, e.Range.EndIndex);
			} else {
				var t = Match((int) TT.Id, (int) TT.TypeKeyword);
				r = F.Id((Symbol) t.Value, t.StartIndex, t.EndIndex);
			}
			return r;
		}
		bool Scan_IdAtom()
		{
			TokenType la0;
			la0 = LA0;
			if (la0 == TT.Substitute) {
				if (!TryMatch((int) TT.Substitute))
					return false;
				if (!Scan_Atom())
					return false;
			} else if (!TryMatch((int) TT.Id, (int) TT.TypeKeyword))
				return false;
			return true;
		}
		void RestOfId(ref LNode r)
		{
			TokenType la0, la1, la2;
			la0 = LA0;
			if (la0 == TT.LT) {
				switch (LA(1)) {
				case TT.Substitute:
					{
						switch (LA(2)) {
						case TT.@base:
						case TT.@default:
						case TT.@this:
						case TT.@checked:
						case TT.@delegate:
						case TT.@new:
						case TT.@sizeof:
						case TT.@typeof:
						case TT.@unchecked:
						case TT.At:
						case TT.Dot:
						case TT.Id:
						case TT.LBrace:
						case TT.LParen:
						case TT.Number:
						case TT.OtherLit:
						case TT.SQString:
						case TT.String:
						case TT.Substitute:
						case TT.Symbol:
						case TT.TypeKeyword:
							TParams(ref r);
							break;
						}
					}
					break;
				case TT.Id:
				case TT.TypeKeyword:
					{
						switch (LA(2)) {
						case TT.ColonColon:
						case TT.Comma:
						case TT.Dot:
						case TT.GT:
						case TT.LT:
						case TT.Not:
							TParams(ref r);
							break;
						}
					}
					break;
				case TT.GT:
					TParams(ref r);
					break;
				}
			} else if (la0 == TT.Dot) {
				la1 = LA(1);
				if (la1 == TT.LBrack) {
					la2 = LA(2);
					if (la2 == TT.RBrack)
						TParams(ref r);
				}
			} else if (la0 == TT.Not) {
				la1 = LA(1);
				if (la1 == TT.LParen) {
					la2 = LA(2);
					if (la2 == TT.RParen)
						TParams(ref r);
				}
			}
			la0 = LA0;
			if (la0 == TT.Dot) {
				la1 = LA(1);
				if (la1 == TT.Substitute) {
					switch (LA(2)) {
					case TT.@base:
					case TT.@default:
					case TT.@this:
					case TT.@checked:
					case TT.@delegate:
					case TT.@new:
					case TT.@sizeof:
					case TT.@typeof:
					case TT.@unchecked:
					case TT.At:
					case TT.Dot:
					case TT.Id:
					case TT.LBrace:
					case TT.LParen:
					case TT.Number:
					case TT.OtherLit:
					case TT.SQString:
					case TT.String:
					case TT.Substitute:
					case TT.Symbol:
					case TT.TypeKeyword:
						DotRestOfId(ref r);
						break;
					}
				} else if (la1 == TT.Id || la1 == TT.TypeKeyword)
					DotRestOfId(ref r);
			}
		}
		bool Try_Scan_RestOfId(int lookaheadAmt)
		{
			using (new SavePosition(this, lookaheadAmt))
				return Scan_RestOfId();
		}
		bool Scan_RestOfId()
		{
			TokenType la0, la1, la2;
			do {
				la0 = LA0;
				if (la0 == TT.LT) {
					switch (LA(1)) {
					case TT.Substitute:
						{
							switch (LA(2)) {
							case TT.@base:
							case TT.@default:
							case TT.@this:
							case TT.@checked:
							case TT.@delegate:
							case TT.@new:
							case TT.@sizeof:
							case TT.@typeof:
							case TT.@unchecked:
							case TT.At:
							case TT.Dot:
							case TT.Id:
							case TT.LBrace:
							case TT.LParen:
							case TT.Number:
							case TT.OtherLit:
							case TT.SQString:
							case TT.String:
							case TT.Substitute:
							case TT.Symbol:
							case TT.TypeKeyword:
								goto match1;
							}
						}
						break;
					case TT.Id:
					case TT.TypeKeyword:
						{
							switch (LA(2)) {
							case TT.ColonColon:
							case TT.Comma:
							case TT.Dot:
							case TT.GT:
							case TT.LT:
							case TT.Not:
								goto match1;
							}
						}
						break;
					case TT.GT:
						goto match1;
					}
				} else if (la0 == TT.Dot) {
					la1 = LA(1);
					if (la1 == TT.LBrack) {
						la2 = LA(2);
						if (la2 == TT.RBrack)
							goto match1;
					}
				} else if (la0 == TT.Not) {
					la1 = LA(1);
					if (la1 == TT.LParen) {
						la2 = LA(2);
						if (la2 == TT.RParen)
							goto match1;
					}
				}
				break;
			match1:
				{
					if (!Scan_TParams())
						return false;
				}
			} while (false);
			do {
				la0 = LA0;
				if (la0 == TT.Dot) {
					la1 = LA(1);
					if (la1 == TT.Substitute) {
						switch (LA(2)) {
						case TT.@base:
						case TT.@default:
						case TT.@this:
						case TT.@checked:
						case TT.@delegate:
						case TT.@new:
						case TT.@sizeof:
						case TT.@typeof:
						case TT.@unchecked:
						case TT.At:
						case TT.Dot:
						case TT.Id:
						case TT.LBrace:
						case TT.LParen:
						case TT.Number:
						case TT.OtherLit:
						case TT.SQString:
						case TT.String:
						case TT.Substitute:
						case TT.Symbol:
						case TT.TypeKeyword:
							goto match1b;
						}
					} else if (la1 == TT.Id || la1 == TT.TypeKeyword)
						goto match1b;
				}
				break;
			match1b:
				{
					if (!Scan_DotRestOfId())
						return false;
				}
			} while (false);
			return true;
		}
		void DotRestOfId(ref LNode r)
		{
			Match((int) TT.Dot);
			var e = IdAtom();
			r = F.Dot(r, e);
			RestOfId(ref r);
		}
		bool Try_Scan_DotRestOfId(int lookaheadAmt)
		{
			using (new SavePosition(this, lookaheadAmt))
				return Scan_DotRestOfId();
		}
		bool Scan_DotRestOfId()
		{
			if (!TryMatch((int) TT.Dot))
				return false;
			if (!Scan_IdAtom())
				return false;
			if (!Scan_RestOfId())
				return false;
			return true;
		}
		void TParams(ref LNode r)
		{
			TokenType la0;
			RWList<LNode> list = new RWList<LNode>();
			Token end;
			la0 = LA0;
			if (la0 == TT.LT) {
				Skip();
				la0 = LA0;
				if (la0 == TT.Id || la0 == TT.Substitute || la0 == TT.TypeKeyword) {
					list.Add(ComplexId());
					for (;;) {
						la0 = LA0;
						if (la0 == TT.Comma) {
							Skip();
							list.Add(ComplexId());
						} else
							break;
					}
				}
				end = Match((int) TT.GT);
			} else if (la0 == TT.Dot) {
				Skip();
				var t = Match((int) TT.LBrack);
				end = Match((int) TT.RBrack);
				list = AppendExprsInside(t, list);
			} else {
				Match((int) TT.Not);
				var t = Match((int) TT.LParen);
				end = Match((int) TT.RParen);
				list = AppendExprsInside(t, list);
			}
			list.Insert(0, r);
			int start = r.Range.StartIndex;
			r = F.Call(S.Of, list.ToRVList(), start, end.EndIndex);
		}
		bool Try_Scan_TParams(int lookaheadAmt)
		{
			using (new SavePosition(this, lookaheadAmt))
				return Scan_TParams();
		}
		bool Scan_TParams()
		{
			TokenType la0;
			la0 = LA0;
			if (la0 == TT.LT) {
				if (!TryMatch((int) TT.LT))
					return false;
				la0 = LA0;
				if (la0 == TT.Id || la0 == TT.Substitute || la0 == TT.TypeKeyword) {
					if (!Scan_ComplexId())
						return false;
					for (;;) {
						la0 = LA0;
						if (la0 == TT.Comma) {
							if (!TryMatch((int) TT.Comma))
								return false;
							if (!Scan_ComplexId())
								return false;
						} else
							break;
					}
				}
				if (!TryMatch((int) TT.GT))
					return false;
			} else if (la0 == TT.Dot) {
				if (!TryMatch((int) TT.Dot))
					return false;
				if (!TryMatch((int) TT.LBrack))
					return false;
				if (!TryMatch((int) TT.RBrack))
					return false;
			} else {
				if (!TryMatch((int) TT.Not))
					return false;
				if (!TryMatch((int) TT.LParen))
					return false;
				if (!TryMatch((int) TT.RParen))
					return false;
			}
			return true;
		}
		bool TypeSuffixOpt(bool allowDimensions, ref LNode e)
		{
			TokenType la0, la1;
			int count;
			bool result = false;
			for (;;) {
				la0 = LA0;
				if (la0 == TT.QuestionMark) {
					Skip();
					e = F.Of(F.Id(S.QuestionMark), e);
					result = true;
				} else if (la0 == TT.Mul) {
					Skip();
					e = F.Of(F.Id(S._Pointer), e);
					result = true;
				} else if (la0 == TT.LBrack) {
					var dims = InternalList<int>.Empty;
					Check((count = CountDims(LT(0), allowDimensions)) > 0, "(count = CountDims(LT($LI), allowDimensions)) > 0");
					Skip();
					Match((int) TT.RBrack);
					dims.Add(count);
					for (;;) {
						la0 = LA0;
						if (la0 == TT.LBrack) {
							la1 = LA(1);
							if (la1 == TT.RBrack) {
								Check((count = CountDims(LT(0), allowDimensions)) > 0, "(count = CountDims(LT($LI), allowDimensions)) > 0");
								Skip();
								Skip();
								dims.Add(count);
							} else
								break;
						} else
							break;
					}
					for (int i = dims.Count - 1; i >= 0; i--)
						e = F.Of(F.Id(S.GetArrayKeyword(dims[i])), e);
					result = true;
				} else
					break;
			}
			return result;
		}
		bool Try_Scan_TypeSuffixOpt(int lookaheadAmt, bool allowDimensions)
		{
			using (new SavePosition(this, lookaheadAmt))
				return Scan_TypeSuffixOpt(allowDimensions);
		}
		bool Scan_TypeSuffixOpt(bool allowDimensions)
		{
			TokenType la0, la1;
			for (;;) {
				la0 = LA0;
				if (la0 == TT.QuestionMark)
					{if (!TryMatch((int) TT.QuestionMark))
						return false;}
				else if (la0 == TT.Mul)
				{
					if (!TryMatch((int)TT.Mul))
						return false;
				}
				else if (la0 == TT.LBrack)
				{
					if (!((count = CountDims(LT(0), allowDimensions)) > 0))
						return false;
					if (!TryMatch((int)TT.LBrack))
						return false;
					if (!TryMatch((int)TT.RBrack))
						return false;
					for (; ; )
					{
						la0 = LA0;
						if (la0 == TT.LBrack)
						{
							la1 = LA(1);
							if (la1 == TT.RBrack)
							{
								if (!((count = CountDims(LT(0), allowDimensions)) > 0))
									return false;
								if (!TryMatch((int)TT.LBrack))
									return false;
								if (!TryMatch((int)TT.RBrack))
									return false;
							}
							else
								break;
						}
						else
							break;
					}
				}
				else
					break;
			}
			return true;
		}
		LNode Atom()
		{
			TokenType la0, la1;
			LNode r;
			switch (LA0) {
			case TT.Dot:
			case TT.Substitute:
				{
					var t = MatchAny();
					var e = Atom();
					e = AutoRemoveParens(e);
					r = F.Call((Symbol) t.Value, e, t.StartIndex, e.Range.EndIndex);
				}
				break;
			case TT.@base:
			case TT.@this:
			case TT.Id:
			case TT.TypeKeyword:
				{
					var t = MatchAny();
					r = F.Id((Symbol) t.Value, t.StartIndex, t.EndIndex);
				}
				break;
			case TT.Number:
			case TT.OtherLit:
			case TT.SQString:
			case TT.String:
			case TT.Symbol:
				{
					var t = MatchAny();
					r = F.Literal(t.Value, t.StartIndex, t.EndIndex);
				}
				break;
			case TT.LParen:
				r = ExprInParens();
				break;
			case TT.LBrace:
				r = BracedBlock();
				break;
			case TT.At:
				{
					var at = MatchAny();
					var t = Match((int) TT.LBrack);
					var rb = Match((int) TT.RBrack);
					r = F.Literal(t.Children, at.StartIndex, rb.EndIndex);
					r = F.Literal(t.Children, t.StartIndex, rb.EndIndex);
				}
				break;
			case TT.@new:
				{
					LNode type = F._Missing;
					Skip();
					switch (LA0) {
					case TT.LBrack:
						{
							Check(LT(0).Count == 0, "LT($LI).Count == 0");
							var lb = MatchAny();
							var rb = Match((int) TT.RBrack);
							type = F.Id(S.Bracks, lb.StartIndex, rb.EndIndex);
						}
						break;
					case TT.Id:
					case TT.Substitute:
					case TT.TypeKeyword:
						type = DataType(true);
						break;
					}
					var list = new RWList<LNode>();
					la0 = LA0;
					if (la0 == TT.LParen) {
						var lp = MatchAny();
						Match((int) TT.RParen);
						list.Add(type.WithArgs(ExprListInside(lp).ToRVList()));
						la0 = LA0;
						if (la0 == TT.LBrace) {
							la1 = LA(1);
							if (la1 == TT.RBrace) {
								var lb = MatchAny();
								Skip();
								AppendExprsInside(lb, list);
							}
						}
					} else {
						list.Add(type.WithArgs());
						var lb = Match((int) TT.LBrace);
						Match((int) TT.RBrace);
						AppendStmtsInside(lb, list);
					}
					r = F.Call(S.New, list.ToRVList());
				}
				break;
			case TT.@default:
			case TT.@checked:
			case TT.@sizeof:
			case TT.@typeof:
			case TT.@unchecked:
				{
					var t = MatchAny();
					var args = Match((int) TT.LParen);
					var rp = Match((int) TT.RParen);
					r = F.Call((Symbol) t.Value, ExprListInside(args), t.StartIndex, rp.EndIndex);
				}
				break;
			case TT.@delegate:
				{
					var t = MatchAny();
					var args = Match((int) TT.LParen);
					Match((int) TT.RParen);
					var block = Match((int) TT.LBrace);
					var rb = Match((int) TT.RBrace);
					r = F.Call(S.Lambda, F.Tuple(ExprListInside(args).ToRVList()), F.Braces(StmtListInside(block).ToRVList(), block.StartIndex, rb.EndIndex), t.StartIndex, rb.EndIndex);
				}
				break;
			default:
				{
					r = Error("Invalid expression");
					for (;;) {
						la0 = LA0;
						if (!(la0 == EOF || la0 == TT.Comma || la0 == TT.Semicolon))
							Skip();
						else
							break;
					}
				}
				break;
			}
			return r;
		}
		static readonly HashSet<int> Scan_Atom_set0 = NewSet((int) TT.Number, (int) TT.OtherLit, (int) TT.SQString, (int) TT.String, (int) TT.Symbol);
		static readonly HashSet<int> Scan_Atom_set1 = NewSet((int) TT.@default, (int) TT.@checked, (int) TT.@sizeof, (int) TT.@typeof, (int) TT.@unchecked);
		bool Scan_Atom()
		{
			TokenType la0, la1;
			switch (LA0) {
			case TT.Dot:
			case TT.Substitute:
				{
					if (!TryMatch((int) TT.Dot, (int) TT.Substitute))
						return false;
					if (!Scan_Atom())
						return false;
				}
				break;
			case TT.@base:
			case TT.@this:
			case TT.Id:
			case TT.TypeKeyword:
				if (!TryMatch((int) TT.@base, (int) TT.@this, (int) TT.Id, (int) TT.TypeKeyword))
					return false;
				break;
			case TT.Number:
			case TT.OtherLit:
			case TT.SQString:
			case TT.String:
			case TT.Symbol:
				if (!TryMatch(Scan_Atom_set0))
					return false;
				break;
			case TT.LParen:
				if (!Scan_ExprInParens())
					return false;
				break;
			case TT.LBrace:
				if (!Scan_BracedBlock())
					return false;
				break;
			case TT.At:
				{
					if (!TryMatch((int) TT.At))
						return false;
					if (!TryMatch((int) TT.LBrack))
						return false;
					if (!TryMatch((int) TT.RBrack))
						return false;
				}
				break;
			case TT.@new:
				{
					if (!TryMatch((int) TT.@new))
						return false;
					switch (LA0) {
					case TT.LBrack:
						{
							if (!(LT(0).Count == 0))
								return false;
							if (!TryMatch((int) TT.LBrack))
								return false;
							if (!TryMatch((int) TT.RBrack))
								return false;
						}
						break;
					case TT.Id:
					case TT.Substitute:
					case TT.TypeKeyword:
						if (!Scan_DataType(true))
							return false;
						break;
					}
					la0 = LA0;
					if (la0 == TT.LParen) {
						if (!TryMatch((int) TT.LParen))
							return false;
						if (!TryMatch((int) TT.RParen))
							return false;
						la0 = LA0;
						if (la0 == TT.LBrace) {
							la1 = LA(1);
							if (la1 == TT.RBrace) {
								if (!TryMatch((int) TT.LBrace))
									return false;
								if (!TryMatch((int) TT.RBrace))
									return false;
							}
						}
					} else {
						if (!TryMatch((int) TT.LBrace))
							return false;
						if (!TryMatch((int) TT.RBrace))
							return false;
					}
				}
				break;
			case TT.@default:
			case TT.@checked:
			case TT.@sizeof:
			case TT.@typeof:
			case TT.@unchecked:
				{
					if (!TryMatch(Scan_Atom_set1))
						return false;
					if (!TryMatch((int) TT.LParen))
						return false;
					if (!TryMatch((int) TT.RParen))
						return false;
				}
				break;
			case TT.@delegate:
				{
					if (!TryMatch((int) TT.@delegate))
						return false;
					if (!TryMatch((int) TT.LParen))
						return false;
					if (!TryMatch((int) TT.RParen))
						return false;
					if (!TryMatch((int) TT.LBrace))
						return false;
					if (!TryMatch((int) TT.RBrace))
						return false;
				}
				break;
			default:
				{
					for (;;) {
						la0 = LA0;
						if (!(la0 == EOF || la0 == TT.Comma || la0 == TT.Semicolon))
							{if (!TryMatchExcept((int) EOF, (int) TT.Comma, (int) TT.Semicolon))
								return false;}
						else
							break;
					}
				}
				break;
			}
			return true;
		}
		static readonly HashSet<int> PrimaryExpr_set0 = NewSet((int) EOF, (int) TT.@as, (int) TT.@in, (int) TT.@is, (int) TT.@new, (int) TT.@using, (int) TT.Add, (int) TT.And, (int) TT.AndBits, (int) TT.AttrKeyword, (int) TT.BQString, (int) TT.Colon, (int) TT.ColonColon, (int) TT.Comma, (int) TT.CompoundSet, (int) TT.DivMod, (int) TT.Dot, (int) TT.DotDot, (int) TT.EqNeq, (int) TT.GT, (int) TT.Id, (int) TT.IncDec, (int) TT.LambdaArrow, (int) TT.LBrack, (int) TT.LEGE, (int) TT.LParen, (int) TT.LT, (int) TT.Mul, (int) TT.NotBits, (int) TT.NullCoalesce, (int) TT.NullDot, (int) TT.OrBits, (int) TT.OrXor, (int) TT.Power, (int) TT.PtrArrow, (int) TT.QuestionMark, (int) TT.QuickBind, (int) TT.Set, (int) TT.Sub, (int) TT.XorBits);
		LNode PrimaryExpr()
		{
			TokenType la1;
			var e = Atom();
			for (;;) {
				switch (LA0) {
				case TT.ColonColon:
				case TT.Dot:
				case TT.PtrArrow:
				case TT.QuickBind:
					{
						var op = MatchAny();
						var rhs = Atom();
						e = F.Call((Symbol) op.Value, e, rhs, e.Range.StartIndex, rhs.Range.EndIndex);
					}
					break;
				case TT.LParen:
					{
						if (Down(0) && Up(LA0 == TT.@as || LA0 == TT.@using || LA0 == TT.PtrArrow))
							e = PrimaryExpr_NewStyleCast(e);
						else {
							var lp = MatchAny();
							var rp = Match((int) TT.RParen);
							e = F.Call(e, ExprListInside(lp), e.Range.StartIndex, rp.EndIndex);
						}
					}
					break;
				case TT.LBrack:
					{
						la1 = LA(1);
						if (la1 == TT.RBrack) {
							var lb = MatchAny();
							var rb = MatchAny();
							var list = new RWList<LNode> { 
								e
							};
							e = F.Call(S.Bracks, AppendExprsInside(lb, list).ToRVList(), e.Range.StartIndex, rb.EndIndex);
						} else
							goto stop;
					}
					break;
				case TT.IncDec:
					{
						var t = MatchAny();
						e = F.Call(t.Value == S.PreInc ? S.PostInc : S.PostDec, e, e.Range.StartIndex, t.EndIndex);
					}
					break;
				case TT.LT:
					{
						if (Try_PrimaryExpr_Test0(0)) {
							la1 = LA(1);
							if (PrimaryExpr_set0.Contains((int) la1))
								TParams(ref e);
							else
								goto stop;
						} else
							goto stop;
					}
					break;
				default:
					goto stop;
				}
			}
		 stop:;
			return e;
		}
		LNode PrimaryExpr_NewStyleCast(LNode e)
		{
			var lp = MatchAny();
			var rp = Match((int) TT.RParen);
			Down(lp);
			var kind = Match((int) TT.@as, (int) TT.@using, (int) TT.PtrArrow);
			var type = ExprStart();
			Match((int) EOF);
			return Up(F.Call(kind.Value == S.PtrArrow ? S.Cast : ((Symbol) kind.Value), e, type, e.Range.StartIndex, rp.EndIndex));
		}
		LNode NullDotExpr()
		{
			TokenType la0;
			var e = PrimaryExpr();
			for (;;) {
				la0 = LA0;
				if (la0 == TT.NullDot) {
					Skip();
					var rhs = PrimaryExpr();
					e = F.Call(S.NullDot, e, rhs, e.Range.StartIndex, rhs.Range.EndIndex);
				} else
					break;
			}
			return e;
		}
		LNode PrefixExpr()
		{
			do {
				switch (LA0) {
				case TT.Add:
				case TT.AndBits:
				case TT.IncDec:
				case TT.Mul:
				case TT.Not:
				case TT.NotBits:
				case TT.Sub:
					{
						var op = MatchAny();
						var e = PrefixExpr();
						return F.Call((Symbol) op.Value, e, op.StartIndex, e.Range.EndIndex);
					}
					break;
				case TT.LParen:
					{
						if (Down(0) && Up(Scan_DataType() && LA0 == EOF)) {
							switch (LA(2)) {
							case TT.@new:
							case TT.Add:
							case TT.AndBits:
							case TT.Dot:
							case TT.Id:
							case TT.IncDec:
							case TT.LParen:
							case TT.Mul:
							case TT.NotBits:
							case TT.Power:
							case TT.Sub:
								{
									if (!Try_PrefixExpr_Test0(2))
										goto match2;
									else
										goto match4;
								}
							case TT.@base:
							case TT.@default:
							case TT.@this:
							case TT.@checked:
							case TT.@delegate:
							case TT.@sizeof:
							case TT.@typeof:
							case TT.@unchecked:
							case TT.At:
							case TT.LBrace:
							case TT.Not:
							case TT.Number:
							case TT.OtherLit:
							case TT.SQString:
							case TT.String:
							case TT.Substitute:
							case TT.Symbol:
							case TT.TypeKeyword:
								goto match2;
							default:
								goto match4;
							}
						} else
							goto match4;
					}
				case TT.Power:
					{
						var op = MatchAny();
						var e = PrefixExpr();
						return F.Call(S._Dereference, F.Call(S._Dereference, e, op.StartIndex + 1, e.Range.EndIndex), op.StartIndex, e.Range.EndIndex);
					}
					break;
				default:
					goto match4;
				}
				break;
			match2:
				{
					var lp = MatchAny();
					Match((int) TT.RParen);
					Check(!Try_PrefixExpr_Test0(0), "!(((TT.Sub|TT.Add) | TT.IncDec TT.LParen))");
					var e = PrefixExpr();
					Down(lp);
					return F.Call(S.Cast, e, Up(DataType()), lp.StartIndex, e.Range.EndIndex);
				}
				break;
			match4:
				{
					var e = NullDotExpr();
					return e;
				}
			} while (false);
		}
		static readonly HashSet<int> Expr_set0 = NewSet((int) TT.@base, (int) TT.@default, (int) TT.@this, (int) TT.@checked, (int) TT.@delegate, (int) TT.@new, (int) TT.@sizeof, (int) TT.@typeof, (int) TT.@unchecked, (int) TT.Add, (int) TT.AndBits, (int) TT.At, (int) TT.Dot, (int) TT.Id, (int) TT.IncDec, (int) TT.LBrace, (int) TT.LParen, (int) TT.Mul, (int) TT.Not, (int) TT.NotBits, (int) TT.Number, (int) TT.OtherLit, (int) TT.Power, (int) TT.SQString, (int) TT.String, (int) TT.Sub, (int) TT.Substitute, (int) TT.Symbol, (int) TT.TypeKeyword);
		static readonly HashSet<int> Expr_set1 = NewSet((int) TT.@base, (int) TT.@default, (int) TT.@this, (int) TT.@as, (int) TT.@checked, (int) TT.@delegate, (int) TT.@in, (int) TT.@is, (int) TT.@new, (int) TT.@sizeof, (int) TT.@typeof, (int) TT.@unchecked, (int) TT.@using, (int) TT.Add, (int) TT.And, (int) TT.AndBits, (int) TT.At, (int) TT.BQString, (int) TT.Colon, (int) TT.ColonColon, (int) TT.CompoundSet, (int) TT.DivMod, (int) TT.Dot, (int) TT.DotDot, (int) TT.EqNeq, (int) TT.GT, (int) TT.Id, (int) TT.IncDec, (int) TT.LambdaArrow, (int) TT.LBrace, (int) TT.LBrack, (int) TT.LEGE, (int) TT.LParen, (int) TT.LT, (int) TT.Mul, (int) TT.Not, (int) TT.NotBits, (int) TT.NullCoalesce, (int) TT.NullDot, (int) TT.Number, (int) TT.OrBits, (int) TT.OrXor, (int) TT.OtherLit, (int) TT.Power, (int) TT.PtrArrow, (int) TT.QuestionMark, (int) TT.QuickBind, (int) TT.RBrace, (int) TT.RParen, (int) TT.Set, (int) TT.SQString, (int) TT.String, (int) TT.Sub, (int) TT.Substitute, (int) TT.Symbol, (int) TT.TypeKeyword, (int) TT.XorBits);
		LNode Expr(Precedence context)
		{
			TokenType la0, la1, la2;
			Debug.Assert(context.CanParse(EP.Prefix));
			Precedence prec;
			var e = PrefixExpr();
			for (;;) {
				switch (LA0) {
				case TT.GT:
				case TT.LT:
					{
						la0 = LA0;
						if (context.CanParse(prec = InfixPrecedenceOf(la0))) {
							if (context.CanParse(EP.Shift) && LT(0).EndIndex == LT(0 + 1).StartIndex) {
								la1 = LA(1);
								if (Expr_set0.Contains((int) la1))
									goto match1;
								else if (la1 == TT.GT || la1 == TT.LT) {
									la2 = LA(2);
									if (Expr_set0.Contains((int) la2))
										goto match3;
									else
										goto stop;
								} else
									goto stop;
							} else {
								la1 = LA(1);
								if (Expr_set0.Contains((int) la1))
									goto match1;
								else
									goto stop;
							}
						} else if (context.CanParse(EP.Shift) && LT(0).EndIndex == LT(0 + 1).StartIndex) {
							la1 = LA(1);
							if (la1 == TT.GT || la1 == TT.LT) {
								la2 = LA(2);
								if (Expr_set0.Contains((int) la2))
									goto match3;
								else
									goto stop;
							} else
								goto stop;
						} else
							goto stop;
					}
				case TT.@as:
				case TT.@in:
				case TT.@is:
				case TT.Add:
				case TT.And:
				case TT.AndBits:
				case TT.BQString:
				case TT.CompoundSet:
				case TT.DivMod:
				case TT.DotDot:
				case TT.EqNeq:
				case TT.LambdaArrow:
				case TT.LEGE:
				case TT.Mul:
				case TT.NotBits:
				case TT.NullCoalesce:
				case TT.OrBits:
				case TT.OrXor:
				case TT.Power:
				case TT.Set:
				case TT.Sub:
				case TT.XorBits:
					{
						la0 = LA0;
						if (context.CanParse(prec = InfixPrecedenceOf(la0))) {
							la1 = LA(1);
							if (Expr_set0.Contains((int) la1))
								goto match1;
							else
								goto stop;
						} else
							goto stop;
					}
				case TT.@using:
					{
						la0 = LA0;
						if (context.CanParse(prec = InfixPrecedenceOf(la0))) {
							la1 = LA(1);
							if (Expr_set0.Contains((int) la1)) {
								var op = MatchAny();
								var rhs = Expr(prec);
								e = F.Call(S.UsingCast, e, rhs, e.Range.StartIndex, rhs.Range.EndIndex);
								e.BaseStyle = NodeStyle.Operator;
							} else
								goto stop;
						} else
							goto stop;
					}
					break;
				case TT.QuestionMark:
					{
						if (context.CanParse(EP.IfElse)) {
							la1 = LA(1);
							if (Expr_set0.Contains((int) la1)) {
								la2 = LA(2);
								if (Expr_set1.Contains((int) la2)) {
									Skip();
									var then = Expr(ContinueExpr);
									Match((int) TT.Colon);
									var @else = Expr(EP.IfElse);
									e = F.Call(S.QuestionMark, e, then, @else, e.Range.StartIndex, @else.Range.EndIndex);
									e.BaseStyle = NodeStyle.Operator;
								} else
									goto stop;
							} else
								goto stop;
						} else
							goto stop;
					}
					break;
				default:
					goto stop;
				}
				continue;
			match1:
				{
					var op = MatchAny();
					var rhs = Expr(prec);
					e = F.Call((Symbol) op.Value, e, rhs, e.Range.StartIndex, rhs.Range.EndIndex);
					e.BaseStyle = NodeStyle.Operator;
				}
				continue;
			match3:
				{
					la0 = LA0;
					if (la0 == TT.LT) {
						Skip();
						Match((int) TT.LT);
						var rhs = Expr(EP.Shift);
						e = F.Call(S.Shl, e, rhs, e.Range.StartIndex, rhs.Range.EndIndex);
					} else {
						Match((int) TT.GT);
						Match((int) TT.GT);
						var rhs = Expr(EP.Shift);
						e = F.Call(S.Shr, e, rhs, e.Range.StartIndex, rhs.Range.EndIndex);
					}
					e.BaseStyle = NodeStyle.Operator;
				}
			}
		 stop:;
			return e;
		}
		LNode ExprStart()
		{
			var e = Expr(ContinueExpr);
			return e;
		}
		void NormalAttributes()
		{
			TokenType la0;
			_attrs.Clear();
			for (;;) {
				la0 = LA0;
				if (la0 == TT.LBrack) {
					var t = MatchAny();
					Match((int) TT.RBrack);
					AppendExprsInside(t, _attrs);
				} else
					break;
			}
			for (;;) {
				la0 = LA0;
				if (la0 == TT.AttrKeyword) {
					switch (LA(1)) {
					case EOF:
					case TT.@new:
					case TT.AttrKeyword:
					case TT.Id:
						{
							var t = MatchAny();
							_attrs.Add(F.Id((Symbol) t.Value, t.StartIndex, t.EndIndex));
						}
						break;
					default:
						goto stop;
					}
				} else
					goto stop;
			}
		 stop:;
		}
		int WordAttributes()
		{
			TokenType la0;
			int count = 0;
			la0 = LA0;
			if (la0 == TT.AttrKeyword) {
				var t = MatchAny();
				_attrs.Add(F.Id((Symbol) t.Value, t.StartIndex, t.EndIndex));
			} else {
				Check(!!LT(0).Value.ToString().StartsWith("#"), "!(!LT($LI).Value.ToString().StartsWith(\"#\"))");
				var t = Match((int) TT.@new, (int) TT.Id);
				Check(Try_WordAttributes_Test0(0), "(DataType TT.Id | &{_spaceName != S.Def} #.(TT, noMacro(@this)) | TT.@checked TT.LBrace TT.RBrace | TT.@unchecked TT.LBrace TT.RBrace | #.(TT, noMacro(@default)) TT.Colon | (TT.@enum|TT.@case|TT.@goto|#.(TT, noMacro(@break))|TT.@using|TT.@try|TT.@delegate|TT.@for|TT.@foreach|TT.@while|TT.@do|#.(TT, noMacro(@throw))|TT.@interface|#.(TT, noMacro(@return))|TT.@struct|TT.@namespace|TT.@lock|#.(TT, noMacro(@continue))|TT.@switch|TT.@event|TT.@class|TT.@fixed))");
				count++;
				_attrs.Add(F.Id("#" + t.Value.ToString(), t.StartIndex, t.EndIndex));
			}
			return count;
		}
		public LNode Stmt()
		{
			TokenType la0;
			NormalAttributes();
			WordAttributes();
			var attrs = _attrs.ToRVList();
			LNode r;
			switch (LA0) {
			case TT.@class:
			case TT.@enum:
			case TT.@interface:
			case TT.@namespace:
			case TT.@struct:
				{
					r = SpaceDecl(attrs);
					r = ExprStart();
					r = r.WithAttrs(attrs);
				}
				break;
			case TT.Semicolon:
				{
					var t = MatchAny();
					r = F.Id(S.Missing, t.StartIndex, t.EndIndex);
				}
				break;
			default:
				{
					r = Error("Syntax error: statement expected");
					for (;;) {
						la0 = LA0;
						if (!(la0 == EOF || la0 == TT.Semicolon))
							Skip();
						else
							break;
					}
					la0 = LA0;
					if (la0 == TT.Semicolon)
						Skip();
				}
				break;
			}
			return r;
		}
		void AsmOrModLabel()
		{
			Check(LT(0).Value == _assembly || LT(0).Value == _module, "LT($LI).Value == _assembly || LT($LI).Value == _module");
			Match((int) TT.Id);
			Match((int) TT.Colon);
		}
		bool Try_Scan_AsmOrModLabel(int lookaheadAmt)
		{
			using (new SavePosition(this, lookaheadAmt))
				return Scan_AsmOrModLabel();
		}
		bool Scan_AsmOrModLabel()
		{
			if (!(LT(0).Value == _assembly || LT(0).Value == _module))
				return false;
			if (!TryMatch((int) TT.Id))
				return false;
			if (!TryMatch((int) TT.Colon))
				return false;
			return true;
		}
		RWList<LNode> AssemblyOrModuleAttribute()
		{
			Check(Down(0) && Up(Try_Scan_AsmOrModLabel(0)), "Down($LI) && Up(Try_Scan_AsmOrModLabel(0))");
			var t = Match((int) TT.LBrack);
			Match((int) TT.RBrack);
			Down(t);
			AsmOrModLabel();
			var L = new RWList<LNode>();
			ExprList(L);
			return Up(L);
		}
		LNode ExprInParens()
		{
			var lp = MatchAny();
			var rp = Match((int) TT.RParen);
			if ((!Down(lp))) {
				return F.Call(S.Tuple, lp.StartIndex, rp.EndIndex);
			}
			return Up(ExprInParensOrTuple(lp.StartIndex, rp.EndIndex));
		}
		bool Scan_ExprInParens()
		{
			if (!TryMatch((int) TT.LParen))
				return false;
			if (!TryMatch((int) TT.RParen))
				return false;
			return true;
		}
		LNode ExprInParensOrTuple(int startIndex, int endIndex)
		{
			TokenType la0, la1;
			var e = ExprStart();
			la0 = LA0;
			if (la0 == TT.Comma) {
				var list = new RVList<LNode> { 
					e
				};
				for (;;) {
					la0 = LA0;
					if (la0 == TT.Comma) {
						la1 = LA(1);
						if (Expr_set0.Contains((int) la1)) {
							Skip();
							list.Add(ExprStart());
						} else
							Skip();
					} else
						break;
				}
				return F.Tuple(list, startIndex, endIndex);
			}
			Match((int) EOF);
			return F.InParens(e, startIndex, endIndex);
		}
		LNode BracedBlock()
		{
			var t = Match((int) TT.LBrace);
			Match((int) TT.RBrace);
			return F.Braces(StmtListInside(t).ToRVList());
		}
		bool Scan_BracedBlock()
		{
			if (!TryMatch((int) TT.LBrace))
				return false;
			if (!TryMatch((int) TT.RBrace))
				return false;
			return true;
		}
		void WhereClausesOpt(ref LNode name)
		{
		}
		void WhereClause()
		{
			Check(Is(0, _where), "Is($LI, _where)");
			Match((int) TT.Id);
		}
		LNode SpaceDecl(RVList<LNode> attrs)
		{
			TokenType la0;
			var t = MatchAny();
			var kind = (Symbol) t.Value;
			var name = ComplexId();
			var bases = BaseListOpt();
			WhereClausesOpt(ref name);
			la0 = LA0;
			if (la0 == TT.Semicolon) {
				var end = MatchAny();
				return F.Call(kind, name, bases, t.StartIndex, end.EndIndex).WithAttrs(attrs);
			} else {
				var body = BracedBlock();
				return F.Call(kind, name, bases, body, t.StartIndex, body.Range.EndIndex).WithAttrs(attrs);
			}
		}
		LNode BaseListOpt()
		{
			TokenType la0;
			la0 = LA0;
			if (la0 == TT.Colon) {
				var bases = new RVList<LNode>();
				Skip();
				bases.Add(DataType());
				for (;;) {
					la0 = LA0;
					if (la0 == TT.Comma) {
						Skip();
						bases.Add(DataType());
					} else
						break;
				}
				return F.List(bases);
			} else
				return F.List();
		}
		LNode ExprOpt()
		{
			TokenType la0;
			la0 = LA0;
			if (Expr_set0.Contains((int) la0)) {
				var e = ExprStart();
				return e;
			} else {
				var i = GetTextPosition(InputPosition);
				return F.Id(S.Missing, i, i);
			}
		}
		void ExprList(RWList<LNode> list)
		{
			TokenType la0;
			list.Add(ExprOpt());
			for (;;) {
				la0 = LA0;
				if (la0 == TT.Comma) {
					Skip();
					list.Add(ExprOpt());
				} else if (la0 == EOF)
					break;
				else {
					Error("Syntax error in expression list");
					for (;;) {
						la0 = LA0;
						if (!(la0 == EOF || la0 == TT.Comma))
							Skip();
						else
							break;
					}
				}
			}
			Skip();
		}
		void StmtList(RWList<LNode> list)
		{
			for (;;) {
				switch (LA0) {
				case TT.@new:
				case TT.AttrKeyword:
				case TT.Id:
				case TT.LBrack:
					list.Add(Stmt());
					break;
				default:
					goto stop;
				}
			}
		 stop:;
			Match((int) EOF);
		}
		static readonly HashSet<int> PrimaryExpr_Test0_set0 = NewSet((int) TT.Id);
		private bool Try_PrimaryExpr_Test0(int lookaheadAmt)
		{
			using (new SavePosition(this, lookaheadAmt))
				return PrimaryExpr_Test0();
		}
		private bool PrimaryExpr_Test0()
		{
			if (!Scan_TParams())
				return false;
			if (!TryMatchExcept(PrimaryExpr_Test0_set0))
				return false;
			return true;
		}
		private bool Try_PrefixExpr_Test0(int lookaheadAmt)
		{
			using (new SavePosition(this, lookaheadAmt))
				return PrefixExpr_Test0();
		}
		private bool PrefixExpr_Test0()
		{
			TokenType la0;
			la0 = LA0;
			if (la0 == TT.Add || la0 == TT.Sub)
				{if (!TryMatch((int) TT.Add, (int) TT.Sub))
					return false;}
			else {
				if (!TryMatch((int) TT.IncDec))
					return false;
				if (!TryMatch((int) TT.LParen))
					return false;
			}
			return true;
		}
		static readonly HashSet<int> WordAttributes_Test0_set0 = NewSet((int) TT.@break, (int) TT.@continue, (int) TT.@return, (int) TT.@throw, (int) TT.@case, (int) TT.@class, (int) TT.@delegate, (int) TT.@do, (int) TT.@enum, (int) TT.@event, (int) TT.@fixed, (int) TT.@for, (int) TT.@foreach, (int) TT.@goto, (int) TT.@interface, (int) TT.@lock, (int) TT.@namespace, (int) TT.@struct, (int) TT.@switch, (int) TT.@try, (int) TT.@using, (int) TT.@while);
		private bool Try_WordAttributes_Test0(int lookaheadAmt)
		{
			using (new SavePosition(this, lookaheadAmt))
				return WordAttributes_Test0();
		}
		private bool WordAttributes_Test0()
		{
			switch (LA0) {
			case TT.Id:
			case TT.Substitute:
			case TT.TypeKeyword:
				{
					if (!Scan_DataType())
						return false;
					if (!TryMatch((int) TT.Id))
						return false;
				}
				break;
			case TT.@this:
				{
					if (!(_spaceName != S.Def))
						return false;
					if (!TryMatch((int) TT.@this))
						return false;
				}
				break;
			case TT.@checked:
				{
					if (!TryMatch((int) TT.@checked))
						return false;
					if (!TryMatch((int) TT.LBrace))
						return false;
					if (!TryMatch((int) TT.RBrace))
						return false;
				}
				break;
			case TT.@unchecked:
				{
					if (!TryMatch((int) TT.@unchecked))
						return false;
					if (!TryMatch((int) TT.LBrace))
						return false;
					if (!TryMatch((int) TT.RBrace))
						return false;
				}
				break;
			case TT.@default:
				{
					if (!TryMatch((int) TT.@default))
						return false;
					if (!TryMatch((int) TT.Colon))
						return false;
				}
				break;
			default:
				if (!TryMatch(WordAttributes_Test0_set0))
					return false;
				break;
			}
			return true;
		}
	}
}
