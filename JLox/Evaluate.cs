namespace Syterra.JLox; 

public static class Evaluate {

  public static object? Add(SyntaxTree tree, Interpreter interpreter) {
    var terms = EvalTerms(interpreter, tree);
    if (terms[0] is string string0 && terms[1] is string string1) return string0 + string1;
    if (terms[0] is double double0 && terms[1] is double double1) return double0 + double1;
    throw new RunTimeException("Operands must be two numbers or two strings", tree.Line);
  }

  public static object? And(SyntaxTree tree, Interpreter interpreter) {
    var left = tree.Children[0].Evaluate(interpreter);
    return !AsBoolean(left) ? left : tree.Children[1].Evaluate(interpreter);
  }
  
  public static object? Assign(SyntaxTree tree, Interpreter interpreter) {
    return interpreter.Environment.Assign(tree.Children[0].Token, tree.Children[1].Evaluate(interpreter));
  }
  
  public static object? Declare(SyntaxTree tree, Interpreter interpreter) {
    interpreter.Environment.Define(tree.Token.Lexeme, tree.Children.Count > 0 ? tree.Children[0].Evaluate(interpreter) : null);
    return null;
  }
  
  public static object? Divide(SyntaxTree tree, Interpreter interpreter) {
    return EvalBinary(tree, interpreter, (a, b) => a/b);
  }
  
  public static object Equal(SyntaxTree tree, Interpreter interpreter) {
    return AreEqual(tree, interpreter);
  }
  
  public static object? Greater(SyntaxTree tree, Interpreter interpreter) {
    return EvalBinary(tree, interpreter, (a, b) => a>b);
  }
  
  public static object? GreaterEqual(SyntaxTree tree, Interpreter interpreter) {
    return EvalBinary(tree, interpreter, (a, b) => a>=b);
  }
  
  public static object? If(SyntaxTree tree, Interpreter interpreter) {
    if (AsBoolean(tree.Children[0].Evaluate(interpreter))) {
      tree.Children[1].Evaluate(interpreter);
    }
    else if (tree.Children.Count > 2) {
      tree.Children[2].Evaluate(interpreter);
    }
    return null;
  }
  
  public static object? Less(SyntaxTree tree, Interpreter interpreter) {
    return EvalBinary(tree, interpreter, (a, b) => a<b);
  }
  
  public static object? LessEqual(SyntaxTree tree, Interpreter interpreter) {
    return EvalBinary(tree, interpreter, (a, b) => a<=b);
  }
  
  public static object? List(SyntaxTree tree, Interpreter interpreter) {
    interpreter.EvalBlock(() => {
      foreach (var child in tree.Children) child.Evaluate(interpreter);
    });
    return null;
  }
  
  public static object? Literal(SyntaxTree tree, Interpreter interpreter) {
    return tree.Value;
  }
  
  public static object? Multiply(SyntaxTree tree, Interpreter interpreter) {
    return EvalBinary(tree, interpreter, (a, b) => a*b);
  }
  
  public static object? Negative(SyntaxTree tree, Interpreter interpreter) {
    return -AsDouble(tree.Children[0].Evaluate(interpreter), tree.Line);
  }

  public static object? Not(SyntaxTree tree, Interpreter interpreter) {
    return !AsBoolean(tree.Children[0].Evaluate(interpreter));
  }
  
  public static object? NotEqual(SyntaxTree tree, Interpreter interpreter) {
    return !AreEqual(tree, interpreter);
  }

  public static object? Or(SyntaxTree tree, Interpreter interpreter) {
    var left = tree.Children[0].Evaluate(interpreter);
    return AsBoolean(left) ? left : tree.Children[1].Evaluate(interpreter);
  }
  
  public static object? Print(SyntaxTree tree, Interpreter interpreter) {
    var result = tree.Children[0].Evaluate(interpreter);
    interpreter.Write(result);
    return null;
  }
  
  public static object? Subtract(SyntaxTree tree, Interpreter interpreter) {
    return EvalBinary(tree, interpreter, (a, b) => a-b);
  }
  
  public static object? Variable(SyntaxTree tree, Interpreter interpreter) {
    return interpreter.Environment.Get(tree.Token);
  }

  public static object? While(SyntaxTree tree, Interpreter interpreter) {
    while (AsBoolean(tree.Children[0].Evaluate(interpreter))) {
      tree.Children[1].Evaluate(interpreter);
    }
    return null;
  }
  
  static object? EvalBinary<T>(SyntaxTree tree, Interpreter interpreter, Func<double, double, T> operation) {
    var terms = EvalTerms(interpreter, tree);
    return operation(AsDouble(terms[0], tree.Line), AsDouble(terms[1], tree.Line));
  }


  static object?[] EvalTerms(Interpreter interpreter, SyntaxTree tree) {
    return new[] { tree.Children[0].Evaluate(interpreter), tree.Children[1].Evaluate(interpreter) };
  }
  
  static bool AreEqual(SyntaxTree tree, Interpreter interpreter) {
    var terms = EvalTerms(interpreter, tree);
    var term0 = terms[0];
    if (term0 == null) return terms[1] == null;
    return term0.Equals(terms[1]);
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
}