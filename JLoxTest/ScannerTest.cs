using System.Linq;
using NUnit.Framework;

namespace Syterra.JLox.Test;

public class ScannerTest {

  [Test]
  public void EmptySource() {
    AssertTokens(string.Empty, string.Empty);
  }

  [Test]
  public void SingleCharacterTokens() {
    AssertTokens(
      "LeftParen (,RightParen ),LeftBrace {,RightBrace },Comma ,,Dot .,Minus -,Plus +,Semicolon ;,Star *,Slash /,Equal =,Not !,Greater >,Less <",
      "(){},.-+;*/=!><");
  }

  [Test]
  public void UnknownCharacter() {
    AssertTokens("LeftBrace {,Error Unknown character '#',RightBrace }", "{#}");
  }

  [Test]
  public void TwoCharacterTokens() {
    AssertTokens("NotEqual !=,EqualEqual ==,GreaterEqual >=,LessEqual <=", "!===>=<=");
  }

  [Test]
  public void IgnoreComment() {
    AssertTokens("LeftParen (", "(//)");
  }

  [Test]
  public void IgnoreBlankSpace() {
    AssertTokens("LeftParen (", " \t\r\n(");
  }

  [Test]
  public void CountLines() {
    var result = new Scanner("(\n(//\n").ScanTokens();
    Assert.AreEqual("1,2,3", string.Join(",", result.Select(t => t.Line)));
  }

  [TestCase("String \"abc\" abc", "\"abc\"")]
  [TestCase("String \"\"", "\"\"")]
  [TestCase("LeftParen (,String \"abc\" abc,RightParen )", "(\"abc\")")]
  [TestCase("Error Unterminated string", "\"abc")]
  public void StringLiterals(string expected, string source) {
    AssertTokens(expected, source);
  }
  
  [TestCase("Number 123 123", "123")]
  [TestCase("Number 123 123,RightParen )", "123)")]
  [TestCase("Number 12.3 12.3", "12.3")]
  [TestCase("Number 123 123,Dot .", "123.")]
  [TestCase("Dot .,Number 123 123", ".123")]
  [TestCase("Number 1.2 1.2,Dot .,Number 3 3", "1.2.3")]
  public void NumericLiterals(string expected, string source) {
    AssertTokens(expected, source);
  }

  [TestCase("Identifier abc", "abc")]
  [TestCase("Identifier _", "_")]
  [TestCase("Identifier _abz_ABZ123", " _abz_ABZ123 ")]
  public void Identifiers(string expected, string source) {
    AssertTokens(expected, source);
  }
  [TestCase("And and", " and ")]
  [TestCase("Class class", "class")]
  [TestCase("Else else", "else")]
  [TestCase("False false", "false")]
  [TestCase("For for", "for")]
  [TestCase("Fun fun", "fun")]
  [TestCase("If if", "if")]
  [TestCase("Nil nil", "nil")]
  [TestCase("Or or", "or")]
  [TestCase("Print print", "print")]
  [TestCase("Return return", "return")]
  [TestCase("Super super", "super")]
  [TestCase("This this", "this")]
  [TestCase("True true", "true")]
  [TestCase("Var var", "var")]
  [TestCase("While while", "while")]
  public void Keywords(string expected, string source) {
    AssertTokens(expected, source);
  }

  static void AssertTokens(string expected, string source) {
    var result = new Scanner(source).ScanTokens();
    Assert.AreEqual(expected + (expected.Length > 0 ? "," : string.Empty) + "Eof",
      string.Join(",", result.Select(t => t.ToString().Trim())));
  }
}