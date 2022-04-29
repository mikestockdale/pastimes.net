namespace Syterra.JLox; 

public interface Callable {
  object? Call(object?[] arguments);
}

public class FunctionCall : Callable {
  public FunctionCall(Environment environment, Token token, string[] names, SyntaxTree body) {
    this.environment = environment;
    this.token = token;
    this.names = names;
    this.body = body;
  }
  
  public object? Call(object?[] arguments) {
    if (arguments.Length != names.Length) {
      throw new RunTimeException($"Expected {names.Length} arguments but got {arguments.Length}", token.Line);
    }
    for (var i = 0; i < arguments.Length; i++) {
      environment.Define(names[i], arguments[i]);
    }
    return body.EvaluateBlock(environment) is ReturnValue value ? value.Value : null;
  }

  readonly string[] names;
  readonly Environment environment;
  readonly Token token;
  readonly SyntaxTree body;
}

public class NativeCall : Callable {
  public object? Call(object?[] arguments) {
    return function(arguments);
  }
  readonly Func<object?[], object?> function;

  public NativeCall(Func<object?[], object?> function) {
    this.function = function;
  }
}