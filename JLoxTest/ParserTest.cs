using System.Collections.Generic;
using NUnit.Framework;

namespace Syterra.JLox.Test; 

public class ParserTest {
  
  [TestCase("false")]
  [TestCase("true")]
  [TestCase("nil")]
  [TestCase("\"abc\"")]
  [TestCase("123")]
  public void Literals(string input) {
    AssertParsesExpression(input, input);
  }

  [TestCase("false", "(false)")]
  [TestCase("(* 123 (+ 456 789))", "123*(456+789)")]
  public void Parentheses(string expected, string input) {
    AssertParsesExpression(expected, input);
  }

  [TestCase("(! nil)", "!nil")]
  [TestCase("(! false)", "!false")]
  [TestCase("(- 123)", "-123")]
  public void Unaries(string expected, string input) {
    AssertParsesExpression(expected, input);
  }

  [TestCase("(* 123 456)", "123*456")]
  [TestCase("(/ 123 456)", "123/456")]
  [TestCase("(* (- 123) (- 456))", "-123*-456")]
  [TestCase("(* (/ 123 456) 789)", "123/456*789")]
  public void Factors(string expected, string input) {
    AssertParsesExpression(expected, input);
  }

  [TestCase("(+ 123 456)", "123+456")]
  [TestCase("(- 123 456)", "123-456")]
  [TestCase("(+ (* 123 456) (/ 789 876))", "123*456+789/876")]
  [TestCase("(- (+ 123 456) 789)", "123+456-789")]
  public void Terms(string expected, string input) {
    AssertParsesExpression(expected, input);
  }

  [TestCase("(> 123 456)", "123>456")]
  [TestCase("(>= 123 456)", "123>=456")]
  [TestCase("(< 123 456)", "123<456")]
  [TestCase("(<= 123 456)", "123<=456")]
  [TestCase("(> (+ 123 456) (/ 789 876))", "123+456>789/876")]
  [TestCase("(< (> 123 456) 789)", "123>456<789")]
  public void Comparisons(string expected, string input) {
    AssertParsesExpression(expected, input);
  }
  
  [TestCase("(== 123 456)", "123==456")]
  [TestCase("(!= 123 456)", "123!=456")]
  [TestCase("(== (* 123 456) (/ 789 876))", "123*456==789/876")]
  [TestCase("(!= (== 123 456) 789)", "123==456!=789")]
  public void Equalities(string expected, string input) {
    AssertParsesExpression(expected, input);
  }
  
  [TestCase("List", "")]
  [TestCase("(List 123)", "123;")]
  [TestCase("(List (print 123))", "print 123;")]
  [TestCase("(List (print 123) 456)", "print 123;456;")]
  public void List(string expected, string input) {
    AssertParses(expected, input);
  }
  
  [TestCase("[line 1] Error at end: Expected ')' after expression", "(1+2")]
  [TestCase("[line 1] Error at ')': Expected expression", ")")]
  [TestCase("[line 1] Error at end: Expect ';' after value", "123")]
  [TestCase("[line 1] Error at end: Expected expression", "print")]
  [TestCase("[line 1] Error at ';': Expected expression", "print;")]
  [TestCase("[line 1] Error at end: Expect ';' after value", "print 123")]
  public void Errors(string expected, string input) {
    AssertParsesError(expected, input);
  }

  static void AssertParsesExpression(string expected, string input) {
    AssertParses($"(List {expected})", input + ";");
  }

  static void AssertParses(string expected, string input) {
    var result = new Parser(new Scanner(input, report).ScanTokens(), report).Parse();
    if (result == null) Assert.Fail();
    else Assert.AreEqual(expected, result.ToString());
  }

  static void AssertParsesError(string expected, string input) {
    errors.Clear();
    var result = new Parser(new Scanner(input, report).ScanTokens(), report).Parse();
    Assert.IsNull(result);
    Assert.AreEqual(expected, string.Join(";", errors));
  }

  static readonly List<string> errors = new();
  static readonly Report report = new(errors.Add);
}