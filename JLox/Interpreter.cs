using System.Diagnostics;

namespace Syterra.JLox; 

public class Interpreter {
  public Interpreter(Report report) {
    this.report = report;
  }
  
  public object? Interpret(SyntaxTree tree) {
    try {
      var environment = new Environment();
      environment.Define("test", new NativeCall(TestFunction, 1));
      environment.Define("clock", new NativeCall(ClockFunction, 0));
      environment.Define("print", new NativeCall(PrintFunction, -1));
      stopwatch.Start();
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

  static object ClockFunction(object?[] parameters) {
    if (parameters.Length > 0) throw new RunTimeException("bad", 0);
    return stopwatch.Elapsed.TotalMilliseconds;
  }

  static readonly Stopwatch stopwatch = new();
  readonly Report report;
}