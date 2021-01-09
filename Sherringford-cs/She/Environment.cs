using System;
using System.Collections.Generic;
using System.Text;

namespace Sherringford.She
{
    abstract class Environment
    {
        public static readonly int True = 1, False = 0;
        public abstract void Put(string key, object value);
        public abstract void PutNew(string key, object value);
        public abstract bool Exist(string key);
        public abstract object Get(string key);
    }

    // old
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
        public override void PutNew(string key, object value)
        {
            throw new NotImplementedException();
        }
        public override bool Exist(string key) => values.ContainsKey(key);
        public override object Get(string key) => values[key];
    }

    class NestedEnvironment : Environment
    {
        public Environment Outer { set; get; }
        protected Dictionary<string, object> values;

        public NestedEnvironment() : this(null) { }
        public NestedEnvironment(Environment outer)
        {
            this.values = new Dictionary<string, object>();
            this.Outer = outer;
        }

        public override void Put(string key, object value)
        {
            Environment e = Where(key) ?? this;
            ((NestedEnvironment)e).PutNew(key, value);
        }

        public override void PutNew(string key, object value)
        {
            values[key] = value;
        }

        public override bool Exist(string key) => values.ContainsKey(key);

        public override object Get(string key) => values[key];

        public Environment Where(string key)
        {
            if (values.ContainsKey(key)) return this;
            return ((NestedEnvironment)Outer)?.Where(key);
        }
    }
}
