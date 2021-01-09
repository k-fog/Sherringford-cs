using Sherringford.She;
using Sherringford.She.Ast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Sherringford.She
{
    class Parser
    {
        protected interface IElement
        {
            void Parse(Lexer lexer, List<ASTree> res);
            bool Match(Lexer lexer);
        }

        protected class Tree : IElement
        {
            protected Parser parser;
            public Tree(Parser p)
            {
                this.parser = p;
            }
            public void Parse(Lexer lexer, List<ASTree> res)
            {
                res.Add(parser.Parse(lexer));
            }
            public bool Match(Lexer lexer) => parser.Match(lexer);
        }

        protected class OrTree : IElement
        {
            protected Parser[] parsers;
            public OrTree(Parser[] p)
            {
                this.parsers = p;
            }
            public void Parse(Lexer lexer, List<ASTree> res)
            {
                Parser p = Choose(lexer);
                res.Add(p != null ? p.Parse(lexer) : throw new ParseException(lexer.Peek(0)));
            }
            public bool Match(Lexer lexer) => Choose(lexer) != null;
            protected Parser Choose(Lexer lexer)
            {
                foreach (var p in parsers) if (p.Match(lexer)) return p;
                return null;
            }
            public void Insert(Parser p)
            {
                Parser[] newParsers = new Parser[parsers.Length + 1];
                newParsers[0] = p;
                Array.Copy(parsers, 0, newParsers, 1, parsers.Length);
                parsers = newParsers;
            }
        }

        protected class Repeat : IElement
        {
            protected Parser parser;
            protected bool onlyOnce;
            public Repeat(Parser parser, bool once)
            {
                this.parser = parser;
                this.onlyOnce = once;
            }
            public void Parse(Lexer lexer, List<ASTree> res)
            {
                while (parser.Match(lexer))
                {
                    ASTree tree = parser.Parse(lexer);
                    if (tree.GetType() != typeof(ASTList) || tree.NumChildren() > 0) res.Add(tree);
                    if (onlyOnce) break;
                }
            }
            public bool Match(Lexer lexer) => parser.Match(lexer);
        }

        protected class Leaf : IElement
        {
            protected string[] tokens;
            public Leaf(string[] pat) { this.tokens = pat; }
            public void Parse(Lexer lexer, List<ASTree> res)
            {
                Token t = lexer.Read();
                if (t.Type == TokenType.Ident)
                    foreach (string token in tokens)
                    {
                        if (token == t.ToString())
                        {
                            Find(res, t);
                            return;
                        }
                    }
                if (tokens.Length > 0) throw new ParseException(tokens[0] + "expected.", t);
                else throw new ParseException(t);
            }
            public bool Match(Lexer lexer)
            {
                Token t = lexer.Peek(0);
                if (t.Type == TokenType.Ident) foreach (string token in tokens) if (token == t.ToString()) return true;
                return false;
            }
            protected virtual void Find(List<ASTree> res, Token token) => res.Add(new ASTLeaf(token));
        }

        protected abstract class AToken : IElement
        {
            protected Factory factory;
            public AToken(Type type)
            {
                if (type != null && !type.IsSubclassOf(typeof(ASTLeaf))) throw new ArgumentException($"{type} must extends {typeof(ASTLeaf)}");
                type ??= typeof(ASTLeaf);
                this.factory = Factory.Get(type, typeof(Token));
            }
            public void Parse(Lexer lexer, List<ASTree> res)
            {
                Token token = lexer.Read();
                if (Test(token))
                {
                    ASTree leaf = factory.Make(token);
                    res.Add(leaf);
                }
                else throw new ParseException(token);
            }
            public bool Match(Lexer lexer) => Test(lexer.Peek(0));
            protected abstract bool Test(Token token);
        }

        protected class IdToken : AToken
        {
            private HashSet<string> reserved;
            public IdToken(Type type, HashSet<string> r) : base(type)
            {
                reserved = r ?? new HashSet<string>();
            }
            protected override bool Test(Token token) => token.Type == TokenType.Ident && !reserved.Contains(token.ToString());
        }

        protected class IntNumToken : AToken
        {
            public IntNumToken(Type type) : base(type) { }
            protected override bool Test(Token token) => token.Type == TokenType.IntNum;
        }

        protected class FloatNumToken : AToken
        {
            public FloatNumToken(Type type) : base(type) { }
            protected override bool Test(Token token) => token.Type == TokenType.FloatNum;
        }

        protected class StrToken : AToken
        {
            public StrToken(Type type) : base(type) { }
            protected override bool Test(Token token) => token.Type == TokenType.String;
        }

        protected class Skip : Leaf
        {
            public Skip(string[] t) : base(t) { }
            protected override void Find(List<ASTree> res, Token token) { }
        }

        public class Precedence
        {
            public int Value { get; }
            public bool LeftAssoc { get; }
            public Precedence(int v, bool a)
            {
                this.Value = v;
                this.LeftAssoc = a;
            }
        }

        public class Operators : Dictionary<string, Precedence>
        {
            public static readonly bool Left = true;
            public static readonly bool Right = false;
            public void Add(string name, int prec, bool leftAssoc)
            {
                Add(name, new Precedence(prec, leftAssoc));
            }
        }

        protected class Expr : IElement
        {
            protected Factory factory;
            protected Operators ops;
            protected Parser factor;
            public Expr(Type clazz, Parser exp, Operators map)
            {
                this.factory = Factory.GetForASTList(clazz);
                this.ops = map;
                this.factor = exp;
            }
            public void Parse(Lexer lexer, List<ASTree> res)
            {
                ASTree right = factor.Parse(lexer);
                Precedence prec;
                while ((prec = NextOperator(lexer)) != null) right = DoShift(lexer, right, prec.Value);
                res.Add(right);
            }
            private ASTree DoShift(Lexer lexer, ASTree left, int prec)
            {
                List<ASTree> list = new List<ASTree>();
                list.Add(left);
                list.Add(new ASTLeaf(lexer.Read()));
                ASTree right = factor.Parse(lexer);
                Precedence next;
                while ((next = NextOperator(lexer)) != null && RightIsExpr(prec, next)) right = DoShift(lexer, right, next.Value);
                list.Add(right);
                return factory.Make(list);
            }
            private Precedence NextOperator(Lexer lexer)
            {
                Token t = lexer.Peek(0);
                if (t.Type == TokenType.Ident && ops.ContainsKey(t.ToString())) return ops[t.ToString()];
                return null;
            }
            private static bool RightIsExpr(int prec, Precedence nextPrec)
            {
                if (nextPrec.LeftAssoc) return prec < nextPrec.Value;
                else return prec <= nextPrec.Value;
            }
            public bool Match(Lexer lexer) => factor.Match(lexer);
        }


        private static readonly string FactoryName = "Create";
        protected class Factory
        {
            protected Func<object, ASTree> Make0 { get; }
            public Factory(Func<object, ASTree> make0)
            {
                this.Make0 = make0;
            }
            public ASTree Make(object arg)
            {
                try
                {
                    return Make0(arg);
                }
                catch (NotImplementedException e1)
                {
                    throw e1;
                }
                catch (Exception e2)
                {
                    throw e2;
                }
            }
            public static Factory GetForASTList(Type clazz)
            {
                Factory f = Get(clazz, typeof(List<ASTree>));
                f ??= new Factory((object arg) =>
                {
                    List<ASTree> results = (List<ASTree>)arg;
                    if (results.Count == 1) return results[0];
                    else return new ASTList(results);
                });
                return f;
            }
            public static Factory Get(Type clazz, Type argType)
            {
                if (clazz == null) return null;

                MethodInfo m = clazz.GetMethod(FactoryName, new Type[] { argType });
                if (m != null)
                {
                    return new Factory((object arg) =>
                    {
                        return (ASTree)m.Invoke(null, new object[] { arg });
                    });
                }

                ConstructorInfo c = clazz.GetConstructor(new Type[] { argType });
                return new Factory((object arg) =>
                {
                    return (ASTree)c.Invoke(new object[] { arg });
                });
            }
        }

        protected List<IElement> elements;
        protected Factory factory;

        public Parser(Type clazz) { Reset(clazz); }

        public Parser(Parser parser)
        {
            this.elements = parser.elements;
            this.factory = parser.factory;
        }

        public Parser Reset()
        {
            this.elements = new List<IElement>();
            return this;
        }

        public Parser Reset(Type clazz)
        {
            if (clazz != null && !clazz.IsSubclassOf(typeof(ASTree))) throw new ArgumentException($"{clazz} must extends {typeof(ASTLeaf)}");
            this.elements = new List<IElement>();
            this.factory = Factory.GetForASTList(clazz);
            return this;
        }

        public ASTree Parse(Lexer lexer)
        {
            List<ASTree> results = new List<ASTree>();
            foreach (var e in elements)
            {
                e.Parse(lexer, results);
            }
            return factory.Make(results);
        }

        public bool Match(Lexer lexer)
        {
            if (elements.Count == 0) return true;
            else
            {
                IElement e = elements[0];
                return e.Match(lexer);
            }
        }

        public static Parser Rule() => Rule(null);

        public static Parser Rule(Type clazz)
        {
            if (clazz != null && !clazz.IsSubclassOf(typeof(ASTree))) throw new ArgumentException($"{clazz} must extends {typeof(ASTLeaf)}");
            return new Parser(clazz);
        }

        public Parser Number() => Number(null);

        public Parser Number(Type clazz)
        {
            if (clazz != null && !clazz.IsSubclassOf(typeof(ASTLeaf))) throw new ArgumentException($"{clazz} must extends {typeof(ASTLeaf)}");
            if (clazz == typeof(IntNumLiteral)) elements.Add(new IntNumToken(clazz));
            else if (clazz == typeof(FloatNumLiteral)) elements.Add(new FloatNumToken(clazz));
            return this;
        }

        public Parser Ident(HashSet<string> reserved) => Ident(null, reserved);

        public Parser Ident(Type clazz, HashSet<string> reserved)
        {
            if (clazz != null && !clazz.IsSubclassOf(typeof(ASTLeaf))) throw new ArgumentException($"{clazz} must extends {typeof(ASTLeaf)}");
            elements.Add(new IdToken(clazz, reserved));
            return this;
        }

        public Parser String() => String(null);

        public Parser String(Type clazz)
        {
            if (clazz != null && !clazz.IsSubclassOf(typeof(ASTLeaf))) throw new ArgumentException($"{clazz} must extends {typeof(ASTLeaf)}");
            elements.Add(new StrToken(clazz));
            return this;
        }

        public Parser Token(params string[] pat)
        {
            elements.Add(new Leaf(pat));
            return this;
        }

        public Parser Sep(params string[] pat)
        {
            elements.Add(new Skip(pat));
            return this;
        }

        public Parser Ast(Parser p)
        {
            elements.Add(new Tree(p));
            return this;
        }

        public Parser Or(params Parser[] ps)
        {
            elements.Add(new OrTree(ps));
            return this;
        }

        public Parser Maybe(Parser p1)
        {
            Parser p2 = new Parser(p1);
            p2.Reset();
            elements.Add(new OrTree(new Parser[] { p1, p2 }));
            return this;
        }

        public Parser Option(Parser p)
        {
            elements.Add(new Repeat(p, true));
            return this;
        }

        public Parser Rep(Parser p)
        {
            elements.Add(new Repeat(p, false));
            return this;
        }

        public Parser Expression(Parser subexp, Operators operators)
        {
            elements.Add(new Expr(null, subexp, operators));
            return this;
        }

        public Parser Expression(Type clazz, Parser subexp, Operators operators)
        {
            if (clazz != null && !clazz.IsSubclassOf(typeof(ASTree))) throw new ArgumentException($"{clazz} must extends {typeof(ASTLeaf)}");
            elements.Add(new Expr(clazz, subexp, operators));
            return this;
        }

        public Parser InsertChoice(Parser p)
        {
            IElement e = elements[0];
            if (e is OrTree) ((OrTree)e).Insert(p);
            else
            {
                Parser otherwise = new Parser(this);
                Reset(null);
                Or(p, otherwise);
            }
            return this;
        }
    }
}