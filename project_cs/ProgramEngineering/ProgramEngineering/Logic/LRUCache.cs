using System.Collections.Specialized;
public class LRUCache
{
    private OrderedDictionary _dict;
    private readonly int _capacity;
    public LRUCache(int capacity = 10)
    {
        _dict = new OrderedDictionary();
        _capacity = capacity;
    }

    public string Get(string key)
    {
        // checking if the dict containskey
        if (_dict.Count != 0 && _dict.Contains(key))
        {
            var val = (string)_dict[key];

            //updating the position because it has been used
            _dict.Remove(key);
            _dict.Add(key, val);

            return val;
        }
        return null;
    }

    public void Set(string key, string value)
    {
        // If contains Key
        if (!_dict.Contains(key))
        {
            // checking if capacity exceeded
            if (_dict.Count < _capacity)
            {
                _dict.Add(key, value);
            }
            else
            {
                // Capacity exceeded, delete last and insert at the beginning
                _dict.RemoveAt(0);
                _dict.Add(key, value);
            }
        }
        // Updating the position and value as it has been used
        else
        {
            _dict.Remove(key);
            _dict.Add(key, value);
        }
    }

    public void Remove(string key)
    {
        // checking if the dict containskey
        if (_dict.Count != 0 && _dict.Contains(key))
        {
            _dict.Remove(key);
        }
    }
}
