using System;
using System.Collections.Generic;
using System.Text;

namespace Sherringford.She.Ast
{
    class ArrayLiteral : ASTList
    {
        public ArrayLiteral(List<ASTree> c) : base(c) { }
        public int Size() => NumChildren();
        public override string ToString() => $"[array]";

        public override object Eval(Environment env)
        {
            int s = NumChildren();
            SheArray res = new SheArray(s);
            for (int i = 0; i < s; i++) res.Add(GetChild(i).Eval(env));
            return res;
        }
    }
}
