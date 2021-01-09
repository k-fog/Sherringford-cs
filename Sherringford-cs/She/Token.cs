using System;
using System.Collections.Generic;
using System.Text;

namespace Sherringford.She
{
    enum TokenType
    {
        Ident,
        IntNum,
        FloatNum,
        String,
        Bool,
        EOL,
        EOF,
        Illegal,
    }

    abstract class Token
    {
        public static Token EOF = EOFToken.EOF;
        public static readonly string EOL = @"\n";

        public int LineNumber { get; }

        public Token(int line)
        {
            this.LineNumber = line;
        }

        public abstract TokenType Type { get; }

        public override string ToString() => "";

        private class EOFToken : Token
        {
            public static new Token EOF { get; } = new EOFToken();
            public override TokenType Type { get => TokenType.EOF; }
            private EOFToken() : base(-1) { }
            public override string ToString() => "EOF";
        }
    }

    class IdToken : Token
    {
        public string Name { get; }

        public override TokenType Type { get => TokenType.Ident; }

        public IdToken(int line, string name)
          : base(line)
        {
            this.Name = name;
        }

        public override string ToString() => $"{Name}";
    }

    class IntNumToken : Token
    {
        public int Value { get; }

        public override TokenType Type { get => TokenType.IntNum; }

        public IntNumToken(int line, int value) :
          base(line)
        {
            this.Value = value;
        }

        public override string ToString() => $"{Value}";
    }

    class FloatNumToken : Token
    {
        public double Value { get; }

        public override TokenType Type { get => TokenType.FloatNum; }

        public FloatNumToken(int line, double value) : base(line)
        {
            this.Value = value;
        }

        public override string ToString() => $"{Value}";
    }

    class StringToken : Token
    {
        public string Value { get; }

        public override TokenType Type { get => TokenType.String; }

        public StringToken(int line, string value) :
          base(line)
        {
            this.Value = value;
        }

        public override string ToString() => $"\"{Value}\"";
    }

    /*class EOL : Token
    {
        public static readonly string Str = @"\n";
        public EOL(int line) : base(line) { }
        public override TokenType Type { get => TokenType.EOL; }
        public override string ToString() => @"\n";
    }*/
}
