using Sherringford.She.Ast;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Sherringford.She
{
    class Interpreter
    {
        private readonly string filePath;

        public Interpreter(string path)
        {
            this.filePath = path;
        }

        public void Run()
        {
            ASTVisualizer visualizer = SheInfo.Visualize ? new ASTVisualizer() : null;
            using (var reader = new StreamReader(filePath))
            {
                Lexer l = new Lexer(reader);
                SheParser sp = new SheParser();
                while (l.Peek(0) != Token.EOF)
                {
                    ASTree ast = sp.Parse(l);
                    Console.WriteLine(ast);
                    if (SheInfo.Visualize) visualizer.Push(ast);
                }
            }
            if (SheInfo.Visualize) visualizer.Visualize($"INTPRT_{SheInfo.StartTime:yyyy-MM-dd-HH-mm-ss}");
        }
    }
}
