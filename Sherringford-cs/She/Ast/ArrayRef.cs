using System;
using System.Collections.Generic;
using System.Text;

namespace Sherringford.She.Ast
{
    class ArrayRef : Postfix
    {
        public ArrayRef(List<ASTree> c) : base(c) { }
        public ASTree Index() => GetChild(0);
        public override string ToString() => $"[{Index()}]";

        public object Eval(Environment env, object value)
        {
            if(value is SheArray list)
            {
                object index = Index().Eval(env);
                if (index is int i) return list[i];
            }
            throw new SheException("bad array access", this);
        }
    }
}
