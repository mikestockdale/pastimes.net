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
  public void Comment() {
    AssertTokens("LeftParen (", "(//)");
  }

  static void AssertTokens(string expected, string source) {
    var result = new Scanner(source).ScanTokens();
    Assert.AreEqual(expected + (expected.Length > 0 ? "," : string.Empty) + "Eof",
      string.Join(",", result.Select(t => t.ToString().Trim())));
  }
}