using System;
using System.Collections.Generic;
using System.Text;
using Sherringford.She.Ast;

namespace Sherringford.She
{
    class SheFunction
    {
        public ParameterList Parameters { protected set; get; }
        public BlockStmnt Body { protected set; get; }
        public Environment Env { protected set; get; }
        public SheFunction(ParameterList parameters, BlockStmnt body, Environment env)
        {
            this.Parameters = parameters;
            this.Body = body;
            this.Env = env;
        }
        public override string ToString() => $"<func:{GetHashCode()}>";
    }
}
