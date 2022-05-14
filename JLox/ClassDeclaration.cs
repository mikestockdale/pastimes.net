namespace Syterra.JLox;

public class ClassDeclaration: Callable {
  public ClassDeclaration(string name) {
    this.name = name;
  }

  public int Arity => 0;
  
  public object? Call(object?[] arguments) {
    return new ClassInstance(this);
  }
  
  public override string ToString() { return name; }

  readonly string name;
}