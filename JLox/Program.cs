namespace Syterra.JLox; 

internal static class Program {

  static int Main(string[] args) {
    switch (args.Length) {
      case > 1:
        Console.WriteLine("Usage: jlox [script]");
        return 64;
      case 1:
        return RunFile(args[0]);
      default:
        RunPrompt();
        break;
    }
    return 0;
  }

  static void RunPrompt() {
    while (true) {
      Console.Write("> ");
      var line = Console.ReadLine();
      if (line == null) break;
      Run(line);
      hadError = false;
    }
  }

  static int RunFile(string path) {
    Run(File.ReadAllText(path));
    return hadError ? 65 : 0;
  }

  static void Run(string source) {
    var scanner = new Scanner(source);
    foreach (var token in scanner.ScanTokens())
      Console.WriteLine(token);
  }

  static void Error(int line, string message) {
    Report(line, "", message);
  }

  static void Report(int line, string where, string message) {
    Console.WriteLine($"[line {line} ] Error{where}: {message}");
    hadError = true;
  }
  
  static bool hadError = false;
}