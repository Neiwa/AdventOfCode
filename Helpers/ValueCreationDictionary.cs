using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers
{
    public class ValueCreationDictionary<TKey, TValue> : Dictionary<TKey, TValue> where TKey : notnull where TValue : new()
    {
        private readonly Func<TKey, TValue> _valueFactory;

        public ValueCreationDictionary()
        {
            _valueFactory = k => new TValue();
        }

        public ValueCreationDictionary(Func<TKey, TValue> valueFactory)
        {
            _valueFactory = valueFactory;
        }

        public new TValue this[TKey key]
        {
            get
            {
                if (!TryGetValue(key, out var value))
                {
                    value = _valueFactory(key);
                    Add(key, value);
                }
                return value;
            }
            set
            {
                base[key] = value;
            }
        }
    }
}
