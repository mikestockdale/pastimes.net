namespace Syterra.JLox;

public class SyntaxTree {
  public SyntaxTree(SymbolType type, Token? token, params SyntaxTree[] children) {
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

  public SyntaxTree(SymbolType type, List<SyntaxTree> children) {
    Type = type;
    Value = null;
    token = null;
    this.children = children;
  }
  
  public int Line => token?.Line ?? 0;
  public SymbolType Type { get; }
  public object? Value { get; }
  public IReadOnlyList<SyntaxTree> Children => children;

  public override string ToString() {
    return children.Count == 0
      ? Name
      : $"({Name} {string.Join(" ", children.Select(c => c.ToString()))})";
  }

  string Name => token?.Lexeme ?? Type.ToString();

  static readonly List<SyntaxTree> noChildren = new();
  readonly Token? token;
  readonly List<SyntaxTree> children;
}