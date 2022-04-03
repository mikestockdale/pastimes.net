namespace Syterra.JLox; 

public enum SymbolType {
  Terminal,
  Not, Negative,
  Add, Subtract, Multiply, Divide,
  Equal, NotEqual,
  Greater, GreaterEqual,
  Less, LessEqual,
  List,
  Print
}