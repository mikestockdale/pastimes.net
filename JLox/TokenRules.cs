namespace Syterra.JLox; 

public static class TokenRules {
  
  public static void Apply(char input, Scanner scanner) {
    rules.GetValueOrDefault(input, s => DefaultRule(input, s))(scanner);
  }

  static void DefaultRule(char input, Scanner scanner) {
    if (IsAlphabetic(input)) IdentifierRule(scanner);
    else scanner.AddError($"Unknown character '{input}'");
  }

  static TokenRules() {
    AddSingleRule('(', TokenType.LeftParen);
    AddSingleRule(')', TokenType.RightParen);
    AddSingleRule('{', TokenType.LeftBrace);
    AddSingleRule('}', TokenType.RightBrace);
    AddSingleRule(',', TokenType.Comma);
    AddSingleRule('.', TokenType.Dot);
    AddSingleRule('-', TokenType.Minus);
    AddSingleRule('+', TokenType.Plus);
    AddSingleRule(';', TokenType.Semicolon);
    AddSingleRule('*', TokenType.Star);
    AddDoubleRule('!', TokenType.Not, '=', TokenType.NotEqual);
    AddDoubleRule('=', TokenType.Equal, '=', TokenType.EqualEqual);
    AddDoubleRule('<', TokenType.Less, '=', TokenType.LessEqual);
    AddDoubleRule('>', TokenType.Greater, '=', TokenType.GreaterEqual);
    rules.Add('/', SlashRule);
    rules.Add('"', StringRule);
    AddIgnore(' ');
    AddIgnore('\t');
    AddIgnore('\r');
    AddIgnore('\n');
    for (var c = '0'; c <= '9'; c++) rules.Add(c, DigitRule);
  }

  static void SlashRule(Scanner scanner) {
    if (!scanner.Match('/')) scanner.AddToken(TokenType.Slash);
    else scanner.AdvanceTo('\n');
  }

  static void StringRule(Scanner scanner) {
    var result = scanner.AdvanceTo('"');
    if (result.EndsWith('"')) scanner.AddToken(TokenType.String, result.Substring(1, result.Length - 2));
    else scanner.AddError("Unterminated string");
  }

  static void IdentifierRule(Scanner scanner) {
    var result = scanner.AdvanceWhile(IsIdentifier);
    scanner.AddToken(keywords.GetValueOrDefault(result, TokenType.Identifier));
  }

  static void DigitRule(Scanner scanner) {
    scanner.AdvanceWhile(IsNumber);
    scanner.AdvanceWhile(IsDecimal);
    var result = scanner.AdvanceWhile(IsNumber);
    scanner.AddToken(TokenType.Number, double.Parse(result));
  }

  static bool IsIdentifier(char current, char next) {
    return IsAlphabetic(current) || IsDigit(current);
  }

  static bool IsAlphabetic(char input) {
    return input is >= 'A' and <= 'Z' or >= 'a' and <= 'z' or '_';
  }
  
  static bool IsDecimal(char current, char next) { return current == '.' && IsDigit(next); }

  static bool IsNumber(char current, char next) { return IsDigit(current); }

  static bool IsDigit(char input) { return input is >= '0' and <= '9'; }

  static void AddSingleRule(char key, TokenType type) {
    rules.Add(key, s => s.AddToken(type));
  }

  static void AddDoubleRule(char first, TokenType firstType, char second, TokenType secondType) {
    rules.Add(first, s => s.AddToken(s.Match(second) ? secondType : firstType));
  }

  static void AddIgnore(char key) {
    rules.Add(key, _ => {});
  }

  static readonly Dictionary<char, Action<Scanner>> rules = new();
  static readonly Dictionary<string, TokenType> keywords = new() {
    { "and", TokenType.And },
    { "class", TokenType.Class },
    { "else", TokenType.Else },
    { "for", TokenType.For },
    { "fun", TokenType.Fun },
    { "if", TokenType.If },
    { "or", TokenType.Or },
    { "print", TokenType.Print },
    { "return", TokenType.Return },
    { "super", TokenType.Super },
    { "this", TokenType.This },
    { "var", TokenType.Var },
    { "while", TokenType.While },
  };
}