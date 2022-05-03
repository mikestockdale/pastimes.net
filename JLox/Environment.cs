namespace Syterra.JLox;

public class Environment {
  public Environment() {
    parent = this;
  }

  public Environment(Environment parent) {
    this.parent = parent;
  }

  
  public void Define(string name, object? value) {
    if (values.ContainsKey(name)) {
      values[name] = value;
    }
    else {
      values.Add(name, value);
    }
  }

  public object? Get(Token name) {
    if (values.ContainsKey(name.Lexeme)) return values[name.Lexeme];
    if (parent != this) return parent.Get(name);
    throw new RunTimeException($"Undefined variable '{name.Lexeme}'", name.Line);
  }

  public object? Assign(Token name, object? value) {
    if (values.ContainsKey(name.Lexeme)) {
      values[name.Lexeme] = value;
      return value;
    }
    if (parent != this) return parent.Assign(name, value);
    throw new RunTimeException($"Undefined variable '{name.Lexeme}'", name.Line);
  }

  readonly Environment parent;
  readonly Dictionary<string, object?> values = new();
}