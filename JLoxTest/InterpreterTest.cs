using System.Collections.Generic;
using NUnit.Framework;

namespace Syterra.JLox.Test; 

[TestFixture]
public class InterpreterTest {
  
  [TestCase(12.3, "12.3")]
  [TestCase("abc", "\"abc\"")]
  [TestCase(false, "false")]
  [TestCase(true, "true")]
  [TestCase(null, "nil")]
  public void Literals(object? expected, string input) {
    AssertInterprets(expected, input);
  }

  [TestCase(true, "!false")]
  [TestCase(true, "!nil")]
  [TestCase(false, "!true")]
  [TestCase(false, "!\"abc\"")]
  [TestCase(-12.3, "-12.3")]
  public void Unaries(object? expected, string input) {
    AssertInterprets(expected, input);
  }
  
  [TestCase(579, "456+123")]
  [TestCase(333, "456-123")]
  [TestCase(1.23, "123*0.01")]
  [TestCase(228, "456/2")]
  public void Arithmetic(object? expected, string input) {
    AssertInterprets(expected, input);
  }

  [TestCase("[line 1] Error: Operand must be a number", "-nil")]
  [TestCase("[line 1] Error: Operand must be a number", "\"abc\"+123")]
  [TestCase("[line 1] Error: Operand must be a number", "123-nil")]
  [TestCase("[line 1] Error: Operand must be a number", "123*\"abc\"")]
  [TestCase("[line 1] Error: Operand must be a number", "\"cde\"/\"abc\"")]
  public void Errors(string expected, string input) {
    errors.Clear();
    var tree = new Parser(new Scanner(input, report).ScanTokens(), report).Parse();
    if (tree == null) Assert.Fail();
    else {
      var result = new Interpreter(report).Interpret(tree);
      Assert.IsNull(result);
      Assert.AreEqual(expected, string.Join(";", errors));
    }
  }

  void AssertInterprets(object? expected, string input) {
    var tree = new Parser(new Scanner(input, report).ScanTokens(), report).Parse();
    if (tree == null) Assert.Fail();
    else Assert.AreEqual(expected, new Interpreter(report).Interpret(tree));
  }
  
  static readonly List<string> errors = new();
  static readonly Report report = new(errors.Add);
}