namespace Syterra.JLox; 

public interface Callable {
  object? Call(object?[] parameters);
}

public class FunctionCall : Callable {
  public FunctionCall(Environment environment, string[] names, SyntaxTree body) {
    this.environment = environment;
    this.names = names;
    this.body = body;
  }
  
  public object? Call(object?[] parameters) {
    for (var i = 0; i < parameters.Length; i++) {
      environment.Define(names[i], parameters[i]);
    }
    return body.EvaluateBlock(environment);
  }

  readonly string[] names;
  readonly Environment environment;
  readonly SyntaxTree body;
}

public class NativeCall : Callable {
  public object? Call(object?[] parameters) {
    return function(parameters);
  }
  readonly Func<object?[], object?> function;

  public NativeCall(Func<object?[], object?> function) {
    this.function = function;
  }
}