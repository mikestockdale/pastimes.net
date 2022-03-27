using NUnit.Framework;

namespace Syterra.JLox.Test; 

public class ParserTest {
  
  [TestCase("false")]
  [TestCase("true")]
  [TestCase("nil")]
  [TestCase("\"abc\"")]
  [TestCase("123")]
  public void Literals(string input) {
    AssertParses(input, input);
  }

  [TestCase("false", "(false)")]
  [TestCase("(* 123 (+ 456 789))", "123*(456+789)")]
  public void Parentheses(string expected, string input) {
    AssertParses(expected, input);
  }

  [TestCase("(! false)", "!false")]
  [TestCase("(- 123)", "-123")]
  public void Unaries(string expected, string input) {
    AssertParses(expected, input);
  }

  [TestCase("(* 123 456)", "123*456")]
  [TestCase("(/ 123 456)", "123/456")]
  [TestCase("(* (- 123) (- 456))", "-123*-456")]
  [TestCase("(* (/ 123 456) 789)", "123/456*789")]
  public void Factors(string expected, string input) {
    AssertParses(expected, input);
  }

  [TestCase("(+ 123 456)", "123+456")]
  [TestCase("(- 123 456)", "123-456")]
  [TestCase("(+ (* 123 456) (/ 789 876))", "123*456+789/876")]
  [TestCase("(- (+ 123 456) 789)", "123+456-789")]
  public void Terms(string expected, string input) {
    AssertParses(expected, input);
  }

  [TestCase("(> 123 456)", "123>456")]
  [TestCase("(>= 123 456)", "123>=456")]
  [TestCase("(< 123 456)", "123<456")]
  [TestCase("(<= 123 456)", "123<=456")]
  [TestCase("(> (+ 123 456) (/ 789 876))", "123+456>789/876")]
  [TestCase("(< (> 123 456) 789)", "123>456<789")]
  public void Comparisons(string expected, string input) {
    AssertParses(expected, input);
  }
  
  [TestCase("(== 123 456)", "123==456")]
  [TestCase("(!= 123 456)", "123!=456")]
  [TestCase("(== (* 123 456) (/ 789 876))", "123*456==789/876")]
  [TestCase("(!= (== 123 456) 789)", "123==456!=789")]
  public void Equalities(string expected, string input) {
    AssertParses(expected, input);
  }

  static void AssertParses(string expected, string input) {
    var parser = new Parser(new Scanner(input).ScanTokens());
    Assert.AreEqual(expected, parser.Parse().ToString());
  }
}