namespace Syterra.JLox; 

public class Optional<T> {
  public static Optional<T> Empty => new();

  public static Optional<T> Of(T value) {
    return new(value);
  }
  
  public bool IsPresent { get; }

  public void IfPresent(Action<T> present) {
    if (IsPresent) present(value);
  }

  public void IfPresentOrElse(Action<T> present, Action notPresent) {
    if (IsPresent) present(value);
    else notPresent();
  }
  
  Optional() {
    IsPresent = false;
  }

  Optional(T value) {
    IsPresent = true;
    this.value = value;
  }

  readonly T value;
}
