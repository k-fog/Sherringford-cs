using Sherringford.She;
using Sherringford.She.Ast;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Sherringford
{
    static class SheInfo
    {
        public static readonly string Version = "0.2.4";
        public static DateTime StartTime { private set; get; }
        public static string Environment { private set; get; }
        public static string UserName { private set; get; }
        public static string ExePath { private set; get; }
        public static bool Visualize { set; get; } = true;
        public static string TempDirectory { set; get; } = @".\output";
        public static bool Setup()
        {
            StartTime = DateTime.Now;
            Environment = System.Environment.OSVersion.ToString();
            UserName = System.Environment.UserName;
            ExePath = Directory.GetCurrentDirectory();
            if (!Directory.Exists(TempDirectory))
            {
                Directory.CreateDirectory(TempDirectory);
            }
            return true;
        }
    }

    class App
    {
        static void Main(string[] args)
        {
            SheInfo.Setup();

            if (args.Length == 0)
            {
                var repl = new Repl();
                repl.Start();
            }
            else
            {
                var runner = new Interpreter(args[0]);
                runner.Run();
            }
        }
    }
}
