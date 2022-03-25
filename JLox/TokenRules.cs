namespace Syterra.JLox; 

public static class TokenRules {
  
  public static void Apply(char input, Scanner scanner) {
    if (rules.ContainsKey(input)) rules[input](scanner);
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
    AddIgnore(' ');
    AddIgnore('\t');
    AddIgnore('\r');
    AddIgnore('\n');
  }

  static void SlashRule(Scanner scanner) {
    if (!scanner.Match('/')) scanner.AddToken(TokenType.Slash);
    else scanner.AdvanceTo('\n');
  }

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
}