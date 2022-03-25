namespace Syterra.JLox;

public class Token {
  public Token(TokenType type, string lexeme, object? literal, int line) {
    this.type = type;
    this.lexeme = lexeme;
    this.literal = literal;
    this.line = line;
  }

  public Token(TokenType type, string lexeme, int line): this(type, lexeme, null, line) { }

  public override string ToString() {
    return $"{type} {lexeme} {literal}";
  }

  readonly TokenType type;
  readonly string lexeme;
  readonly object? literal;
  readonly int line;
}