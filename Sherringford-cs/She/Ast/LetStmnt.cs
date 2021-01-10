using System;
using System.Collections.Generic;
using System.Text;

namespace Sherringford.She.Ast
{
    class LetStmnt : ASTList
    {
        public LetStmnt(List<ASTree> c) : base(c) { }
        public ASTree Name() => GetChild(0);
        public ASTree Value() => GetChild(1);
        public override string ToString() => $"(let {Name()} = {Value()})";

        public override object Eval(Environment env)
        {
            if (((NestedEnvironment)env).Where(Name().ToString()) == env) throw new SheException($"{Name()} already exists :", this);
            object v = Value().Eval(env);
            env.Put(((Name)Name()).TheName(), v);
            return v;
        }
    }
}
