using System;
using System.Collections.Generic;
using System.Text;

namespace Sherringford.She.Ast
{
    class Arguments : ASTList
    {
        public Arguments(List<ASTree> c) : base(c) { }
        public int Size() => NumChildren();

        public object Eval(Environment env, object value)
        {
            if (value.GetType() == typeof(SheFunction))
            {
                SheFunction func = (SheFunction)value;
                ParameterList @params = func.Parameters;
                if (Size() != @params.Size()) throw new SheException("bad number of arguments", this);
                Environment newEnv = new NestedEnvironment(env);
                int num = 0;
                foreach (ASTree ast in this) { @params.Eval(newEnv, num++, ast.Eval(env)); }
                return func.Body.Eval(newEnv);
            }
            else if (value.GetType() == typeof(NativeFunction))
            {
                NativeFunction func = (NativeFunction)value;
                if(func.NumParams != NativeFunction.VariadicArg && func.NumParams != NumChildren()) throw new SheException("bad number of arguments", this);
                object[] @params = new object[NumChildren()];
                for (int i = 0; i < NumChildren(); i++) @params[i] = GetChild(i).Eval(env);
                return func.Invoke(@params, this);
            }
            else throw new SheException("bad function", this);
        }
    }
}
