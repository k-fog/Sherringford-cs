using System;
using System.Collections.Generic;
using System.Text;

namespace Sherringford.She.Ast
{
    class NullStmnt : ASTList
    {
        public NullStmnt(List<ASTree> c) : base(c) { }
        public override object Eval(Environment env) => null;
    }
}
