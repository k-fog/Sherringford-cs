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
        private Func<object> method0;

        public NativeFunction(string name, Func<object[], object> m)
        {
            this.Name = name;
            this.method = m;
            this.NumParams = m.GetMethodInfo().GetParameters().Length;
        }

        public NativeFunction(string name, Func<object> m)
        {
            this.Name = name;
            this.method0 = m;
            this.NumParams = 0;
        }

        public object Invoke(object[] args, ASTree tree)
        {
            try { return NumParams == 0 ? method0() : method(args); }
            catch (Exception) { throw new SheException("bad c#-native function call: " + Name, tree); }
        }

        public override string ToString() => $"<nativeFunc:{GetHashCode()}>";
    }

    static class Natives
    {
        public static void AppendNatives(Environment env)
        {
            env.PutNew("print", new NativeFunction("print", x =>
            {
                foreach (var item in x) Console.Write(item);
                Console.WriteLine();
                return null;
            }));
            env.PutNew("input", new NativeFunction("input", () => Console.ReadLine()));
            env.PutNew("currentTime", new NativeFunction("currentTime",
                () => (int)((DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds % 1e9)));
        }
    }
}
