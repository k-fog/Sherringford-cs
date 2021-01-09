using System;
using System.Collections.Generic;
using System.Text;

namespace Sherringford.She.Ast
{
    class FloatNumLiteral : ASTLeaf
    {
        public FloatNumLiteral(Token token) : base(token) { }
        public double Value() => ((FloatNumToken)Token).Value;
        public override object Eval(Environment env) => Value();
    }
}
