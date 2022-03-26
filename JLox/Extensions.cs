namespace Syterra.JLox; 

public static class Extensions {
  public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue) {
    return dictionary.TryGetValue(key, out var result) ? result : defaultValue;
  }
}