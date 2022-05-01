namespace Syterra.JLox; 

public interface Callable {
  int Arity { get; }
  object? Call(object?[] arguments);
}

public class FunctionCall : Callable {
  public FunctionCall(Environment environment, string[] names, SyntaxTree body) {
    this.environment = environment;
    this.names = names;
    this.body = body;
  }

  public int Arity => names.Length;

  public object? Call(object?[] arguments) {
    for (var i = 0; i < arguments.Length; i++) {
      environment.Define(names[i], arguments[i]);
    }
    return body.EvaluateBlock(environment) is ReturnValue value ? value.Value : null;
  }

  readonly string[] names;
  readonly Environment environment;
  readonly SyntaxTree body;
}

public class NativeCall : Callable {
  public NativeCall(Func<object?[], object?> function, int arity) {
    this.function = function;
    Arity = arity;
  }

  public int Arity { get; }

  public object? Call(object?[] arguments) {
    return function(arguments);
  }
  
  readonly Func<object?[], object?> function;
}