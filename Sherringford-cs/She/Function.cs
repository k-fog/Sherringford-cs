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
        public static readonly int VariadicArg = -1;

        public NativeFunction(string name, int numParams, Func<object[], object> m)
        {
            this.Name = name;
            this.method = m;
            this.NumParams = numParams;
        }

        public NativeFunction(string name, int numParams, Func<object> m)
        {
            this.Name = name;
            this.method0 = m;
            this.NumParams = numParams == 0 ? 0 : throw new ArgumentOutOfRangeException($"error at create native function {name}");
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
            env.PutNew("print", new NativeFunction("print", NativeFunction.VariadicArg, x =>
            {
                foreach (var item in x) Console.Write(item);
                Console.WriteLine();
                return null;
            }));
            env.PutNew("input", new NativeFunction("input", 0, () => Console.ReadLine()));
            env.PutNew("int", new NativeFunction("int", 1, (x) => x[0] as int?));
            env.PutNew("typeof", new NativeFunction("typeof", 1, (x) => x[0].GetType().ToString()));
            env.PutNew("currentTime", new NativeFunction("currentTime", 0,
                () => (int)((DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds % 1e9)));
            env.PutNew("len", new NativeFunction("len", 1, 
                (x) => x[0] is SheArray arr ? arr.Count : throw new SheException("len: argument type must be SheArray")));
        }
    }
}
