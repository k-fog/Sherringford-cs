using System;
using System.Collections.Generic;
using System.Reflection;
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

    class NativeFunction
    {
        public string Name { private set; get; }
        public int NumParams { private set; get; }
        private Func<object[], object> method;

        public NativeFunction(string name, Func<object[], object> m)
        {
            this.Name = name;
            this.method = m;
        }

        public object Invoke(object[] args, ASTree tree)
        {
            try { return method(args); }
            catch (Exception) { throw new SheException("bad c#-native function call: " + Name, tree); }
        }

        public override string ToString() => $"<nativeFunc:{GetHashCode()}>";
    }
}
