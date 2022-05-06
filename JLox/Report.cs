namespace Syterra.JLox; 

public static class Report {
  public static string Message(int line, string message) {
    return $"[line {line}] Error: {message}";
  }

  public static string Message(Token token, string message) {
    var location = token.Type == TokenType.Eof ? "end" : $"'{token.Lexeme}'";
    return $"[line {token.Line}] Error at {location}: {message}";
  }
}