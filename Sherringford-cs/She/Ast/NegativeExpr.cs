using System;
using System.Collections.Generic;
using System.Text;

namespace Sherringford.She.Ast
{
    class NegativeExpr : ASTList
    {
        public NegativeExpr(List<ASTree> c) : base(c) { }
        public ASTree Operand() => GetChild(0);
        public override string ToString() => "-" + Operand();
        public override object Eval(Environment env)
        {
            object x = Operand().Eval(env);
            if (x is int i) return -(int)i;
            else if (x is double d) return -(double)d;
            else throw new SheException("bad operand :", this);
        }
    }
}
