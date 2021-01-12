using System;
using System.Collections.Generic;
using System.Text;

namespace Sherringford.She.Ast
{
    class ParameterList : ASTList
    {
        public ParameterList(List<ASTree> c) : base(c) { }
        public string Name(int i) => ((ASTLeaf)GetChild(i)).Token.ToString();
        public int Size() => NumChildren();
    }
}
