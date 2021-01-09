using System;
using System.Collections.Generic;
using System.Text;

namespace Sherringford.She.Ast
{
    class PrimaryExpr : ASTList
    {
        public PrimaryExpr(List<ASTree> c) : base(c) { }
        public static ASTree Create(List<ASTree> c) => c.Count == 1 ? c[0] : new PrimaryExpr(c);
    }
}
