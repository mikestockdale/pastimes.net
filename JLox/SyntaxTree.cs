namespace Syterra.JLox;

public class SyntaxTree {
  public SyntaxTree(SymbolType type, Token token, params SyntaxTree[] children) {
    Type = type;
    this.token = token;
    Value = null;
    this.children = new List<SyntaxTree>(children);
  }

  public SyntaxTree(object? value, Token token) {
    Type = SymbolType.Terminal;
    Value = value;
    this.token = token;
    children = noChildren;
  }

  public int Line => token.Line;
  public SymbolType Type { get; }
  public object? Value { get; }
  
  public SyntaxTree Child(int index) { return children[index]; }

  public override string ToString() {
    return children.Count == 0
      ? token.Lexeme
      : $"({token.Lexeme} {string.Join(" ", children.Select(c => c.ToString()))})";
  }

  static readonly List<SyntaxTree> noChildren = new();
  readonly Token token;
  readonly List<SyntaxTree> children;
}