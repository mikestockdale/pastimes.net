namespace Syterra.JLox;

public class SyntaxTree {
  public SyntaxTree(object? value, Token token) {
    rule = JLox.Evaluate.Literal;
    Value = value;
    Token = token;
    children = noChildren;
  }
  
  public SyntaxTree(Func<SyntaxTree, Interpreter, object?> rule, Token token, params SyntaxTree[] children) :
    this(rule, token, new List<SyntaxTree>(children)) { }

  public SyntaxTree(Func<SyntaxTree, Interpreter, object?> rule, List<SyntaxTree> children):
    this(rule, listToken, children) { }

  public SyntaxTree(List<SyntaxTree> children) :
    this(NoRule, children) { }

  public Token Token { get; }
  public object? Value { get; }
  
  public IReadOnlyList<SyntaxTree> Children => children;
  public int Line => Token.Line;

  public object? Evaluate(Interpreter interpreter) {
    return rule(this, interpreter);
  }

  public object? EvaluateChild(int child, Interpreter interpreter) {
    return Children[child].Evaluate(interpreter);
  }

  public object?[] EvaluateChildren(Interpreter interpreter) {
    return children.Select(c => c.Evaluate(interpreter)).ToArray();
  }

  public override string ToString() {
    return children.Count == 0
      ? Name
      : $"({Name} {string.Join(" ", children.Select(c => c.ToString()))})";
  }

  static readonly List<SyntaxTree> noChildren = new();
  static object? NoRule(SyntaxTree tree, Interpreter interpreter) { return null; }
  static readonly Token listToken = new(TokenType.Identifier, "List", 0);
  
  SyntaxTree(Func<SyntaxTree, Interpreter, object?> rule, Token token, List<SyntaxTree> children) {
    this.rule = rule;
    Token = token;
    Value = null;
    this.children = children;
  }

  string Name => Token.Lexeme;

  readonly Func<SyntaxTree, Interpreter, object?> rule;
  readonly List<SyntaxTree> children;
}