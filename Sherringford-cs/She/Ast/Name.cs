using System;
using System.Collections.Generic;
using System.Text;

namespace Sherringford.She.Ast
{
    class Name : ASTLeaf
    {
        public Name(Token token) : base(token) { }
        public string TheName() => Token.ToString();
        public override object Eval(Environment env)
        {
            if (!env.Exist(TheName())) throw new SheException($"{TheName()} doesnt exist :", this);
            return env.Get(TheName());
        }
    }
}
