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
    AssertParsesList(input, input);
  }
  
  [TestCase("abc", "abc")]
  [TestCase("(+ abc 1)", "abc+1")]
  public void Variables(string expected, string input) {
    AssertParsesList(expected, input);
  }

  [TestCase("false", "(false)")]
  [TestCase("(* 123 (+ 456 789))", "123*(456+789)")]
  public void Parentheses(string expected, string input) {
    AssertParsesList(expected, input);
  }

  [TestCase("(! nil)", "!nil")]
  [TestCase("(! false)", "!false")]
  [TestCase("(- 123)", "-123")]
  public void Unaries(string expected, string input) {
    AssertParsesList(expected, input);
  }

  [TestCase("(* 123 456)", "123*456")]
  [TestCase("(/ 123 456)", "123/456")]
  [TestCase("(* (- 123) (- 456))", "-123*-456")]
  [TestCase("(* (/ 123 456) 789)", "123/456*789")]
  public void Factors(string expected, string input) {
    AssertParsesList(expected, input);
  }

  [TestCase("(+ 123 456)", "123+456")]
  [TestCase("(- 123 456)", "123-456")]
  [TestCase("(+ (* 123 456) (/ 789 876))", "123*456+789/876")]
  [TestCase("(- (+ 123 456) 789)", "123+456-789")]
  public void Terms(string expected, string input) {
    AssertParsesList(expected, input);
  }

  [TestCase("(> 123 456)", "123>456")]
  [TestCase("(>= 123 456)", "123>=456")]
  [TestCase("(< 123 456)", "123<456")]
  [TestCase("(<= 123 456)", "123<=456")]
  [TestCase("(> (+ 123 456) (/ 789 876))", "123+456>789/876")]
  [TestCase("(< (> 123 456) 789)", "123>456<789")]
  public void Comparisons(string expected, string input) {
    AssertParsesList(expected, input);
  }
  
  [TestCase("(== 123 456)", "123==456")]
  [TestCase("(!= 123 456)", "123!=456")]
  [TestCase("(== (* 123 456) (/ 789 876))", "123*456==789/876")]
  [TestCase("(!= (== 123 456) 789)", "123==456!=789")]
  public void Equalities(string expected, string input) {
    AssertParsesList(expected, input);
  }
  
  [TestCase("List", "")]
  [TestCase("(List 123)", "123;")]
  [TestCase("(List (print 123))", "print 123;")]
  [TestCase("(List (print 123) 456)", "print 123;456;")]
  [TestCase("(List (print 123) (List (print 456)))", "print 123;{print 456;}")]
  [TestCase("(List 123 (List 45 78) 90)", "123;{45;78;}90;")]
  public void List(string expected, string input) {
    AssertParses(expected, input);
  }
  
  [TestCase("abc", "var abc")]
  [TestCase("(abc 1)", "var abc=1")]
  [TestCase("(abc (+ abc 1))", "var abc=abc+1")]
  [TestCase("(abc 1) (print abc)", "var abc=1;print abc")]
  public void Declaration(string expected, string input) {
    AssertParsesList(expected, input);
  }
  
  [TestCase("(= abc 1)", "abc = 1")]
  [TestCase("(= abc (+ abc 1))", "abc = abc+1")]
  [TestCase("(= abc (= def 2))", "abc = def = 2")]
  public void Assignment(string expected, string input) {
    AssertParsesList(expected, input);
  }
  
  [TestCase("(if (== a 1) (print a))", "if(a==1)print a")]
  [TestCase("(if cond a b)", "if(cond)a;else b")]
  [TestCase("(if c1 (if c2 a b))", "if(c1) if(c2)a;else b")]
  [TestCase("(if cond (List a b) (List c d)) e", "if(cond){a;b;}else {c;d;}e")]
  public void If(string expected, string input) {
    AssertParsesList(expected, input);
  }
  
  [TestCase("[line 1] Error at end: Expected ')' after expression", "(1+2")]
  [TestCase("[line 1] Error at ')': Expected expression", ")")]
  [TestCase("[line 1] Error at end: Expect ';' after value", "123")]
  [TestCase("[line 1] Error at end: Expected expression", "print")]
  [TestCase("[line 1] Error at ';': Expected expression", "print;")]
  [TestCase("[line 1] Error at end: Expect ';' after value", "print 123")]
  [TestCase("[line 1] Error at 'true': Expect variable name", "var true=false;")]
  [TestCase("[line 1] Error at '=': Invalid assignment target", "(a+1)=2;")]
  [TestCase("[line 1] Error at end: Expect '}' after block", "{123;")]
  [TestCase("[line 1] Error at 'a': Expect '(' after 'if'", "if a b")]
  [TestCase("[line 1] Error at 'b': Expect ')' after if condition", "if (a b")]
  public void Errors(string expected, string input) {
    AssertParsesError(expected, "List", input);
  }

  [TestCase("[line 1] Error at '+': Expect variable name", "(List 123)", "var +=/;123;")]
  [TestCase("[line 1] Error at '+': Expect variable name", "(List (print 123))", "var +=/ print 123;")]
  public void ErrorRecovery(string expectedError, string expectedSyntax, string input) {
    AssertParsesError(expectedError, expectedSyntax, input);
  }

  static void AssertParsesList(string expected, string input) {
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