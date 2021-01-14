using System;
using System.Collections.Generic;
using System.Text;

namespace Sherringford.She
{
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
            // env.PutNew("input", new NativeFunction("input", x => Console.ReadLine()));
        }
    }
}
