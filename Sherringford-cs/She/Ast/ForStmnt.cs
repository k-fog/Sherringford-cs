using System;
using System.Collections.Generic;
using System.Text;

namespace Sherringford.She.Ast
{
    class ForStmnt : ASTList
    {
        public ForStmnt(List<ASTree> c) : base(c) { }
        public ASTree IterControl() => GetChild(0);
        public ASTree ForBody() => GetChild(1);
        public override string ToString() => "(for " + IterControl() + " " + ForBody() + ")";

        public class ForIterExpr : ASTList
        {
            public ForIterExpr(List<ASTree> c) : base(c) { }
            public ASTree Define() => GetChild(0);
            public ASTree Condition() => GetChild(1);
            public ASTree Next() => GetChild(2);
        }

        public override object Eval(Environment env)
        {
            object ret = null;
            NestedEnvironment newScope = new NestedEnvironment(env);
            ((ForIterExpr)IterControl()).Define().Eval(newScope);
            while ((int)((ForIterExpr)IterControl()).Condition().Eval(newScope) == Environment.True)
            {
                ret = ForBody().Eval(newScope);
                ((ForIterExpr)IterControl()).Next().Eval(newScope);
            }
            return ret;
        }
    }
}
