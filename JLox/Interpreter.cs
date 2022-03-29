namespace Syterra.JLox; 

public class Interpreter {
  public object? Interpret(SyntaxTree tree) {
    return rules[tree.Token.Type](tree, this);
  }

  static readonly Dictionary<TokenType, Func<SyntaxTree, Interpreter, object?>> rules = new() {
    { TokenType.Number, EvalLiteral },
    { TokenType.String, EvalLiteral },
    { TokenType.False, (_,_) => false },
    { TokenType.True, (_,_) => true },
    { TokenType.Nil, (_,_) => null },
    { TokenType.Not, EvalNot },
    { TokenType.Minus, EvalMinus }
  };

  static object? EvalLiteral(SyntaxTree tree, Interpreter interpeter) {
    return tree.Token.Literal;
  }
  
  static object? EvalNot(SyntaxTree tree, Interpreter interpreter) {
    return !IsTruthy(interpreter.Interpret(tree.Child(0)));
  }

  static object? EvalMinus(SyntaxTree tree, Interpreter interpreter) {
    return -(double)interpreter.Interpret(tree.Child(0));
  }

  static bool IsTruthy(object? value) {
    return value switch {
      null => false,
      bool booleanValue => booleanValue,
      _ => true
    };
  }
}