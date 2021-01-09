using System;
using System.Collections.Generic;
using System.Text;

namespace Sherringford.She.Ast
{
    class BlockStmnt : ASTList
    {
        public BlockStmnt(List<ASTree> c) : base(c) { }
        public override object Eval(Environment env)
        {
            object ret = null;
            foreach(var stmnt in this)
            {
                if (stmnt.GetType() == typeof(NullStmnt)) continue;
                ret = stmnt.Eval(env);
            }
            return ret;
        }
    }
}
