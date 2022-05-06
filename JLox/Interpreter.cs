namespace Syterra.JLox; 

public class Interpreter {
  public Interpreter(Platform platform) {
    this.platform = platform;
  }

  public Optional<object?> Interpret(SyntaxTree tree) {
    try {
      var environment = new Environment();
      environment.Define("clock", new NativeCall(_ => platform.Elapsed, 0));
      environment.Define("print", new NativeCall(PrintFunction, -1));
      platform.Start();
      return Optional<object?>.Of(tree.Evaluate(environment));
    }
    catch (RunTimeException error) {
      platform.Write(error.Message);
      return Optional<object?>.Empty;
    }
  }

  object? PrintFunction(object?[] parameters) {
    platform.Write(string.Join("", parameters.Select(p => p != null ? p.ToString() : "nil")));
    return null;
  }

  readonly Platform platform;
}