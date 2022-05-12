using NUnit.Framework;

namespace NQueens; 

public class Test {
  // Test a method HasConflict to detect if there are queens on conflicting squares
  // I started out with separate methods for row conflict, column conflict, etc.
  // Then I saw they were very similar and it was easier to combine them into a single method
  //[TestCase(true, "00", "06")]
  [TestCase(true, "00", "10")]
  [TestCase(true, "01", "10")]
  [TestCase(true, "00", "11")]
  [TestCase(false, "00", "12")]
  public void Conflicts(bool expected, params string[] squares) {
    Assert.AreEqual(expected, Square.HasConflict(squares.Select(s => new Square(s[0] - '0', s[1] - '0'))));
  }

  [Test]
  // Test a method FindSolution to, well, find a solution
  // Interesting that the test uses the method HasConflict that we developed for FindSolution
  public void Solution() {
    var solution = Square.FindSolution();
    Assert.IsFalse(Square.HasConflict(solution));
    Assert.AreEqual(8, solution.Count);
    TestContext.WriteLine(string.Join(",", solution.Select(s => $"{s.Row}{s.Column}")));
  }

  [Test]
  public void All() {
    var all = Square.FindAll();
    Assert.AreEqual(92, all.Count);
    foreach (var solution in all) {
      TestContext.WriteLine(string.Join(",", solution.Select(s => $"{s.Row}{s.Column}")));
    }
  }
}

public record Square(int Row, int Column) {
  
  // See if the queens in the list of squares have any conflicts
  public static bool HasConflict(IEnumerable<Square> squares) {
    // four arrays to keep track of which rows, columns, etc. are occupied by a queen
    //var rowOccupied = new bool[8];
    var columnOccupied = new bool[8];
    // there's 15 forward-slanted diagonals and 15 backward-slanted diagonals. Count them!
    var forwardDiagonalOccupied = new bool[15];
    var backwardDiagonalOccupied = new bool[15];
    foreach (var (row, column) in squares) {
      // check if anything is already occupied by a previous queen
      //if (rowOccupied[row]) return true;
      if (columnOccupied[column]) return true;
      // the forward diagonals are numbered by the sum of row and column
      var forward = row + column;
      if (forwardDiagonalOccupied[forward]) return true;
      // the backward diagonals are numbered by the difference of row and column,
      // plus 7 to keep the number non-negative
      var backward = row - column + 7;
      if (backwardDiagonalOccupied[backward]) return true;
      // if no conflicts so far, mark the places that this queen occupies
      //rowOccupied[row] = true;
      columnOccupied[column] = true;
      forwardDiagonalOccupied[forward] = true;
      backwardDiagonalOccupied[backward] = true;
    }
    return false;
  }

  public static List<Square> FindSolution() { return FindSolution(empty); }

  // find a place to add a queen
  // this is called recursively, starting with an empty board, until there's 8 queens added
  static List<Square> FindSolution(IEnumerable<Square> previous) {
    // make a copy of the previous state
    var current = new List<Square>(previous);
    // the next queen will be added at the end
    var next = current.Count;
    // add a dummy entry - this will be replaced with actual candidate squares in the loop below
    current.Add(new Square(-1, -1));
    // originally I looped through all the rows here
    // the one perforamce optimization I made was to always assign rows sequentially
    // i.e. the first queen is in row 0, the second queen is in row 1, etc.
    for (var column = 0; column < 8; column++) {
      // try a candidate square for the new queen
      current[next] = new Square(next, column);
      // if conflict, keep trying
      if (HasConflict(current)) continue;
      // if it was the eighth queen, we're done!
      if (current.Count == 8) return current;
      // try to add another queen
      var result = FindSolution(current);
      // if a solution was found, return it.
      if (result.Count > 0) return result;
      // otherwise, try another column
    }
    // return an empty solution to indicate no valid solution was found
    return empty;
  }

  public static List<List<Square>> FindAll() {
    all.Clear();
    FindAll(empty);
    return all;
  }

  static void FindAll(IEnumerable<Square> previous) {
    var current = new List<Square>(previous);
    var next = current.Count;
    current.Add(new Square(-1, -1));
    for (var column = 0; column < 8; column++) {
      current[next] = new Square(next, column);
      if (HasConflict(current)) continue;
      if (current.Count == 8) all.Add(new List<Square>(current));
      else FindAll(current);
    }
  }

  static readonly List<Square> empty = new();
  static readonly List<List<Square>> all = new();
}