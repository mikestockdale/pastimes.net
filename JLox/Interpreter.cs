namespace Syterra.JLox; 

public class Interpreter {
  public Interpreter(Report report) {
    this.report = report;
  }
  
  public object? Interpret(SyntaxTree tree) {
    try {
      return Evaluate(tree);
    }
    catch (RunTimeException error) {
      report.RunTimeError(error.Line, error.MessageText);
      return null;
    }
  }

  object? Evaluate(SyntaxTree tree) {
    return rules[tree.Type](tree, this);
  }

  void Write(object? value) {
    report.Print(value != null ? value.ToString() ?? "nil" : "nil");
  }

  static readonly Dictionary<SymbolType, Func<SyntaxTree, Interpreter, object?>> rules = new() {
    { SymbolType.Literal, EvalLiteral },
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
    { SymbolType.List, EvalList },
    { SymbolType.Print, EvalPrint },
    { SymbolType.Declare , EvalDeclare },
    { SymbolType.Variable, EvalVariable }
  };

  static object? EvalVariable(SyntaxTree tree, Interpreter interpreter) {
    return interpreter.environment.Get(tree.Token);
  }

  static object? EvalDeclare(SyntaxTree tree, Interpreter interpreter) {
    interpreter.environment.Define(tree.Token.Lexeme, tree.Children.Count > 0 ? interpreter.Evaluate(tree.Children[0]) : null);
    return null;
  }

  static object? EvalList(SyntaxTree tree, Interpreter interpreter) {
    foreach (var child in tree.Children) interpreter.Evaluate(child);
    return null;
  }

  static object? EvalPrint(SyntaxTree tree, Interpreter interpreter) {
    var result = interpreter.Evaluate(tree.Children[0]);
    interpreter.Write(result);
    return null;
  }

  static object? EvalLiteral(SyntaxTree tree, Interpreter interpreter) {
    return tree.Value;
  }
  
  static object? EvalNot(SyntaxTree tree, Interpreter interpreter) {
    return !AsBoolean(interpreter.Evaluate(tree.Children[0]));
  }

  static object? EvalNegative(SyntaxTree tree, Interpreter interpreter) {
    return -AsDouble(interpreter.Evaluate(tree.Children[0]), tree.Line);
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
    return new[] { interpreter.Evaluate(tree.Children[0]), interpreter.Evaluate(tree.Children[1]) };
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

  readonly Environment environment = new();
  readonly Report report;
}