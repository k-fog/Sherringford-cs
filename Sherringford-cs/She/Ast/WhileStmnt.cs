using System;
using System.Collections.Generic;
using System.Text;

namespace Sherringford.She.Ast
{
    class WhileStmnt : ASTList
    {
        public WhileStmnt(List<ASTree> c) : base(c) { }
        public ASTree Condition() => GetChild(0);
        public ASTree LoopBody() => GetChild(1);
        public override string ToString() => "(while " + Condition() + " " + LoopBody() + ")";

        public override object Eval(Environment env)
        {
            object ret = null;
            while((int)Condition().Eval(env) == Environment.True)
            {
                ret = LoopBody().Eval(env);
            }
            return ret;
        }
    }
}
