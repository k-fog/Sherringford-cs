using System;
using System.Collections.Generic;
using System.Text;
using Sherringford.She.Ast;

namespace Sherringford.She
{
    class SheException : Exception
    {
        public SheException(string m) : base(m) { }
        public SheException(string m, ASTree t) : base(m + " " + t.Location()) { }
    }
}
