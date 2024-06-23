using SaveUp.Common;
using SaveUp.Common.Helpers;

namespace SaveUp.Tests.Util.Helper
{
    public class FakePreferences : IPreferences
    {
        private Dictionary<(string Key, string SharedName), object> _preferences = new Dictionary<(string, string), object>();

        public void Clear(string? sharedName = null)
        {
            var keysToRemove = _preferences.Keys.Where(k => k.SharedName == sharedName).ToList();
            foreach (var key in keysToRemove)
            {
                _preferences.Remove(key);
            }
        }

        public bool ContainsKey(string key, string? sharedName = null)
        {
            return _preferences.ContainsKey((key, sharedName));
        }

        public void Remove(string key, string? sharedName = null)
        {
            _preferences.Remove((key, sharedName));
        }

        public void Set<T>(string key, T value, string? sharedName = null)
        {
            _preferences[(key, sharedName)] = value;
        }

        public T Get<T>(string key, T defaultValue, string? sharedName = null)
        {
            if (_preferences.TryGetValue((key, sharedName), out var value))
            {
                return (T)value;
            }
            return defaultValue;
        }
    }
}
