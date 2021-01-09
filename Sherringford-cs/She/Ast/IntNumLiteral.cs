using System;
using System.Collections.Generic;
using System.Text;

namespace Sherringford.She.Ast
{
    class IntNumLiteral : ASTLeaf
    {
        public IntNumLiteral(Token token) : base(token) { }
        public int Value() => ((IntNumToken)Token).Value;
        public override object Eval(Environment env) => Value();
    }
}
