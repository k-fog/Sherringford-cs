using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Sherringford.She
{
    class ParseException : Exception
    {
        public ParseException(Token token) : this("", token) { }
        public ParseException(string message, Token token) : base("syntax error around " + Location(token) + ". " + message) { }
        private static string Location(Token token)
        {
            if (token == Token.EOF) return "the last line";
            else return "\"" + token + "\" at line " + token.LineNumber;
        }
        public ParseException(IOException e) : base("", e) { }
        public ParseException(string message) : base(message) { }
    }
}
