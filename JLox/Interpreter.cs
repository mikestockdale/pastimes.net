namespace Syterra.JLox; 

public class Interpreter {
  public Interpreter(Report report) {
    this.report = report;
  }
  public object? Interpret(SyntaxTree tree) {
    try {
      return rules[tree.Token.Type](tree, this);
    }
    catch (RunTimeException error) {
      report.RunTimeError(error.Token.Line, error.MessageText);
      return null;
    }
  }

  static readonly Dictionary<TokenType, Func<SyntaxTree, Interpreter, object?>> rules = new() {
    { TokenType.Number, EvalLiteral },
    { TokenType.String, EvalLiteral },
    { TokenType.False, (_,_) => false },
    { TokenType.True, (_,_) => true },
    { TokenType.Nil, (_,_) => null },
    { TokenType.Not, EvalNot },
    { TokenType.Plus, EvalPlus },
    { TokenType.Minus, EvalMinus },
    { TokenType.Star, EvalMultiply },
    { TokenType.Slash, EvalDivide }
  };

  static object? EvalLiteral(SyntaxTree tree, Interpreter interpeter) {
    return tree.Token.Literal;
  }
  
  static object? EvalNot(SyntaxTree tree, Interpreter interpreter) {
    return !AsBoolean(interpreter.Interpret(tree.Child(0)));
  }

  static object? EvalPlus(SyntaxTree tree, Interpreter interpreter) {
    return EvalBinary(tree, interpreter, (a, b) => a+b);
  }

  static object? EvalMinus(SyntaxTree tree, Interpreter interpreter) {
    return tree.Count == 1
      ? -AsDouble(interpreter.Interpret(tree.Child(0)), tree.Token)
      : EvalBinary(tree, interpreter, (a, b) => a-b);
  }

  static object? EvalMultiply(SyntaxTree tree, Interpreter interpreter) {
    return EvalBinary(tree, interpreter, (a, b) => a*b);
  }

  static object? EvalDivide(SyntaxTree tree, Interpreter interpreter) {
    return EvalBinary(tree, interpreter, (a, b) => a/b);
  }

  static object? EvalBinary(SyntaxTree tree, Interpreter interpreter, Func<double, double, double?> operation) {
    return operation(
      AsDouble(interpreter.Interpret(tree.Child(0)), tree.Token),
      AsDouble(interpreter.Interpret(tree.Child(1)), tree.Token));
  }

  static double AsDouble(object? value, Token token) {
    if (value is double doubleValue) return doubleValue;
    throw new RunTimeException("Operand must be a number", token);
  }

  static bool AsBoolean(object? value) {
    return value switch {
      null => false,
      bool booleanValue => booleanValue,
      _ => true
    };
  }
  
  readonly Report report;

  class RunTimeException : Exception {
    public RunTimeException(string messageText, Token token) {
      MessageText = messageText;
      Token = token;
    }

    public string MessageText { get; }
    public Token Token { get; }
  }
}