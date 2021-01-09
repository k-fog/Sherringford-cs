using System;
using System.Collections.Generic;
using System.Text;

namespace Sherringford.She.Ast
{
    class IfStmnt : ASTList
    {
        public IfStmnt(List<ASTree> c) : base(c) { }
        public ASTree Condition() => GetChild(0);
        public ASTree ThenBlock() => GetChild(1);
        public ASTree ElseBlock() => NumChildren() > 2 ? GetChild(2) : null;
        public override string ToString() => "(if " + Condition() + " " + ThenBlock() + (ElseBlock() != null ? " else " + ElseBlock() : "") + ")";

        public override object Eval(Environment env)
        {
            if ((int)Condition().Eval(env) == Environment.True) return ThenBlock().Eval(env);
            else return ElseBlock()?.Eval(env);
        }
    }
}
