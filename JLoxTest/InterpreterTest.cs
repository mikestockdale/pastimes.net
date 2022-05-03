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

  [TestCase(false, "true and false")]
  [TestCase(true, "false or true")]
  [TestCase(456, "122 and 456")]
  [TestCase(null, "nil and 456")]
  [TestCase(456, "nil or 456")]
  [TestCase(123, "123 or 456")]
  public void Logicals(object? expected, string input) {
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
  
  [TestCase("hi", "print(\"hi\");")]
  [TestCase("hi123", "print(\"hi\",123);")]
  [TestCase("nil", "print(nil);")]
  public void Print(string expected, string input) {
    AssertInterpretsStatements(expected, input);
  }

  [TestCase("1", "var x=1;print(x);")]
  [TestCase("2", "var x=1;var x=2;print(x);")]
  [TestCase("2;1", "var x=1;{var x=2;print(x);}print(x);")]
  public void Variables(string expected, string input) {
    AssertInterpretsStatements(expected, input);
  }
  
  [TestCase("2", "var x;x=2;print(x);")]
  [TestCase("2", "var x=1;print(x=2);")]
  [TestCase("2", "var x=1;x=x+1;print(x);")]
  [TestCase("4", "var x=1;var y;x=y=x+1;print(x+y);")]
  [TestCase("2;2", "var x=1;{x=2;print(x);}print(x);")]
  public void Assignment(string expected, string input) {
    AssertInterpretsStatements(expected, input);
  }

  [TestCase("123", "var x=1;if(x==1)print(123);")]
  [TestCase("", "var x=2;if(x==1)print(123);")]
  [TestCase("456", "var x=2;if(x==1)print(123);else print(456);")]
  public void If(string expected, string input) {
    AssertInterpretsStatements(expected, input);
  }

  [TestCase("123", "var x=1;while(x==1){print(123);x=2;}")]
  [TestCase("", "var x=2;while(x==1)print(123);")]
  [TestCase("123;123", "var x=1;while(x<3){print(123);x=x+1;}")]
  public void While(string expected, string input) {
    AssertInterpretsStatements(expected, input);
  }

  [TestCase("123;123", "for(var x=1;x<3;x=x+1)print(123);")]
  public void For(string expected, string input) {
    AssertInterpretsStatements(expected, input);
  }

  [TestCase("123", "print(test(123));")]
  [TestCase("456", "fun a(){print(456);}a();")]
  [TestCase("789", "fun a(b){print(b);}a(789);")]
  public void Call(string expected, string input) {
    AssertInterpretsStatements(expected, input);
  }
  
  [TestCase("124", "fun a(b){return b+1;return 789;}print(a(123));")]
  [TestCase("124", "fun a(b){if (b>0)return b+1;}print(a(123));")]
  [TestCase("nil", "fun a(b){if (b>0)return b+1;}print(a(-123));")]
  [TestCase("124", "fun a(b){if (b>0)return b+1;return b;}print(a(123));")]
  [TestCase("-123", "fun a(b){if (b>0)return b+1;return b;}print(a(-123));")]
  [TestCase("-124", "fun a(b){if (b>0)return b+1;else return b-1;}print(a(-123));")]
  [TestCase("nil", "fun a(b){if (b>0)return;return b;}print(a(123));")]
  [TestCase("124", "fun a(b){while (b>0) return b+1;}print(a(123));")]
  [TestCase("124", "fun a(b){for (;;) return b+1;}print(a(123));")]
  public void Return(string expected, string input) {
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
  [TestCase("[line 1] Error: Can only call functions and classes", "123(123)")]
  [TestCase("[line 2] Error: Expected 2 arguments but got 1", "fun x(a,b){return 0;}\nx(123)")]
  [TestCase("[line 1] Error: Expected 1 arguments but got 2", "test(1,2)")]
  public void Errors(string expected, string input) {
    var tree = Parse(input + ";");
    Assert.AreNotEqual(tree.Branches.Count, 0);
    Assert.IsFalse(Interpret(tree).IsPresent);
    Assert.AreEqual(expected, string.Join(";", errors));
  }

  static void AssertInterpretsStatements(string expected, string input) {
    var tree = Parse(input);
    Assert.AreNotEqual(tree.Branches.Count, 0);
    Assert.IsTrue(Interpret(tree).IsPresent);
    Assert.AreEqual(expected, string.Join(";", errors));
  }

  void AssertInterpretsExpression(object? expected, string input) {
    var tree = Parse(input + ";");
    Assert.AreNotEqual(tree.Branches.Count, 0);
    Interpret(tree.Branches[0]).IfPresentOrElse(v => Assert.AreEqual(expected,v), Assert.Fail);
  }

  static Optional<object?> Interpret(SyntaxTree tree) {
    return new Interpreter(errors.Add).Interpret(tree, e => e.Define("test", new NativeCall(p => p[0], 1)));
  }

  static SyntaxTree Parse(string input) {
    errors.Clear();
    return new Parser(new Scanner(input, report).ScanTokens(), report).Parse();
  }
  
  static readonly List<string> errors = new();
  static readonly Report report = new(errors.Add);
}