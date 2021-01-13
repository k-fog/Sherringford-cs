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
        public Arguments Postfix(int nest) => (Arguments)GetChild(NumChildren() - nest - 1);
        public bool HasPostfix(int nest) => NumChildren() - nest > 1;

        public override object Eval(Environment env) => EvalSubExpr(env, 0);
        public object EvalSubExpr(Environment env, int nest)
        {
            if (HasPostfix(nest))
            {
                object target = EvalSubExpr(env, nest + 1);
                return ((Arguments)Postfix(nest)).Eval(env, target);
            }
            else return Operand().Eval(env);
        }

    }
}
