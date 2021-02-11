using System;
using System.Collections.Generic;
using System.Text;

namespace Sherringford.She
{
    class SheArray : List<object>
    {
        public SheArray(int capacity) : base(capacity) { }
        public SheArray(IEnumerable<object> collection) : base(collection) { }
        public override string ToString() => $"[{string.Join(",", this)}]";
    }
}
