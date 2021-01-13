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
        public void Eval(Environment env, int index, object value) => env.PutNew(Name(index), value);
    }
}
