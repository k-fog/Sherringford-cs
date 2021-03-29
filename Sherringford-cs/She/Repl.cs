using Sherringford.She.Ast;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Sherringford.She
{
    class Repl
    {
        private readonly string startMessage = $"Welcome to Sherringford {SheInfo.Version}, {SheInfo.UserName} !!! ({SheInfo.StartTime})\n[on {SheInfo.Environment}]";
        private readonly string prompt = "=>";
        private Environment replEnvironment;

        public Repl()
        {
            this.replEnvironment = new NestedEnvironment();
            Natives.AppendNatives(this.replEnvironment);
        }

        public void Start()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(startMessage);
            ASTVisualizer visualizer = SheInfo.Visualize ? new ASTVisualizer() : null;
            while (true)
            {
                WriteSystemMessage(prompt);
                string input = ReadSource();
                if (input == null) break;
                Lexer lexer = new Lexer(new StringReader(input));

                /*while (lexer.Peek(0) != Token.EOF)
                {
                    Console.WriteLine($"{lexer.Peek(0).GetType()}:{lexer.Read()}");
                }*/

                SheParser parser = new SheParser();
                ASTree ast = parser.Parse(lexer);

                if (SheInfo.Visualize) visualizer.Push(ast);
                //Console.WriteLine(ast);
                Console.WriteLine(ast.Eval(replEnvironment));
                Console.WriteLine();
            }
            if (SheInfo.Visualize) visualizer.Visualize($"REPL_{SheInfo.StartTime:yyyy-MM-dd-HH-mm-ss}");
        }

        private string ReadSource()
        {
            Stack<char> indent = new Stack<char>();
            StringBuilder input = new StringBuilder(Console.ReadLine());
            if (string.IsNullOrEmpty(input.ToString())) return null;

            for (int i = 0; i < input.ToString().Count(c => c == '{'); i++) indent.Push('{');
            while (indent.Count > 0)
            {
                WriteSystemMessage("--");
                string line = Console.ReadLine();
                for (int i = 0; i < line.Count(c => c == '{'); i++) indent.Push('{');
                for (int i = 0; i < line.Count(c => c == '}'); i++) indent.Pop();
                input.Append(line);
            }
            return input.ToString();
        }

        private void WriteSystemMessage(string s)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(s);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
