using AOC.Utilities;

namespace AOC2024.Day_02;

public class Solver
{
    [Theory]
    [InlineData("example.txt", 2)]
    [InlineData("input.txt", 598)]
    public static void CalculateSafeReports(string file, int expectedResult)
    {
        StreamReader streamReader = DataParser.GetStreamReader(file);

        int safeReportCount = 0;
        while (streamReader.EndOfStream == false)
        {
            string line = streamReader.ReadLine() ?? throw new Exception("");
            IEnumerable<int> levels = line.Split(" ").Select(int.Parse);
            int? previousLevel = null;
            bool? ascending = null;
            bool isSafe = false;
            foreach (int level in levels)
            {
                if (previousLevel is not null)
                {
                    ascending ??= level > previousLevel.Value;
                    int difference = level - previousLevel.Value;
                    isSafe = ascending.Value && difference is > 0 and <= 3 || ascending.Value is false && difference is < 0 and >= -3;

                    if (isSafe is false)
                    {
                        break;
                    }
                }
                previousLevel = level;
            }

            if (isSafe)
            {
                safeReportCount++;
            }
        }

        Assert.Equal(expectedResult, safeReportCount);
    }

    private static bool IsSafeReport(List<int> report)
    {
        List<int> differences = report.Skip(1)
            .Select((level, index) => level - report.ElementAt(index))
            .ToList();

        bool isSafe = differences.All(diff => diff is <= 3 and > 0) || differences.All(diff => diff is >= -3 and < 0);
        return isSafe;
    }

    [Theory]
    [InlineData("example.txt", 4)]
    [InlineData("input.txt", 634)]
    public static void CalculateSafeReportsWithDampener(string file, int expectedResult)
    {
        StreamReader streamReader = DataParser.GetStreamReader(file);

        int safeReportCount = 0;
        while (streamReader.EndOfStream == false)
        {
            string line = streamReader.ReadLine() ?? throw new Exception("");

            List<int> levels = line.Split(" ").Select(int.Parse).ToList();
            List<List<int>> reports = [levels];
            bool isSafe = IsSafeReport(levels);

            if (isSafe == false)
            {
                for (int i = 0; i < levels.Count; i++)
                {
                    List<int> report = [.. levels];
                    report.RemoveAt(i);
                    isSafe = IsSafeReport(report);
                    if (isSafe)
                    {
                        break;
                    }
                }
            }

            if (isSafe)
            {
                safeReportCount++;
            }
        }

        Assert.Equal(expectedResult, safeReportCount);
    }
}
