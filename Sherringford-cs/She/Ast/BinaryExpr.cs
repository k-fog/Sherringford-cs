using System;
using System.Collections.Generic;
using System.Text;

namespace Sherringford.She.Ast
{
    class BinaryExpr : ASTList
    {
        public BinaryExpr(List<ASTree> c) : base(c) { }
        public ASTree Left() => GetChild(0);
        public string Operator() => ((ASTLeaf)GetChild(1)).Token.ToString();
        public ASTree Right() => GetChild(2);

        public override object Eval(Environment env)
        {
            if (Operator().ToString() == "=")
            {
                object right = Right().Eval(env);
                return ComputeAssign(env, right);
            }
            else
            {
                object left = ((ASTree)Left()).Eval(env);
                object right = ((ASTree)Right()).Eval(env);
                return ComputeOp(left, Operator().ToString(), right);
            }

        }

        private object ComputeAssign(Environment env, Object rvalue)
        {
            if (Left().GetType() == typeof(Name))
            {
                string variable = ((Name)Left()).TheName();
                if (!env.Exist(variable)) throw new SheException($"{variable} doesnt exist :", this);
                env.Put(variable, rvalue);
                return rvalue;
            }
            else
                throw new SheException("bad assignment", this);
        }

        private object ComputeOp(object left, string op, object right)
        {
            if (left is int x && right is int y) return ComputeNumber(x, op, y);
            else if (left is double a && right is int b) return ComputeNumber(a, op, (double)b);
            else if (left is int c && right is double d) return ComputeNumber((double)c, op, d);
            else if (left is double e && right is double f) return ComputeNumber(e, op, f);
            else if (left is string s && right is string t)
            {
                if (op == "+") return s + t;
                else if (op == "==") return s == t ? Environment.True : Environment.False;
                else throw new SheException("bad operator", this);
            }
            else throw new SheException("can't evaluate :", this);
        }

        private object ComputeNumber(int a, string op, int b)
        {
            if (op == "+") return a + b;
            else if (op == "-") return a - b;
            else if (op == "*") return a * b;
            else if (op == "/") return a / b;
            else if (op == "%") return a % b;
            else if (op == "==") return a == b ? Environment.True : Environment.False;
            else if (op == "!=") return a != b ? Environment.True : Environment.False;
            else if (op == "<") return a < b ? Environment.True : Environment.False;
            else if (op == ">") return a > b ? Environment.True : Environment.False;
            else if (op == "<=") return a <= b ? Environment.True : Environment.False;
            else if (op == ">=") return a >= b ? Environment.True : Environment.False;
            else throw new SheException("bad operator", this);
        }

        private object ComputeNumber(double a, string op, double b)
        {
            if (op == "+") return a + b;
            else if (op == "-") return a - b;
            else if (op == "*") return a * b;
            else if (op == "/") return a / b;
            else if (op == "%") return a % b;
            else if (op == "==") return a == b ? Environment.True : Environment.False;
            else if (op == "!=") return a != b ? Environment.True : Environment.False;
            else if (op == "<") return a < b ? Environment.True : Environment.False;
            else if (op == ">") return a > b ? Environment.True : Environment.False;
            else if (op == "<=") return a <= b ? Environment.True : Environment.False;
            else if (op == ">=") return a >= b ? Environment.True : Environment.False;
            else throw new SheException("bad operator", this);
        }
    }
}
