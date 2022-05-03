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
      report.Reset();
    }
  }

  static int RunFile(string path) {
    var result = Run(File.ReadAllText(path));
    return report.HadError ? 65 : result.IsPresent ? 0 : 70;
  }

  static Optional<object?> Run(string source) {
    var scanner = new Scanner(source, report);
    var parser = new Parser(scanner.ScanTokens(), report);
    var tree = parser.Parse();
    return report.HadError ? Optional<object?>.Empty : interpreter.Interpret(tree);
  }
  
  static readonly Report report = new(Console.WriteLine);
  static readonly Interpreter interpreter = new (Console.WriteLine);
}