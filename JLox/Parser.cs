namespace Syterra.JLox; 

public class Parser {

  public Parser(IEnumerable<Token> tokens, Report report) {
    this.report = report;
    this.tokens = new List<Token>(tokens);
  }

  public SyntaxTree? Parse() {
    try {
      return List();
    }
    catch (ParseException) {
      return null;
    }
  }

  SyntaxTree List() {
    var list = new List<SyntaxTree>();
    while (!AtEnd) list.Add(Declaration());
    return new SyntaxTree(SymbolType.List, list);
  }

  SyntaxTree Declaration() {
      return (Match(TokenType.Var)) ? VarDeclaration() : Statement();
  }

  SyntaxTree VarDeclaration() {
    Consume(TokenType.Identifier, "Expect variable name");
    var name = Previous;
    var result = Match(TokenType.Equal)
      ? new SyntaxTree(SymbolType.Declare, name, Expression())
      : new SyntaxTree(SymbolType.Declare, name);
    Consume(TokenType.Semicolon, "Expect ';' after variable declaration");
    return result;
  }

  SyntaxTree Statement() {
    return Match(TokenType.Print) ? PrintStatement() : ExpressionStatement();
  }

  SyntaxTree PrintStatement() {
    var token = Previous;
    var result = Expression();
    Consume(TokenType.Semicolon, "Expect ';' after value");
    return new SyntaxTree(SymbolType.Print, token, result);
  }

  SyntaxTree ExpressionStatement() {
    var result = Expression();
    Consume(TokenType.Semicolon, "Expect ';' after value");
    return result;
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
    if (Match(TokenType.Literal)) {
      return new SyntaxTree(Previous.Literal, Previous);
    }
    if (Match(TokenType.Identifier)) {
      return new SyntaxTree(SymbolType.Variable, Previous);
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