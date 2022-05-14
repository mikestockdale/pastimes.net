namespace Syterra.JLox;

public class ClassInstance {
  public ClassInstance(ClassDeclaration declaration) {
    this.declaration = declaration;
  }

  public object? Get(Token name) {
    if (fields.ContainsKey(name.Lexeme)) return fields[name.Lexeme];
    throw new RunTimeException($"Undefined property '{name.Lexeme}'", name.Line);
  }

  public void Set(Token name, object? value) {
    fields[name.Lexeme] = value;
  }

  public override string ToString() { return $"{declaration} instance"; }
 
  readonly ClassDeclaration declaration;
  readonly Dictionary<string, object?> fields = new();
}