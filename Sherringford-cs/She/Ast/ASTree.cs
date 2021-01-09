using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Sherringford.She.Ast
{
    abstract class ASTree : IEnumerable<ASTree>
    {
        public abstract ASTree GetChild(int i);
        public abstract int NumChildren();
        public abstract IEnumerator<ASTree> GetChildren();
        public abstract string Location();
        public abstract object Eval(Environment env);
        public IEnumerator<ASTree> GetEnumerator() => GetChildren();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    class ASTList : ASTree
    {
        protected List<ASTree> Children;

        public ASTList(List<ASTree> list)
        {
            this.Children = list;
        }

        public override ASTree GetChild(int i) => Children[i];

        public override int NumChildren() => Children.Count;

        public override IEnumerator<ASTree> GetChildren() => Children.GetEnumerator();

        public override string Location()
        {
            foreach (ASTree tree in Children)
            {
                string s = tree.Location();
                if (s != null) return s;
            }
            return null;
        }

        public override object Eval(Environment env)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("(");
            string sep = "";
            foreach (ASTree tree in Children)
            {
                sb.Append(sep);
                sep = " ";
                sb.Append(tree.ToString());
            }
            sb.Append(")");
            return sb.ToString();
        }
    }

    class ASTLeaf : ASTree
    {
        private static readonly List<ASTree> empty = new List<ASTree>();
        public Token Token { protected set; get; }

        public ASTLeaf(Token token)
        {
            this.Token = token;
        }

        public override ASTree GetChild(int i) => null;

        public override int NumChildren() => 0;

        public override IEnumerator<ASTree> GetChildren() => empty.GetEnumerator();

        public override string Location() => "at line " + Token.LineNumber;

        public override object Eval(Environment env)
        {
            throw new NotImplementedException();
        }

        public override string ToString() => Token.ToString();
    }
}
