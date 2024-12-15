using AOC.Utilities;

namespace AOC2024.Day_04;

public class Solver
{
    private enum Direction
    {
        Up,
        UpRight,
        UpLeft,
        Left,
        Right,
        Down,
        DownRight,
        DownLeft
    }

    [Theory]
    [InlineData("example.txt", 18)]
    [InlineData("input.txt", 2458)]
    public static void SolveWordSearch(string file, int expectedResult)
    {
        StreamReader streamReader = DataParser.GetStreamReader(file);
        List<List<char>> wordSearch = [];
        while (!streamReader.EndOfStream)
        {
            string line = streamReader.ReadLine() ?? throw new Exception("");
            List<char> lineChars = [.. line.Select(x => x)];
            wordSearch.Add(lineChars);
        }

        int foundWords = 0;
        for (int i = 0; i < wordSearch.Count; i++)
        {
            for (int j = 0; j < wordSearch[i].Count; j++)
            {

                if (wordSearch[i][j] != 'X')
                {
                    continue;
                }
                foreach (Direction direction in Enum.GetValues<Direction>())
                {
                    foundWords += FindWordByDirection(wordSearch, (i, j), direction);
                }
            }
        }

        Assert.Equal(foundWords, expectedResult);
    }

    private static int FindWordByDirection(List<List<char>> wordSearch, (int x, int y) index, Direction direction)
    {
        try
        {
            (int xIndexModifier, int yIndexModifier) = direction switch
            {
                Direction.Up => (0, -1),
                Direction.Down => (0, 1),
                Direction.Left => (-1, 0),
                Direction.Right => (1, 0),
                Direction.UpRight => (1, -1),
                Direction.UpLeft => (-1, -1),
                Direction.DownLeft => (-1, 1),
                Direction.DownRight => (1, 1),
                _ => throw new NotImplementedException(),
            };

            List<char> chars = [
                wordSearch[index.x][index.y],
                wordSearch[index.x + xIndexModifier * 1][index.y + yIndexModifier * 1],
                wordSearch[index.x + xIndexModifier * 2][index.y + yIndexModifier * 2],
                wordSearch[index.x + xIndexModifier * 3][index.y + yIndexModifier * 3],
            ];


            return new string([.. chars]) == "XMAS" ? 1 : 0;
        }
        catch
        {

            return 0;
        }
    }

    [Theory]
    [InlineData("example.txt", 9)]
    [InlineData("input.txt", 1945)]
    public static void SolveWordSearchX(string file, int expectedResult)
    {
        StreamReader streamReader = DataParser.GetStreamReader(file);
        List<List<char>> wordSearch = [];
        while (!streamReader.EndOfStream)
        {
            string line = streamReader.ReadLine() ?? throw new Exception("");
            List<char> lineChars = [.. line.Select(x => x)];
            wordSearch.Add(lineChars);
        }

        int foundWords = 0;
        for (int i = 0; i < wordSearch.Count; i++)
        {
            for (int j = 0; j < wordSearch[i].Count; j++)
            {

                if (wordSearch[i][j] != 'A')
                {
                    continue;
                }
                foundWords += FindCrossPattern(wordSearch, (i, j));
            }
        }

        Assert.Equal(foundWords, expectedResult);
    }

    private static int FindCrossPattern(List<List<char>> wordSearch, (int x, int y) index)
    {
        try
        {
            string pattern1 = new([
                wordSearch[index.x - 1][index.y - 1],
                wordSearch[index.x][index.y],
                wordSearch[index.x + 1][index.y + 1],
            ]);
            string pattern2 = new([
                wordSearch[index.x + 1][index.y - 1],
                wordSearch[index.x][index.y],
                wordSearch[index.x - 1][index.y + 1],
            ]);

            bool crossPattern =
                (pattern1 == "MAS" || pattern1 == "SAM")
                && (pattern2 == "MAS" || pattern2 == "SAM");

            return crossPattern ? 1 : 0;
        }
        catch
        {
            return 0;
        }
    }
}
