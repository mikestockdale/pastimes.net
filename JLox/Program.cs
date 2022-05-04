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
    }
  }

  static int RunFile(string path) {
    return Run(File.ReadAllText(path));
  }

  static int Run(string source) {
    var scanner = new Scanner(source, Console.WriteLine);
    var parser = new Parser(scanner.ScanTokens(), Console.WriteLine);
    var tree = parser.Parse();
    return scanner.HasErrors || !tree.IsPresent ? 65 : tree.Select(interpreter.Interpret).IsPresent ? 0 : 70;
  }
  
  static readonly Interpreter interpreter = new (Console.WriteLine);
}