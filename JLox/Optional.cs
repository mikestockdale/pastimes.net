namespace Syterra.JLox; 

public class Optional<T> {
  public static Optional<T> Empty => new();

  public static Optional<T> Of(T value) {
    return new Optional<T>(value);
  }
  
  public bool IsPresent { get; }

  public void IfPresent(Action<T> present) {
    if (IsPresent) present(value);
  }

  public void IfPresentOrElse(Action<T> present, Action notPresent) {
    if (IsPresent) present(value);
    else notPresent();
  }

  public Optional<U> Select<U>(Func<T, U> map) {
    return IsPresent ? Optional<U>.Of(map(value)) : Optional<U>.Empty;
  }

  public T OrElse(T other) {
    return IsPresent ? value : other;
  }
  
#pragma warning disable CS8618
  Optional() {
    IsPresent = false;
  }
#pragma warning restore CS8618

  Optional(T value) {
    IsPresent = true;
    this.value = value;
  }

  readonly T value;
}
