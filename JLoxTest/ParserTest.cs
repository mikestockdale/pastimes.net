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
  
  [TestCase("abc", "abc")]
  [TestCase("(+ abc 1)", "abc+1")]
  public void Variables(string expected, string input) {
    AssertParsesExpression(expected, input);
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
  
  [TestCase("(List abc)", "var abc;")]
  [TestCase("(List (abc 1))", "var abc=1;")]
  [TestCase("(List (abc (+ abc 1)))", "var abc=abc+1;")]
  [TestCase("(List (abc 1) (print abc))", "var abc=1;print abc;")]
  public void Declaration(string expected, string input) {
    AssertParses(expected, input);
  }
  
  [TestCase("[line 1] Error at end: Expected ')' after expression", "(1+2")]
  [TestCase("[line 1] Error at ')': Expected expression", ")")]
  [TestCase("[line 1] Error at end: Expect ';' after value", "123")]
  [TestCase("[line 1] Error at end: Expected expression", "print")]
  [TestCase("[line 1] Error at ';': Expected expression", "print;")]
  [TestCase("[line 1] Error at end: Expect ';' after value", "print 123")]
  [TestCase("[line 1] Error at 'true': Expect variable name", "var true=false;")]
  public void Errors(string expected, string input) {
    AssertParsesError(expected, "List", input);
  }

  [TestCase("[line 1] Error at '+': Expect variable name", "(List 123)", "var +=/;123;")]
  [TestCase("[line 1] Error at '+': Expect variable name", "(List (print 123))", "var +=/ print 123;")]
  public void ErrorRecovery(string expectedError, string expectedSyntax, string input) {
    AssertParsesError(expectedError, expectedSyntax, input);
  }

  static void AssertParsesExpression(string expected, string input) {
    AssertParses($"(List {expected})", input + ";");
  }

  static void AssertParses(string expected, string input) {
    errors.Clear();
    var result = new Parser(new Scanner(input, report).ScanTokens(), report).Parse();
    Assert.AreEqual(0, errors.Count);
    Assert.AreEqual(expected, result.ToString());
  }

  static void AssertParsesError(string expectedError, string expectedSyntax, string input) {
    errors.Clear();
    var result = new Parser(new Scanner(input, report).ScanTokens(), report).Parse();
    Assert.AreEqual(expectedSyntax, result.ToString());
    Assert.AreEqual(expectedError, string.Join(";", errors));
  }

  static readonly List<string> errors = new();
  static readonly Report report = new(errors.Add);
}