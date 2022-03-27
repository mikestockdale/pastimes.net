using System.Collections.Concurrent;

namespace Syterra.JLox; 

public class Parser {
  public Parser(IEnumerable<Token> tokens) {
    this.tokens = new List<Token>(tokens);
  }

  public SyntaxTree Parse() {
    return Expression();
  }

  SyntaxTree Expression() {
    return Primary();
  }

  SyntaxTree Primary() {
    if (Match(TokenType.False, TokenType.True, TokenType.Nil, TokenType.Number, TokenType.String)) {
      return new SyntaxTree(Previous);
    }

    if (Match(TokenType.LeftParen)) {
      var result = Expression();
      Consume(TokenType.RightParen, "Expected ')' after expression");
      return result;
    }
    throw new NotImplementedException();
  }

  bool Match(params TokenType[] types) {
    if (!types.Any(Check)) return false;
    Advance();
    return true;
  }

  Token Advance() {
    if (!AtEnd) current++;
    return Previous;
  }

  Token Consume(TokenType type, string message) {
    if (Check(type)) return Advance();
    throw new NotImplementedException();
  }

  bool Check(TokenType type) { return !AtEnd && Current.Type == type; }

  bool AtEnd => Current.Type == TokenType.Eof;

  Token Previous => tokens[current - 1];

  Token Current => tokens[current];

  readonly List<Token> tokens;
  int current = 0;
}