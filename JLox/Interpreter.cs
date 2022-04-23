namespace Syterra.JLox; 

public class Interpreter {
  public Interpreter(Report report) {
    this.report = report;
  }
  
  public object? Interpret(SyntaxTree tree) {
    try {
      var environment = new Environment();
      environment.Define("test", TestFunction);
      environment.Define("print", PrintFunction);
      return tree.Evaluate(environment);
    }
    catch (RunTimeException error) {
      report.RunTimeError(error.Line, error.MessageText);
      return null;
    }
  }

  object? PrintFunction(object?[] parameters) {
    report.Print(string.Join("", parameters.Select(p => p != null ? p.ToString() : "nil")));
    return null;
  }

  static object? TestFunction(object?[] parameters) {
    return parameters[0];
  }

  readonly Report report;
}