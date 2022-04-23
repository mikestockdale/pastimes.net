namespace Syterra.JLox; 

public class Interpreter {
  public Interpreter(Report report) {
    this.report = report;
    Environment.Define("test", TestFunction);
    Environment.Define("print", PrintFunction);
  }

  object? PrintFunction(object?[] parameters) {
    report.Print(string.Join("", parameters.Select(p => p != null ? p.ToString() : "nil")));
    return null;
  }

  static object? TestFunction(object?[] parameters) {
    return parameters[0];
  }
  
  public object? Interpret(SyntaxTree tree) {
    try {
      return tree.Evaluate(this);
    }
    catch (RunTimeException error) {
      report.RunTimeError(error.Line, error.MessageText);
      return null;
    }
  }

  public void EvalBlock(Action action) {
    try {
      Environment = new Environment(Environment);
      action();
    }
    finally {
      Environment = Environment.Parent;
    }
  }

  public void Write(object? value) {
    report.Print(value != null ? value.ToString() ?? "nil" : "nil");
  }

  public Environment Environment { get; private set; } = new();
  
  readonly Report report;
}