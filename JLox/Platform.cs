using System.Diagnostics;

namespace Syterra.JLox; 

public interface Platform {
  void Write(string input);
  double Elapsed { get; }
  void Start();
}

public class RuntimePlatform : Platform {
  public void Write(string input) {
    Console.WriteLine(input);
  }

  public double Elapsed => stopwatch.Elapsed.TotalMilliseconds;

  public void Start() {
    stopwatch.Start();
  }
  
  readonly Stopwatch stopwatch = new();
}