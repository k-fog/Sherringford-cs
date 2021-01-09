using Sherringford.Algorithm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Sherringford.She
{
    class Lexer
    {
        private static readonly string Ident = @"[A-Z_a-z][A-Z_a-z0-9]*|==|<=|>=|&&|\|\||\p{S}|\p{P}";
        private static readonly string IntNum = @"[0-9]+";
        private static readonly string FloatNum = @"[0-9]+\.[0-9]*";
        private static readonly string String = "\"(\\\\\"|\\\\\\\\|\\\\n|[^\"])*\"";
        private static readonly string Bool = "True|False";

        public static string RegexPat = $"\\s*((?<comment>//.*)|(?<floatNum>{FloatNum})|(?<intNum>{IntNum})|(?<str>{String})|(?<bool>{Bool})|(?<id>{Ident}))?";

        private Regex Pattern = new Regex(RegexPat, RegexOptions.Compiled);
        private Deque<Token> Queue = new Deque<Token>();
        bool HasMore;
        private TextReader Reader;
        private int LineNumber;

        public Lexer(TextReader reader)
        {
            this.HasMore = true;
            this.LineNumber = 0;
            this.Reader = reader;
        }

        public Token Read()
        {
            if (FillQueue(0)) return Queue.PopFront();
            else return Token.EOF;
        }

        public Token Peek(int i)
        {
            if (FillQueue(i)) return Queue[i];
            else return Token.EOF;
        }

        private bool FillQueue(int i)
        {
            while (i >= Queue.Length)
            {
                if (HasMore) ReadLine();
                else return false;
            }
            return true;
        }

        protected void ReadLine()
        {
            string line = Reader.ReadLine();
            if (line == null)
            {
                HasMore = false;
                return;
            }
            int lineNo = LineNumber++;
            Match match = Pattern.Match(line);
            while (match.Success)
            {
                AddToken(lineNo, match);
                match = match.NextMatch();
            }
            Queue.PushBack(new IdToken(lineNo, Token.EOL));
        }

        protected void AddToken(int lineNo, Match match)
        {
            string m = match.Groups[1].Value;
            if (m == "") return;
            if (match.Groups["comment"].Value != "") return;
            Token token;
            if (match.Groups["floatNum"].Value != "") token = new FloatNumToken(lineNo, double.Parse(m));
            else if (match.Groups["intNum"].Value != "") token = new IntNumToken(lineNo, int.Parse(m));
            else if (match.Groups["str"].Value != "") token = new StringToken(lineNo, ToStringLiteral(m));
            else if (match.Groups["bool"].Value != "") token = new IntNumToken(lineNo, m == "True" ? Environment.True : Environment.False);
            else if (match.Groups["id"].Value != "") token = new IdToken(lineNo, m);
            else return;
            Queue.PushBack(token);
        }

        protected string ToStringLiteral(string s)
        {
            StringBuilder sb = new StringBuilder();
            int len = s.Length - 1;
            for (int i = 1; i < len; i++)
            {
                char c = s[i];
                if (c == '\\' && i + 1 < len)
                {
                    char c2 = s[i + 1];
                    if (c2 == '"' || c2 == '\\') c = s[++i];
                    else if (c2 == 'n')
                    {
                        ++i;
                        c = '\n';
                    }
                }
                sb.Append(c);
            }
            return sb.ToString();
        }
    }
}
