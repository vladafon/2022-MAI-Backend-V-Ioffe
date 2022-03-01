using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProgramEngineering.Logic
{
    public class LRUCache
    {
        private int _capacity;
        private Dictionary<string, string> _cache;
        private List<KeyValuePair<string, string>> _logs;

        public LRUCache(int capacity = 10)
        {
            _capacity = capacity;
            _cache = new Dictionary<string, string>(_capacity);
            _logs = new List<KeyValuePair<string, string>>(_capacity);
        }

        public void Set(string key, string value)
        {
            if (_cache.Count == _capacity) // когда кеш заполнен, мы удаляем первый (самый старый) объект
            {
                var toRemove = _logs[0];
                _cache.Remove(toRemove.Key); 
                _logs.Remove(toRemove);
            }
            _logs.Add(new KeyValuePair<string,string>(key, value));
            _cache[key] = value;
        }

        public string Get(string key)
        {
            if (!_cache.ContainsKey(key))
            {
                return null;
            }

            //Опускаем запись вниз списка, делая ее более актуальной
            var tempCacheCell = _logs.FirstOrDefault(x => x.Key == key);
            _logs.Remove(tempCacheCell);
            _logs.Add(tempCacheCell);
            return _cache[key];
        }

        public void Remove(string key)
        {
            if (!_cache.ContainsKey(key))
            {
                return;
            }

            _cache.Remove(key);
            _logs.RemoveAll(c => c.Key == key);
        }


    }
}
