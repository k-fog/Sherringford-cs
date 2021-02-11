using System;
using System.Collections.Generic;
using System.Text;

namespace Sherringford.She.Ast
{
    class PrimaryExpr : ASTList
    {
        public PrimaryExpr(List<ASTree> c) : base(c) { }
        public static ASTree Create(List<ASTree> c) => c.Count == 1 ? c[0] : new PrimaryExpr(c);
        public ASTree Operand() => GetChild(0);
        public Postfix Postfix(int nest) => (Postfix)GetChild(NumChildren() - nest - 1);
        public bool HasPostfix(int nest) => NumChildren() - nest > 1;

        public override object Eval(Environment env) => EvalSubExpr(env, 0);
        public object EvalSubExpr(Environment env, int nest)
        {
            if (HasPostfix(nest))
            {
                object target = EvalSubExpr(env, nest + 1);
                object p = Postfix(nest);
                if (p is Arguments arg) return arg.Eval(env, target);
                else if (p is ArrayRef arr) return arr.Eval(env, target);
                else throw new SheException("bad postfix");
            }
            else return Operand().Eval(env);
        }

    }
}
