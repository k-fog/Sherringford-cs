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
            object[] res = new object[s];
            for (int i = 0; i < s; i++) res[i] = GetChild(i).Eval(env);
            return res;
        }
    }
}
