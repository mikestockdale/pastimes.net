namespace Syterra.JLox;

public class SyntaxTree {
  public SyntaxTree(object? value, Token token) {
    rule = JLox.Evaluate.Literal;
    Value = value;
    Token = token;
    branches = noBranches;
  }
  
  public SyntaxTree(Func<SyntaxTree, Environment, object?> rule, Token token, params SyntaxTree[] branches) :
    this(rule, token, new List<SyntaxTree>(branches)) { }

  public SyntaxTree(Func<SyntaxTree, Environment, object?> rule, List<SyntaxTree> branches):
    this(rule, listToken, branches) { }

  public SyntaxTree(List<SyntaxTree> branches) :
    this(NoRule, branches) { }

  public Token Token { get; }
  public object? Value { get; }
  
  public IReadOnlyList<SyntaxTree> Branches => branches;
  public int Line => Token.Line;

  public object? Evaluate(Environment environment) {
    return rule(this, environment);
  }

  public object? EvaluateBranch(int branch, Environment environment) {
    return Branches[branch].Evaluate(environment);
  }

  public object?[] EvaluateBranches(Environment environment) {
    return branches.Select(c => c.Evaluate(environment)).ToArray();
  }

  public object? EvaluateBlock(Environment environment) {
    foreach (var result in branches.Select(branch => branch.Evaluate(environment))) {
      if (result is ReturnValue value) return value.Value;
    }
    return null;
  }

  public override string ToString() {
    return branches.Count == 0
      ? Name
      : $"({Name} {string.Join(" ", branches.Select(c => c.ToString()))})";
  }

  static readonly List<SyntaxTree> noBranches = new();
  static object? NoRule(SyntaxTree tree, Environment environment) { return null; }
  static readonly Token listToken = new(TokenType.Identifier, "List", 0);
  
  SyntaxTree(Func<SyntaxTree, Environment, object?> rule, Token token, List<SyntaxTree> branches) {
    this.rule = rule;
    Token = token;
    Value = null;
    this.branches = branches;
  }

  string Name => Token.Lexeme;

  readonly Func<SyntaxTree, Environment, object?> rule;
  readonly List<SyntaxTree> branches;
}