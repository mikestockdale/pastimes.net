namespace Syterra.JLox; 

public class Environment {
  public Environment() {
    Parent = this;
  }

  public Environment(Environment parent) {
    Parent = parent;
  }
  
  public Environment Parent { get;  }
  
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
    if (Parent != this) return Parent.Get(name);
    throw new RunTimeException($"Undefined variable '{name.Lexeme}'", name.Line);
  }

  public object? Assign(Token name, object? value) {
    if (values.ContainsKey(name.Lexeme)) {
      values[name.Lexeme] = value;
      return value;
    }
    if (Parent != this) return Parent.Assign(name, value);
    throw new RunTimeException($"Undefined variable '{name.Lexeme}'", name.Line);
  }

  readonly Dictionary<string, object?> values = new();
}