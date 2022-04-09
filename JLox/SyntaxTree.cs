namespace Syterra.JLox;

public class SyntaxTree {
  public SyntaxTree(SymbolType type, Token token, params SyntaxTree[] children) {
    Type = type;
    Token = token;
    Value = null;
    this.children = new List<SyntaxTree>(children);
  }

  public SyntaxTree(object? value, Token token) {
    Type = SymbolType.Literal;
    Value = value;
    Token = token;
    children = noChildren;
  }

  public SyntaxTree(SymbolType type, List<SyntaxTree> children) {
    Type = type;
    Value = null;
    Token = listToken;
    this.children = children;
  }
  
  public Token Token { get; }
  public int Line => Token.Line;
  public SymbolType Type { get; }
  public object? Value { get; }
  public IReadOnlyList<SyntaxTree> Children => children;

  public override string ToString() {
    return children.Count == 0
      ? Name
      : $"({Name} {string.Join(" ", children.Select(c => c.ToString()))})";
  }

  string Name => Token.Lexeme;

  static readonly List<SyntaxTree> noChildren = new();
  static readonly Token listToken = new(TokenType.Identifier, "List", 0);
  
  readonly List<SyntaxTree> children;
}