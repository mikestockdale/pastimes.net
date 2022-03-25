namespace Syterra.JLox;

public class Scanner {
  public Scanner(string source) {
    this.source = source;
  }

  public IEnumerable<Token> ScanTokens() {
    while (!AtEnd()) {
      start = current;
      TokenRules.Apply(Advance(), this);
    }
    tokens.Add(new Token(TokenType.Eof, string.Empty, line));
    return tokens;
  }

  public bool Match(char expected) {
    if (AtEnd()) return false;
    if (source[current] != expected) return false;
    current++;
    return true;
  }

  public void AdvanceTo(char terminator) {
    while (!AtEnd() && Peek() != terminator) Advance();
  }

  public void AddToken(TokenType type) {
    tokens.Add(new Token(type, source.Substring(start, current - start), line));
  }

  public void AddError(string message) {
    tokens.Add(new Token(TokenType.Error, message, line));
  }

  char Peek() {
    return AtEnd() ? '\0' : source[current];
  }

  char Advance() {
    return source[current++];
  }

  bool AtEnd() {
    return current >= source.Length;
  }

  readonly string source;
  readonly List<Token> tokens = new();
  
  int current = 0;
  int start;
  int line = 1;
}