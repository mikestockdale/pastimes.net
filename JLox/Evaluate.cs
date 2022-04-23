namespace Syterra.JLox; 

public static class Evaluate {

  public static object? Add(SyntaxTree tree, Environment environment) {
    var terms = tree.EvaluateBranches(environment);
    if (terms[0] is string string0 && terms[1] is string string1) return string0 + string1;
    if (terms[0] is double double0 && terms[1] is double double1) return double0 + double1;
    throw new RunTimeException("Operands must be two numbers or two strings", tree.Line);
  }

  public static object? And(SyntaxTree tree, Environment environment) {
    var left = tree.EvaluateBranch(0, environment);
    return !AsBoolean(left) ? left : tree.EvaluateBranch(1, environment);
  }
  
  public static object? Assign(SyntaxTree tree, Environment environment) {
    return environment.Assign(tree.Branches[0].Token, tree.EvaluateBranch(1, environment));
  }

  public static object? Call(SyntaxTree tree, Environment environment) {
    if (tree.EvaluateBranch(0, environment) is not Func<object?[], object?> function)
      throw new RunTimeException("Can only call functions and classes", tree.Line);
    return function(tree.Branches[1].EvaluateBranches(environment));
  }
  
  public static object? Declare(SyntaxTree tree, Environment environment) {
    environment.Define(tree.Token.Lexeme, tree.Branches.Count > 0 ? tree.EvaluateBranch(0, environment) : null);
    return null;
  }
  
  public static object? Divide(SyntaxTree tree, Environment environment) {
    return EvalBinary(tree, environment, (a, b) => a/b);
  }
  
  public static object Equal(SyntaxTree tree, Environment environment) {
    return AreEqual(tree, environment);
  }

  public static object? For(SyntaxTree tree, Environment environment) {
    tree.EvaluateBranch(0, environment);
    while (AsBoolean(tree.EvaluateBranch(1, environment))) {
      tree.EvaluateBranch(3, environment);
      tree.EvaluateBranch(2, environment);
    }
    return null;
  }

  public static object? Function(SyntaxTree tree, Environment environment) {
    object? EvalFunction(object?[] args) {
      var names = tree.Branches[0].Branches.Select(b => b.Token.Lexeme).ToArray();
      var newEnvironment = new Environment(environment);
      for (var i = 0; i < args.Length; i++) {
        newEnvironment.Define(names[i], args[i]);
      }
      tree.Branches[1].EvaluateBranches(newEnvironment);
      return null;
    }

    environment.Define(tree.Token.Lexeme, EvalFunction);
    return null;
  }
  
  public static object? Greater(SyntaxTree tree, Environment environment) {
    return EvalBinary(tree, environment, (a, b) => a>b);
  }
  
  public static object? GreaterEqual(SyntaxTree tree, Environment environment) {
    return EvalBinary(tree, environment, (a, b) => a>=b);
  }
  
  public static object? If(SyntaxTree tree, Environment environment) {
    if (AsBoolean(tree.EvaluateBranch(0, environment))) {
      tree.EvaluateBranch(1, environment);
    }
    else if (tree.Branches.Count > 2) {
      tree.EvaluateBranch(2, environment);
    }
    return null;
  }
  
  public static object? Less(SyntaxTree tree, Environment environment) {
    return EvalBinary(tree, environment, (a, b) => a<b);
  }
  
  public static object? LessEqual(SyntaxTree tree, Environment environment) {
    return EvalBinary(tree, environment, (a, b) => a<=b);
  }
  
  public static object? List(SyntaxTree tree, Environment environment) {
    tree.EvaluateBranches(new Environment(environment));
    return null;
  }
  
  public static object? Literal(SyntaxTree tree, Environment environment) {
    return tree.Value;
  }
  
  public static object? Multiply(SyntaxTree tree, Environment environment) {
    return EvalBinary(tree, environment, (a, b) => a*b);
  }
  
  public static object? Negative(SyntaxTree tree, Environment environment) {
    return -AsDouble(tree.EvaluateBranch(0, environment), tree.Line);
  }

  public static object? Not(SyntaxTree tree, Environment environment) {
    return !AsBoolean(tree.EvaluateBranch(0, environment));
  }
  
  public static object? NotEqual(SyntaxTree tree, Environment environment) {
    return !AreEqual(tree, environment);
  }

  public static object? Or(SyntaxTree tree, Environment environment) {
    var left = tree.EvaluateBranch(0, environment);
    return AsBoolean(left) ? left : tree.EvaluateBranch(1, environment);
  }
  
  public static object? Subtract(SyntaxTree tree, Environment environment) {
    return EvalBinary(tree, environment, (a, b) => a-b);
  }
  
  public static object? Variable(SyntaxTree tree, Environment environment) {
    return environment.Get(tree.Token);
  }

  public static object? While(SyntaxTree tree, Environment environment) {
    while (AsBoolean(tree.EvaluateBranch(0, environment))) {
      tree.EvaluateBranch(1, environment);
    }
    return null;
  }
  
  static object? EvalBinary<T>(SyntaxTree tree, Environment environment, Func<double, double, T> operation) {
    var terms = tree.EvaluateBranches(environment);
    return operation(AsDouble(terms[0], tree.Line), AsDouble(terms[1], tree.Line));
  }

  static bool AreEqual(SyntaxTree tree, Environment environment) {
    var terms = tree.EvaluateBranches(environment);
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