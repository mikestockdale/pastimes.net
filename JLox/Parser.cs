namespace Syterra.JLox; 

public class Parser {
  public Parser(IEnumerable<Token> tokens) {
    this.tokens = new List<Token>(tokens);
  }

  public SyntaxTree Parse() {
    return Expression();
  }

  SyntaxTree Expression() {
    return Equality();
  }

  SyntaxTree Equality() {
    return Binary(Comparison, TokenType.EqualEqual, TokenType.NotEqual);
  }

  SyntaxTree Comparison() {
    return Binary(Term, TokenType.Greater, TokenType.GreaterEqual, TokenType.Less, TokenType.LessEqual);
  }

  SyntaxTree Term() {
    return Binary(Factor, TokenType.Minus, TokenType.Plus);
  }

  SyntaxTree Factor() {
    return Binary(Unary, TokenType.Star, TokenType.Slash);
  }

  SyntaxTree Binary(Func<SyntaxTree> parse, params TokenType[] matches) {
    var result = parse();
    while (Match(matches)) {
      var token = Previous;
      var right = parse();
      result = new SyntaxTree(token, result, right);
    }
    return result;
  }

  SyntaxTree Unary() {
    if (!Match(TokenType.Not, TokenType.Minus)) return Primary();
    var token = Previous;
    var right = Unary();
    return new SyntaxTree(token, right);
  }

  SyntaxTree Primary() {
    if (Match(TokenType.False, TokenType.True, TokenType.Nil, TokenType.Number, TokenType.String)) {
      return new SyntaxTree(Previous);
    }
    if (!Match(TokenType.LeftParen)) throw new NotImplementedException();
    var result = Expression();
    Consume(TokenType.RightParen, "Expected ')' after expression");
    return result;
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
  int current;
}