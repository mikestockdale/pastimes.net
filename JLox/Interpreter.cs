using System.Diagnostics;

namespace Syterra.JLox; 

public class Interpreter {
  public Interpreter(Action<string> write) {
    this.write = write;
  }

  public Optional<object?> Interpret(SyntaxTree tree) {
    return Interpret(tree, _ => { });
  }

  public Optional<object?> Interpret(SyntaxTree tree, Action<Environment> testAction) {
    try {
      var environment = new Environment();
      environment.Define("clock", new NativeCall(ClockFunction, 0));
      environment.Define("print", new NativeCall(PrintFunction, -1));
      stopwatch.Start();
      testAction(environment);
      return Optional<object?>.Of(tree.Evaluate(environment));
    }
    catch (RunTimeException error) {
      write(error.Message);
      return Optional<object?>.Empty;
    }
  }

  object? PrintFunction(object?[] parameters) {
    write(string.Join("", parameters.Select(p => p != null ? p.ToString() : "nil")));
    return null;
  }

  static object ClockFunction(object?[] parameters) {
    return stopwatch.Elapsed.TotalMilliseconds;
  }

  static readonly Stopwatch stopwatch = new();
  readonly Action<string> write;
}