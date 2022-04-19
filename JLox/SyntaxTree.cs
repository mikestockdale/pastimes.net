namespace Syterra.JLox;

public class SyntaxTree {
  public SyntaxTree(Func<SyntaxTree, Interpreter, object?> rule, Token token, params SyntaxTree[] children) {
    this.rule = rule;
    Token = token;
    Value = null;
    this.children = new List<SyntaxTree>(children);
  }

  public SyntaxTree(object? value, Token token) {
    rule = JLox.Evaluate.Literal;
    Value = value;
    Token = token;
    children = noChildren;
  }

  public SyntaxTree(Func<SyntaxTree, Interpreter, object?> rule, List<SyntaxTree> children) {
    this.rule = rule;
    Value = null;
    Token = listToken;
    this.children = children;
  }
  
  public Token Token { get; }
  public object? Value { get; }
  
  public IReadOnlyList<SyntaxTree> Children => children;
  public int Line => Token.Line;

  public object? Evaluate(Interpreter interpreter) {
    return rule(this, interpreter);
  }

  public override string ToString() {
    return children.Count == 0
      ? Name
      : $"({Name} {string.Join(" ", children.Select(c => c.ToString()))})";
  }

  static readonly List<SyntaxTree> noChildren = new();
  static readonly Token listToken = new(TokenType.Identifier, "List", 0);
  
  string Name => Token.Lexeme;

  readonly Func<SyntaxTree, Interpreter, object?> rule;
  readonly List<SyntaxTree> children;
}