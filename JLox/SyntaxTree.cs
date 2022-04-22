namespace Syterra.JLox;

public class SyntaxTree {
  public SyntaxTree(object? value, Token token) {
    rule = JLox.Evaluate.Literal;
    Value = value;
    Token = token;
    branches = noBranches;
  }
  
  public SyntaxTree(Func<SyntaxTree, Interpreter, object?> rule, Token token, params SyntaxTree[] branches) :
    this(rule, token, new List<SyntaxTree>(branches)) { }

  public SyntaxTree(Func<SyntaxTree, Interpreter, object?> rule, List<SyntaxTree> branches):
    this(rule, listToken, branches) { }

  public SyntaxTree(List<SyntaxTree> branches) :
    this(NoRule, branches) { }

  public Token Token { get; }
  public object? Value { get; }
  
  public IReadOnlyList<SyntaxTree> Branches => branches;
  public int Line => Token.Line;

  public object? Evaluate(Interpreter interpreter) {
    return rule(this, interpreter);
  }

  public object? EvaluateBranch(int branch, Interpreter interpreter) {
    return Branches[branch].Evaluate(interpreter);
  }

  public object?[] EvaluateBranches(Interpreter interpreter) {
    return branches.Select(c => c.Evaluate(interpreter)).ToArray();
  }

  public override string ToString() {
    return branches.Count == 0
      ? Name
      : $"({Name} {string.Join(" ", branches.Select(c => c.ToString()))})";
  }

  static readonly List<SyntaxTree> noBranches = new();
  static object? NoRule(SyntaxTree tree, Interpreter interpreter) { return null; }
  static readonly Token listToken = new(TokenType.Identifier, "List", 0);
  
  SyntaxTree(Func<SyntaxTree, Interpreter, object?> rule, Token token, List<SyntaxTree> branches) {
    this.rule = rule;
    Token = token;
    Value = null;
    this.branches = branches;
  }

  string Name => Token.Lexeme;

  readonly Func<SyntaxTree, Interpreter, object?> rule;
  readonly List<SyntaxTree> branches;
}