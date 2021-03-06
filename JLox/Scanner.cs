namespace Syterra.JLox;

public class Scanner {
  public Scanner(string source, Action<string> write) {
    this.source = source;
    this.write = write;
    HasErrors = false;
  }

  public IEnumerable<Token> ScanTokens() {
    while (!AtEnd()) {
      start = current;
      TokenRules.Apply(Advance(), this);
    }
    tokens.Add(new Token(TokenType.Eof, string.Empty, line));
    return tokens;
  }
  
  public bool HasErrors { get; private set; }

  public bool Match(char expected) {
    if (AtEnd()) return false;
    if (source[current] != expected) return false;
    Advance();
    return true;
  }

  public string AdvanceTo(char terminator) {
    while (!AtEnd() && Advance() != terminator) {}
    return source.Substring(start, current - start);
  }

  public string AdvanceWhile(Func<char, char, bool> criteria) {
    while (!AtEnd() && criteria(source[current], Peek())) Advance();
    return source.Substring(start, current - start);
  }

  public void AddToken(TokenType type) {
    tokens.Add(new Token(type, source.Substring(start, current - start), line));
  }

  public void AddToken(TokenType type, object? literal) {
    tokens.Add(new Token(type, source.Substring(start, current - start), literal, line));
  }

  public void AddError(string message) {
    HasErrors = true;
    write(Report.Message(line, message));
  }

  char Advance() {
    var result = source[current++];
    if (result == '\n') line++;
    return result;
  }

  char Peek() {
    return current + 1 < source.Length ? source[current + 1] : '\0';
  }

  bool AtEnd() {
    return current >= source.Length;
  }

  readonly string source;
  readonly Action<string> write;
  readonly List<Token> tokens = new();
  
  int current;
  int start;
  int line = 1;
}