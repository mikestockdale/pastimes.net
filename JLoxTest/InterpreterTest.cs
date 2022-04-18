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
    AssertInterpretsExpression(expected, input);
  }

  [TestCase(true, "!false")]
  [TestCase(true, "!nil")]
  [TestCase(false, "!true")]
  [TestCase(false, "!\"abc\"")]
  [TestCase(-12.3, "-12.3")]
  public void Unaries(object? expected, string input) {
    AssertInterpretsExpression(expected, input);
  }
  
  [TestCase(579, "456+123")]
  [TestCase("abcdef", "\"abc\"+\"def\"")]
  [TestCase(333, "456-123")]
  [TestCase(1.23, "123*0.01")]
  [TestCase(228, "456/2")]
  public void Arithmetic(object? expected, string input) {
    AssertInterpretsExpression(expected, input);
  }

  [TestCase(true, "2>1")]
  [TestCase(false, "1>=2")]
  [TestCase(true, "1<2")]
  [TestCase(false, "2<=1")]
  public void Comparisons(object? expected, string input) {
    AssertInterpretsExpression(expected, input);
  }

  [TestCase(true, "nil==nil")]
  [TestCase(false, "nil!=nil")]
  [TestCase(true, "1==1")]
  [TestCase(true, "1!=\"1\"")]
  [TestCase(false, "1==nil")]
  [TestCase(true, "nil!=1")]
  [TestCase(true, "\"abc\"==\"abc\"")]
  [TestCase(false, "\"abc\"==nil")]
  [TestCase(true, "\"3\"!=3")]
  public void Equality(object? expected, string input) {
    AssertInterpretsExpression(expected, input);
  }
  
  [TestCase("hi", "print \"hi\";")]
  [TestCase("hi;123", "print \"hi\";print 123;")]
  [TestCase("[line 1] Error: Operands must be two numbers or two strings", "print \"hi\"+123;print 123;")]
  public void Print(string expected, string input) {
    AssertInterpretsStatements(expected, input);
  }

  [TestCase("1", "var x=1;print x;")]
  [TestCase("2", "var x=1;var x=2;print x;")]
  [TestCase("2;1", "var x=1;{var x=2;print x;}print x;")]
  public void Variables(string expected, string input) {
    AssertInterpretsStatements(expected, input);
  }
  
  [TestCase("2", "var x;x=2;print x;")]
  [TestCase("2", "var x=1;print x=2;")]
  [TestCase("2", "var x=1;x=x+1;print x;")]
  [TestCase("4", "var x=1;var y;x=y=x+1;print x+y;")]
  [TestCase("2;2", "var x=1;{x=2;print x;}print x;")]
  public void Assignment(string expected, string input) {
    AssertInterpretsStatements(expected, input);
  }

  [TestCase("[line 1] Error: Operand must be a number", "-nil")]
  [TestCase("[line 1] Error: Operands must be two numbers or two strings", "\"abc\"+123")]
  [TestCase("[line 1] Error: Operand must be a number", "123-nil")]
  [TestCase("[line 1] Error: Operand must be a number", "123*\"abc\"")]
  [TestCase("[line 1] Error: Operand must be a number", "\"cde\"/\"abc\"")]
  [TestCase("[line 1] Error: Operand must be a number", "123>\"abc\"")]
  [TestCase("[line 1] Error: Operand must be a number", "123>=\"abc\"")]
  [TestCase("[line 1] Error: Operand must be a number", "123<\"abc\"")]
  [TestCase("[line 1] Error: Operand must be a number", "123<=\"abc\"")]
  [TestCase("[line 1] Error: Operands must be two numbers or two strings", "123<=(123+\"abc\")")]
  [TestCase("[line 1] Error: Undefined variable 'x'", "x=123")]
  [TestCase("[line 1] Error: Undefined variable 'x'", "{var x=1;}x=2")]
  public void Errors(string expected, string input) {
    var tree = Parse(input +";");
    Assert.AreNotEqual(tree.Children.Count, 0);
    var result = new Interpreter(report).Interpret(tree);
    Assert.IsNull(result);
    Assert.AreEqual(expected, string.Join(";", errors));
  }

  static void AssertInterpretsStatements(string expected, string input) {
    var tree = Parse(input);
    Assert.AreNotEqual(tree.Children.Count, 0);
    var result = new Interpreter(report).Interpret(tree);
    Assert.IsNull(result);
    Assert.AreEqual(expected, string.Join(";", errors));
  }

  void AssertInterpretsExpression(object? expected, string input) {
    var tree = Parse(input + ";");
    Assert.AreNotEqual(tree.Children.Count, 0);
    Assert.AreEqual(expected, new Interpreter(report).Interpret(tree.Children[0]));
  }

  static SyntaxTree Parse(string input) {
    errors.Clear();
    return new Parser(new Scanner(input, report).ScanTokens(), report).Parse();
  }
  
  static readonly List<string> errors = new();
  static readonly Report report = new(errors.Add);
}