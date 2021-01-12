using Sherringford.She.Ast;
using System;
using System.Collections.Generic;
using System.Text;
using static Sherringford.She.Parser;

namespace Sherringford.She
{
    class SheParser
    {
        private HashSet<string> reserved = new HashSet<string>();
        private Operators operators = new Operators();
        private Parser expr, factor, simple, primary, block, module, proram;


        public SheParser()
        {
            this.reserved.Add(";");
            this.reserved.Add("}");
            this.reserved.Add(Token.EOL);

            this.operators.Add("=", 1, Operators.Right);
            this.operators.Add("==", 2, Operators.Left);
            this.operators.Add(">", 2, Operators.Left);
            this.operators.Add("<", 2, Operators.Left);
            this.operators.Add(">=", 2, Operators.Left);
            this.operators.Add("<=", 2, Operators.Left);
            this.operators.Add("+", 3, Operators.Left);
            this.operators.Add("-", 3, Operators.Left);
            this.operators.Add("*", 4, Operators.Left);
            this.operators.Add("/", 4, Operators.Left);
            this.operators.Add("%", 4, Operators.Left);

            Parser expr0 = Rule();
            this.primary = Rule(typeof(PrimaryExpr))
                .Or(
                    Rule().Sep("(").Ast(expr0).Sep(")"),
                    Rule().Number(typeof(FloatNumLiteral)),
                    Rule().Number(typeof(IntNumLiteral)),
                    Rule().Ident(typeof(Name), reserved),
                    Rule().String(typeof(StringLiteral)));
            this.factor = Rule().Or(Rule(typeof(NegativeExpr)).Sep("-").Ast(primary), primary);
            this.expr = expr0.Expression(typeof(BinaryExpr), factor, operators);
            Parser statement0 = Rule();
            this.block = Rule(typeof(BlockStmnt))
                .Sep("{").Option(statement0)
                .Rep(Rule().Sep(";", Token.EOL).Option(statement0))
                .Sep("}");

            Parser variable = Rule(typeof(LetStmnt)).Sep("let").Ident(typeof(Name), reserved).Sep("=").Ast(expr);
            this.simple = Rule(typeof(PrimaryExpr)).Or(variable, Rule().Ast(expr));
            Parser ifParser = Rule(typeof(IfStmnt)).Sep("if").Ast(expr).Ast(block).Option(Rule().Sep("else").Ast(block));
            Parser whileParser = Rule(typeof(WhileStmnt)).Sep("while").Ast(expr).Ast(block);
            Parser forIterControl = Rule(typeof(ForStmnt.ForIterExpr))
                .Sep("(")
                .Maybe(Rule().Ast(simple.Rep(Rule().Sep(",").Option(simple)))).Sep(";")
                .Maybe(Rule().Ast(simple)).Sep(";")
                .Maybe(Rule().Ast(simple))
                .Sep(")");
            Parser forParser = Rule(typeof(ForStmnt)).Sep("for").Ast(forIterControl).Ast(block);

            Parser statement = statement0.Or(
                ifParser,
                whileParser,
                forParser,
                simple);

            this.module = Rule().Or(statement, Rule(typeof(NullStmnt))).Sep(";", Token.EOL);
            // this.program = Rule().Rep(module);
        }

        private void FunctionParser()
        {
            Parser parameter = Rule().Ident(typeof(Name), reserved);
            Parser @params = Rule().Ast(parameter).Rep(Rule().Sep(",").Ast(parameter));
            Parser paramList = Rule().Sep("(").Maybe(@params).Sep(")");
            Parser def = Rule(typeof(FuncStmnt)).Sep("func").Ident(typeof(Name), reserved).Ast(paramList).Ast(block);
            Parser args = Rule(typeof(Arguments)).Ast(expr).Rep(Rule().Sep(",").Ast(expr));
            Parser postfix = Rule().Sep("(").Maybe(args).Sep(")");
            reserved.Add(")");
            primary.Rep(postfix);
            simple.Option(args);
            module.InsertChoice(def);
        }

        public ASTree Parse(Lexer lexer)
        {
            return module.Parse(lexer);
        }
    }
}
