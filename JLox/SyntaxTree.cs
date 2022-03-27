namespace Syterra.JLox;

public class SyntaxTree {
  public SyntaxTree(Token token, params SyntaxTree[] children) {
    this.token = token;
    this.children = new List<SyntaxTree>(children);
  }

  public override string ToString() {
    return children.Count == 0
      ? token.Lexeme
      : $"({token.Lexeme} {string.Join(" ", children.Select(c => c.ToString()))})";
  }
  
  readonly Token token;
  readonly List<SyntaxTree> children;
}