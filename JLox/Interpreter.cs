namespace Syterra.JLox; 

public class Interpreter {
  public Interpreter(Report report) {
    this.report = report;
  }
  public object? Interpret(SyntaxTree tree) {
    try {
      return rules[tree.Type](tree, this);
    }
    catch (RunTimeException error) {
      report.RunTimeError(error.Line, error.MessageText);
      return null;
    }
  }

  static readonly Dictionary<SymbolType, Func<SyntaxTree, Interpreter, object?>> rules = new() {
    { SymbolType.Terminal, EvalLiteral },
    { SymbolType.Not, EvalNot },
    { SymbolType.Negative, EvalNegative },
    { SymbolType.Add, EvalAdd },
    { SymbolType.Subtract, (tree, interpreter) => EvalBinary(tree, interpreter, (a, b) => a-b) },
    { SymbolType.Multiply, (tree, interpreter) => EvalBinary(tree, interpreter, (a, b) => a*b) },
    { SymbolType.Divide, (tree, interpreter) => EvalBinary(tree, interpreter, (a, b) => a/b) },
    { SymbolType.Greater, (tree, interpreter) => EvalBinary(tree, interpreter, (a, b) => a>b) },
    { SymbolType.GreaterEqual, (tree, interpreter) => EvalBinary(tree, interpreter, (a, b) => a>=b) },
    { SymbolType.Less, (tree, interpreter) => EvalBinary(tree, interpreter, (a, b) => a<b) },
    { SymbolType.LessEqual, (tree, interpreter) => EvalBinary(tree, interpreter, (a, b) => a<=b) },
    { SymbolType.Equal, (tree, interpreter) => AreEqual(tree, interpreter) },
    { SymbolType.NotEqual, (tree, interpreter) => !AreEqual(tree, interpreter) },
  };

  static object? EvalLiteral(SyntaxTree tree, Interpreter interpreter) {
    return tree.Value;
  }
  
  static object? EvalNot(SyntaxTree tree, Interpreter interpreter) {
    return !AsBoolean(interpreter.Interpret(tree.Child(0)));
  }

  static object? EvalNegative(SyntaxTree tree, Interpreter interpreter) {
    return -AsDouble(interpreter.Interpret(tree.Child(0)), tree.Line);
  }

  static object? EvalAdd(SyntaxTree tree, Interpreter interpreter) {
    var terms = EvalTerms(interpreter, tree);
    if (terms[0] is string string0 && terms[1] is string string1) return string0 + string1;
    if (terms[0] is double double0 && terms[1] is double double1) return double0 + double1;
    throw new RunTimeException("Operands must be two numbers or two strings", tree.Line);
  }
  
  static bool AreEqual(SyntaxTree tree, Interpreter interpreter) {
    var terms = EvalTerms(interpreter, tree);
    var term0 = terms[0];
    if (term0 == null) return terms[1] == null;
    return term0.Equals(terms[1]);
  }

  static object? EvalBinary<T>(SyntaxTree tree, Interpreter interpreter, Func<double, double, T> operation) {
    var terms = EvalTerms(interpreter, tree);
    return operation(AsDouble(terms[0], tree.Line), AsDouble(terms[1], tree.Line));
  }

  static object?[] EvalTerms(Interpreter interpreter, SyntaxTree tree) {
    return new[] { interpreter.Interpret(tree.Child(0)), interpreter.Interpret(tree.Child(1)) };
  }

  static double AsDouble(object? value, int line) {
    if (value is double doubleValue) return doubleValue;
    throw new RunTimeException("Operand must be a number", line);
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
    public RunTimeException(string messageText, int line) {
      MessageText = messageText;
      Line = line;
    }

    public string MessageText { get; }
    public int Line { get; }
  }
}