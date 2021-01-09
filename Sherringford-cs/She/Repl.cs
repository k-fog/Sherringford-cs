using Sherringford.She.Ast;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Sherringford.She
{
    class Repl
    {
        private readonly string startMessage = $"Welcome to Sherringford {SheInfo.Version}, {SheInfo.UserName} !!! ({SheInfo.StartTime})\n[on {SheInfo.Environment}]";
        private readonly string prompt = "=>";
        private static int Count = 0;
        private Environment replEnvironment;

        public Repl()
        {
            this.replEnvironment = new BasicEnvironment();
        }

        public void Start()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(startMessage);
            while (true)
            {
                WriteSystemMessage(prompt);

                Stack<char> indent = new Stack<char>();
                string input = Console.ReadLine();
                if (string.IsNullOrEmpty(input)) break;

                if (input.EndsWith('{')) indent.Push('{');
                while (indent.Count > 0)
                {
                    WriteSystemMessage("--");
                    input += Console.ReadLine();
                    if (input.EndsWith('{')) indent.Push('{');
                    if (input.EndsWith('}'))
                    {
                        indent.Pop();
                    }
                }

                Lexer lexer = new Lexer(new StringReader(input));


                /*while (lexer.Peek(0) != Token.EOF)
                {
                    Console.WriteLine($"{lexer.Peek(0).GetType()}:{lexer.Read()}");
                }*/

                
                SheParser parser = new SheParser();
                ASTree ast = parser.Parse(lexer);

                if (SheInfo.Visualize) ASTVisualizer.Visualize(ast.PlotDotGraph(), $"REPL_{SheInfo.StartTime:yyyy-MM-dd-HH-mm-ss}_{Count++}");
                // Console.WriteLine(ast);
                Console.WriteLine(ast.Eval(replEnvironment));
                Console.WriteLine();
            }
        }

        private void WriteSystemMessage(string s)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(s);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
