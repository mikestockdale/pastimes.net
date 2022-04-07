namespace Syterra.JLox; 

public class Environment {
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
    throw new RunTimeException($"Undefined variable '{name.Lexeme}'", name.Line);
  }
  readonly Dictionary<string, object?> values = new();
}