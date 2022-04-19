namespace Syterra.JLox; 

public enum SymbolType {
  Literal, Variable,
  Not, Negative,
  Add, Subtract, Multiply, Divide,
  Equal, NotEqual,
  Greater, GreaterEqual,
  Less, LessEqual,
  List,
  Print, Declare, Assign, If
}