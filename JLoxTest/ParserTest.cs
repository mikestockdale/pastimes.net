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
  public void Parentheses(string expected, string input) {
    AssertParses(expected, input);
  }

  static void AssertParses(string expected, string input) {
    var parser = new Parser(new Scanner(input).ScanTokens());
    Assert.AreEqual(expected, parser.Parse().ToString());
  }
}