namespace Syterra.JLox;

public class RunTimeException : Exception {
  public RunTimeException(string messageText, int line) {
    this.messageText = messageText;
    this.line = line;
  }

  public override string Message => Report.Message(line, messageText);
  
  readonly string messageText;
  readonly int line;
}
