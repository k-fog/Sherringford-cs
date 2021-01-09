using System;
using System.Collections.Generic;
using System.Text;

namespace Sherringford.She.Ast
{
    class StringLiteral : ASTLeaf
    {
        public StringLiteral(Token token) : base(token) { }
        public string Value() => ((StringToken)Token).Value;
        public override object Eval(Environment env) => Value();
    }
}
