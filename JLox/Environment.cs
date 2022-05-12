namespace Syterra.JLox;

public class Environment {
  public Environment() {
    parent = this;
  }

  public Environment(Environment parent, int parentVersion = -1) {
    this.parent = parent;
    this.parentVersion = parentVersion;
  }

  public int Count => values.Count;
  
  public void Define(string name, object? value) {
    var item = values.FindIndex(t => t.Name == name);
    if (item != -1) {
      values[item] = new Entry(name, value);
    }
    else {
      values.Add(new Entry(name, value));
    }
  }

  public object? Get(Token name, int version = -1) {
    var item = FindIndex(name.Lexeme, version);
    if (item != -1) return values[item].Value;
    if (parent != this) return parent.Get(name, parentVersion);
    throw new RunTimeException($"Undefined variable '{name.Lexeme}'", name.Line);
  }

  public object? Assign(Token name, object? value, int version = -1) {
    var item = FindIndex(name.Lexeme, version);
    if (item != -1) {
      values[item] = new Entry(name.Lexeme, value);
      return value;
    }
    if (parent != this) return parent.Assign(name, value, parentVersion);
    throw new RunTimeException($"Undefined variable '{name.Lexeme}'", name.Line);
  }

  int FindIndex(string name, int version) {
    var max = version == -1 ? values.Count : version;
    for (var i = 0; i < max; i++) {
      if (values[i].Name == name) return i;
    }
    return -1;
  }

  readonly Environment parent;
  readonly int parentVersion;
  readonly List<Entry> values = new();

  readonly record struct Entry(string Name, object? Value);
}