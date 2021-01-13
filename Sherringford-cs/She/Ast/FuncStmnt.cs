using System;
using System.Collections.Generic;
using System.Text;

namespace Sherringford.She.Ast
{
    class FuncStmnt : ASTList
    {
        public FuncStmnt(List<ASTree> c) : base(c) { }
        public string Name() => ((ASTLeaf)GetChild(0)).Token.ToString();
        public ParameterList Params() => (ParameterList)GetChild(1);
        public BlockStmnt Body() => (BlockStmnt)GetChild(2);
        public override string ToString() => $"(func {Name()} {Params()} {Body()})";

        public override object Eval(Environment env)
        {
            env.PutNew(Name(), new SheFunction(Params(), Body(), env));
            return Name();
        }
    }
}
