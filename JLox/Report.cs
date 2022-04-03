namespace Syterra.JLox; 

public class Report {

  public Report(Action<string> write) {
    this.write = write;
    Reset();
  }

  public void Error(Token token, string message) {
    Write(
      token.Line,
      token.Type == TokenType.Eof ? " at end" : $" at '{token.Lexeme}'",
      message);
  }

  public void Error(int line, string message) {
    Write(line, string.Empty, message);
  }

  public void RunTimeError(int line, string message) {
    write($"[line {line}] Error: {message}");
    HadRunTimeError = true;
  }

  public void Reset() {
    HadError = false;
    HadRunTimeError = false;
  }

  public void Print(string message) {
    write(message);
  }
  
  public bool HadError { get; private set; }
  public bool HadRunTimeError { get; private set; }
  
  void Write(int line, string location, string message) {
    write($"[line {line}] Error{location}: {message}");
    HadError = true;
  }
  
  readonly Action<string> write;
}