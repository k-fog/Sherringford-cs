using System;
using System.Collections.Generic;
using System.Text;

namespace Sherringford.She.Ast
{
    class Arguments : ASTList
    {
        public Arguments(List<ASTree> c) : base(c) { }
        public int Size() => NumChildren();
    }
}
