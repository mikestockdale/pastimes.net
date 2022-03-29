namespace Syterra.JLox;

public class SyntaxTree {
  public SyntaxTree(Token token, params SyntaxTree[] children) {
    Token = token;
    this.children = new List<SyntaxTree>(children);
  }

  public SyntaxTree Child(int index) { return children[index]; }

  public int Count => children.Count;

  public override string ToString() {
    return children.Count == 0
      ? Token.Lexeme
      : $"({Token.Lexeme} {string.Join(" ", children.Select(c => c.ToString()))})";
  }
  
  public Token Token { get; }
  readonly List<SyntaxTree> children;
}