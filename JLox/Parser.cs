namespace Syterra.JLox; 

public class Parser {

  public Parser(IEnumerable<Token> tokens, Report report) {
    this.report = report;
    this.tokens = new List<Token>(tokens);
  }

  public SyntaxTree Parse() {
      return List();
  }

  SyntaxTree List() {
    var list = new List<SyntaxTree>();
    while (!AtEnd) {
      var declaration = Declaration();
      if (declaration != null) list.Add(declaration);
    }
    return new SyntaxTree(Evaluate.List, list);
  }

  SyntaxTree? Declaration() {
    try {
      return Match(TokenType.Var) ? VarDeclaration() : Statement();
    }
    catch (ParseException) {
      Synchronize();
      return null;
    }
  }

  SyntaxTree VarDeclaration() {
    Consume(TokenType.Identifier, "Expect variable name");
    var name = Previous;
    var result = Match(TokenType.Equal)
      ? new SyntaxTree(Evaluate.Declare, name, Expression())
      : new SyntaxTree(Evaluate.Declare, name);
    Consume(TokenType.Semicolon, "Expect ';' after variable declaration");
    return result;
  }

  SyntaxTree Statement() {
    if (Match(TokenType.If)) return IfStatement();
    if (Match(TokenType.Print)) return PrintStatement();
    if (Match(TokenType.LeftBrace)) return BlockStatement();
    return ExpressionStatement();
  }

  SyntaxTree IfStatement() {
    var token = Previous;
    Consume(TokenType.LeftParen, "Expect '(' after 'if'");
    var condition = Expression();
    Consume(TokenType.RightParen, "Expect ')' after if condition");
    var then = Statement();
    return Match(TokenType.Else)
      ? new SyntaxTree(Evaluate.If, token, condition, then, Statement())
      : new SyntaxTree(Evaluate.If, token, condition, then);
  }

  SyntaxTree BlockStatement() {
    var statements = new List<SyntaxTree>();
    while (!AtEnd && !Check(TokenType.RightBrace)) {
      var statement = Declaration();
      if (statement != null) statements.Add(statement);
    }
    Consume(TokenType.RightBrace, "Expect '}' after block");
    return new SyntaxTree(Evaluate.List, statements);
  }

  SyntaxTree PrintStatement() {
    var token = Previous;
    var result = Expression();
    Consume(TokenType.Semicolon, "Expect ';' after value");
    return new SyntaxTree(Evaluate.Print, token, result);
  }

  SyntaxTree ExpressionStatement() {
    var result = Expression();
    Consume(TokenType.Semicolon, "Expect ';' after value");
    return result;
  }

  SyntaxTree Expression() {
    return Assignment();
  }

  SyntaxTree Assignment() {
    var result = Or();
    if (!Match(TokenType.Equal)) return result;
    var token = Previous;
    var value = Assignment();
    if (result.Token.Type != TokenType.Identifier) throw Error(token, "Invalid assignment target");
    return new SyntaxTree(Evaluate.Assign, token, result, value);
  }

  SyntaxTree Or() {
    return Binary(And, TokenType.Or);
  }

  SyntaxTree And() {
    return Binary(Equality, TokenType.And);
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
      return new SyntaxTree(Evaluate.Variable, Previous);
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

  void Synchronize() {
    Advance();
    while (!AtEnd) {
      if (Previous.Type == TokenType.Semicolon) return;
      if (statementTypes.Contains(Current.Type)) return;
      Advance();
    }
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

  static readonly Dictionary<TokenType, Func<SyntaxTree, Interpreter, object?>> unaryOperators = new() {
    { TokenType.Not, Evaluate.Not },
    { TokenType.Minus, Evaluate.Negative }
  };

  static readonly Dictionary<TokenType, Func<SyntaxTree, Interpreter, object?>> binaryOperators = new() {
    { TokenType.Plus, Evaluate.Add },
    { TokenType.Minus, Evaluate.Subtract },
    { TokenType.Star, Evaluate.Multiply },
    { TokenType.Slash, Evaluate.Divide },
    { TokenType.EqualEqual, Evaluate.Equal },
    { TokenType.NotEqual, Evaluate.NotEqual },
    { TokenType.Greater, Evaluate.Greater },
    { TokenType.GreaterEqual, Evaluate.GreaterEqual },
    { TokenType.Less, Evaluate.Less },
    { TokenType.LessEqual, Evaluate.LessEqual },
    { TokenType.Or, Evaluate.Or },
    { TokenType.And, Evaluate.And }
  };

  static readonly HashSet<TokenType> statementTypes = new() {
    TokenType.Class,
    TokenType.Fun,
    TokenType.Var,
    TokenType.For,
    TokenType.If,
    TokenType.While,
    TokenType.Print,
    TokenType.Return
  };

  class ParseException: Exception {}
}