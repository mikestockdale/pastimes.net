namespace Syterra.JLox; 

public class Parser {
  public Parser(IEnumerable<Token> tokens, Action<string> write) {
    this.write = write;
    this.tokens = new List<Token>(tokens);
    hasErrors = false;
  }

  public Optional<SyntaxTree> Parse() {
      var result = List();
      return hasErrors ? Optional<SyntaxTree>.Empty : Optional<SyntaxTree>.Of(result);
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
      return
        Match(TokenType.Var) ? VarDeclaration() :
        Match(TokenType.Fun) ? Function("function") :
        Statement();
    }
    catch (ParseException) {
      Synchronize();
      return null;
    }
  }

  SyntaxTree Function(string kind) {
    var name = Consume(TokenType.Identifier,$"Expect {kind} name");
    Consume(TokenType.LeftParen, $"Expect '(' after {kind} name");
    var list = new List<SyntaxTree>();
    if (!Check(TokenType.RightParen)) {
      do {
        Consume(TokenType.Identifier, "Expect parameter name");
        list.Add(new SyntaxTree(Evaluate.Variable, Previous));
      } while (Match(TokenType.Comma));
    }
    Consume(TokenType.RightParen, "Expect ')' after parameters");
    Consume(TokenType.LeftBrace, "Expect '{' after parameters");
    var body = BlockStatement();
    return new SyntaxTree(Evaluate.Function, name, new SyntaxTree(list), body);
  }

  SyntaxTree VarDeclaration() {
    var name = Consume(TokenType.Identifier, "Expect variable name");
    var result = Match(TokenType.Equal)
      ? new SyntaxTree(Evaluate.Declare, name, Expression())
      : new SyntaxTree(Evaluate.Declare, name);
    Consume(TokenType.Semicolon, "Expect ';' after variable declaration");
    return result;
  }

  SyntaxTree Statement() {
    if (Match(TokenType.If)) return IfStatement();
    if (Match(TokenType.Return)) return ReturnStatement();
    if (Match(TokenType.While)) return WhileStatement();
    if (Match(TokenType.For)) return ForStatement();
    if (Match(TokenType.LeftBrace)) return BlockStatement();
    return ExpressionStatement();
  }

  SyntaxTree ReturnStatement() {
    var token = Previous;
    var result = Check(TokenType.Semicolon)
      ? new SyntaxTree(Evaluate.Return, token)
      : new SyntaxTree(Evaluate.Return, token, Expression());
    Consume(TokenType.Semicolon, "Expect ';' after return value");
    return result;
  }

  SyntaxTree ForStatement() {
    var token = Previous;
    var empty = new SyntaxTree(true, new Token(TokenType.Literal, "true", token.Line));
    Consume(TokenType.LeftParen, "Expect '(' after 'for'");
    var initializer = Match(TokenType.Semicolon) ? empty : Match(TokenType.Var) ? VarDeclaration() : ExpressionStatement();
    var condition = Match(TokenType.Semicolon) ? empty : ExpressionStatement();
    var increment = Check(TokenType.RightParen) ? empty : Expression();
    Consume(TokenType.RightParen, "Expect ')' after increment");
    return new SyntaxTree(Evaluate.For, token, initializer, condition, increment, Statement());
  }

  SyntaxTree WhileStatement() {
    var token = Previous;
    Consume(TokenType.LeftParen, "Expect '(' after 'while'");
    var condition = Expression();
    Consume(TokenType.RightParen, "Expect ')' after condition");
    var body = Statement();
    return new SyntaxTree(Evaluate.While, token, condition, body);
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
    if (!Match(TokenType.Not, TokenType.Minus)) return Call();
    var token = Previous;
    var right = Unary();
    return new SyntaxTree(unaryOperators[token.Type], token, right);
  }

  SyntaxTree Call() {
    var result = Primary();
    while (Match(TokenType.LeftParen)) {
      var arguments = new List<SyntaxTree>();
      if (!Check(TokenType.RightParen)) {
        do {
          arguments.Add(Expression());
        } while (Match(TokenType.Comma));
      }
      Consume(TokenType.RightParen, "Expect ')' after arguments");
      result = new SyntaxTree(Evaluate.Call, Previous, result, new SyntaxTree(arguments));
    }
    return result;
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

  Token Consume(TokenType type, string message) {
    if (!Check(type)) throw Error(Current, message);
    Advance();
    return Previous;
  }

  ParseException Error(Token token, string message) {
    write(Report.Message(token, message));
    hasErrors = true;
    return new ParseException();
  }

  bool Check(TokenType type) { return !AtEnd && Current.Type == type; }

  bool AtEnd => Current.Type == TokenType.Eof;

  Token Previous => tokens[current - 1];

  Token Current => tokens[current];

  readonly List<Token> tokens;
  readonly Action<string> write;
  int current;
  bool hasErrors;

  static readonly Dictionary<TokenType, Func<SyntaxTree, Environment, object?>> unaryOperators = new() {
    { TokenType.Not, Evaluate.Not },
    { TokenType.Minus, Evaluate.Negative }
  };

  static readonly Dictionary<TokenType, Func<SyntaxTree, Environment, object?>> binaryOperators = new() {
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
    TokenType.Return
  };

  class ParseException: Exception {}
}