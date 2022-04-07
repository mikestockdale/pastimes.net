namespace Syterra.JLox;

public class RunTimeException : Exception {
  public RunTimeException(string messageText, int line) {
    MessageText = messageText;
    Line = line;
  }

  public string MessageText { get; }
  public int Line { get; }
}
