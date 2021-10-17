using System.Runtime.Serialization;

namespace NetworkSkins {
    using System.Collections.Generic;

    public class DictionarySoft<TKey, TValue> : Dictionary<TKey, TValue> {
        public DictionarySoft() : base() { }
        //
        // Summary:
        //     Initializes a new instance of the System.Collections.Generic.Dictionary`2 class
        //     that is empty, has the specified initial capacity, and uses the default equality
        //     comparer for the key type.
        //
        // Parameters:
        //   capacity:
        //     The initial number of elements that the System.Collections.Generic.Dictionary`2
        //     can contain.
        //
        // Exceptions:
        //   T:System.ArgumentOutOfRangeException:
        //     capacity is less than 0.
        public DictionarySoft(int capacity) : base(capacity) { }


        //
        // Summary:
        //     Gets or sets the value associated with the specified key.
        //
        // Parameters:
        //   key:
        //     The key of the value to get or set.
        //
        // Returns:
        //     The value associated with the specified key. If the specified key is not found,
        //     a get operation returns null or default, and
        //     a set operation creates a new element with the specified key.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     key is null.
        //
        //   T:System.Collections.Generic.KeyNotFoundException:
        //     The property is retrieved and key does not exist in the collection.
        public new TValue this[TKey key] {
            get {
                if(TryGetValue(key, out var ret))
                    return ret;
                return default;
            }
            set => this[key] = value;
        }


    }
}
