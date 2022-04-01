namespace Syterra.JLox; 

public class Parser {

  public Parser(IEnumerable<Token> tokens, Report report) {
    this.report = report;
    this.tokens = new List<Token>(tokens);
  }

  public SyntaxTree? Parse() {
    try {
      return Expression();
    }
    catch (ParseException) {
      return null;
    }
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
      result = new SyntaxTree(binaryOperators[token.Type], token, result, right);
    }
    return result;
  }

  SyntaxTree Unary() {
    if (!Match(TokenType.Not, TokenType.Minus)) return Primary();
    var token = Previous;
    var right = Unary();
    return new SyntaxTree(unaryOperators[token.Type], token, right);
  }

  SyntaxTree Primary() {
    if (Match(TokenType.Number, TokenType.String)) {
      return new SyntaxTree(Previous.Literal, Previous);
    }
    if (Match(TokenType.False)) {
      return new SyntaxTree(false, Previous);
    }
    if (Match(TokenType.True)) {
      return new SyntaxTree(true, Previous);
    }
    if (Match(TokenType.Nil)) {
      return new SyntaxTree(null, Previous);
    }
    if (!Match(TokenType.LeftParen)) throw Error(Current, "Expected expression");
    var result = Expression();
    Consume(TokenType.RightParen, "Expected ')' after expression");
    return result;
  }

  bool Match(params TokenType[] types) {
    if (!types.Any(Check)) return false;
    Advance();
    return true;
  }

  void Advance() { if (!AtEnd) current++; }

  void Consume(TokenType type, string message) {
    if (Check(type)) Advance();
    else throw Error(Current, message);
  }

  ParseException Error(Token token, string message) {
    report.Error(token, message);
    return new ParseException();
  }

  bool Check(TokenType type) { return !AtEnd && Current.Type == type; }

  bool AtEnd => Current.Type == TokenType.Eof;

  Token Previous => tokens[current - 1];

  Token Current => tokens[current];

  readonly List<Token> tokens;
  readonly Report report;
  int current;

  static readonly Dictionary<TokenType, SymbolType> unaryOperators = new() {
    { TokenType.Not, SymbolType.Not },
    { TokenType.Minus, SymbolType.Negative }
  };

  static readonly Dictionary<TokenType, SymbolType> binaryOperators = new() {
    { TokenType.Plus, SymbolType.Add },
    { TokenType.Minus, SymbolType.Subtract },
    { TokenType.Star, SymbolType.Multiply },
    { TokenType.Slash, SymbolType.Divide },
    { TokenType.EqualEqual, SymbolType.Equal },
    { TokenType.NotEqual, SymbolType.NotEqual },
    { TokenType.Greater, SymbolType.Greater },
    { TokenType.GreaterEqual, SymbolType.GreaterEqual },
    { TokenType.Less, SymbolType.Less },
    { TokenType.LessEqual, SymbolType.LessEqual }
  };

  class ParseException: Exception {}
}