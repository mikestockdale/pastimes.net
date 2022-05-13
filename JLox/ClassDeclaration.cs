namespace Syterra.JLox;

public class ClassDeclaration {
  public ClassDeclaration(string name) {
    this.name = name;
  }
  
  public override string ToString() { return name; }

  readonly string name;
}