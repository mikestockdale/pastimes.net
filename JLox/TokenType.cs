namespace Syterra.JLox; 

public enum TokenType {
  // Single-character tokens
  LeftParen, RightParen, LeftBrace, RightBrace,
  Comma, Dot, Minus, Plus, Semicolon, Slash, Star,

  // One or two character tokens.
  Not, NotEqual,
  Equal, EqualEqual,
  Greater, GreaterEqual,
  Less, LessEqual,

  // Terminals
  Identifier, Literal,

  // Keywords
  And, Class, Else, Fun, For, If, Or,
  Print, Return, Super, This, Var, While,

  // Special
  Eof
}