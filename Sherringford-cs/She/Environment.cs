using System;
using System.Collections.Generic;
using System.Text;

namespace Sherringford.She
{
    abstract class Environment
    {
        public static readonly int True = 1, False = 0;
        public abstract void Put(string key, object value);
        public abstract bool Exist(string key);
        public abstract object Get(string key);
    }

    class BasicEnvironment : Environment
    {
        private Dictionary<string, object> values;
        public BasicEnvironment()
        {
            this.values = new Dictionary<string, object>();
        }
        public override void Put(string key, object value)
        {
            if (!Exist(key)) values.Add(key, value);
            else values[key] = value;
        }
        public override bool Exist(string key) => values.ContainsKey(key);
        public override object Get(string key) => values[key];
    }

    class NestedEnvironment : Environment
    {
        private Environment outer;
        private Dictionary<string, object> values;

        public NestedEnvironment() : this(null) { }
        public NestedEnvironment(Environment outer)
        {
            this.outer = outer;
        }

        public override void Put(string key, object value)
        {
            if (!Exist(key)) values.Add(key, value);
            else values[key] = value;
        }

        public override bool Exist(string key) => values.ContainsKey(key);

        public override object Get(string key) => values[key];

        public Environment Where(string key)
        {

        }
    }
}
