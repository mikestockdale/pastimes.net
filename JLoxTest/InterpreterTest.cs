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

  void AssertInterprets(object? expected, string input) {
    var tree = new Parser(new Scanner(input, report).ScanTokens(), report).Parse();
    if (tree == null) Assert.Fail();
    else Assert.AreEqual(expected, new Interpreter().Interpret(tree));
  }
  
  static readonly List<string> errors = new();
  static readonly Report report = new(errors.Add);
}