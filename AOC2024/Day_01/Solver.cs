using AOC.Utilities;
using System.Text.RegularExpressions;

namespace AOC2024.Day_01;

public partial class Solver
{
    private static (List<int>, List<int>) GetLocationLists(string file)
    {
        StreamReader streamReader = DataParser.GetStreamReader(file);

        List<int> locationIdsLeft = [];
        List<int> locationIdsRight = [];

        while (!streamReader.EndOfStream)
        {
            string line = streamReader.ReadLine() ?? throw new Exception();
            string[] parts = WhitespaceRegex().Split(line);
            locationIdsLeft.Add(int.Parse(parts[0]));
            locationIdsRight.Add(int.Parse(parts[1]));
        }


        return (locationIdsLeft, locationIdsRight);
    }

    [Theory]
    [InlineData("example.txt", 11)]
    [InlineData("input.txt", 1388114)]
    public static void CompareLists(string file, int expectedResult)
    {
        (List<int> locationIdsLeft, List<int> locationIdsRight) = GetLocationLists(file);
        locationIdsLeft = [.. locationIdsLeft.OrderBy(x => x)];
        locationIdsRight = [.. locationIdsRight.OrderBy(x => x)];

        int difference = locationIdsLeft
            .Select((idLeft, index) => Math.Abs(idLeft - locationIdsRight[index]))
            .Sum();

        Assert.Equal(expectedResult, difference);
    }

    [Theory]
    [InlineData("example.txt", 31)]
    [InlineData("input.txt", 23529853)]
    public static void GetSimilarityScore(string file, int expectedResult)
    {
        (List<int> locationIdsLeft, List<int> locationIdsRight) = GetLocationLists(file);

        Dictionary<int, int> comparisonMap = [];
        int similarityScore = 0;
        foreach (int i in locationIdsLeft)
        {
            if (comparisonMap.TryGetValue(i, out int value))
            {
                similarityScore += i * value;
                continue;
            }

            int count = locationIdsRight.Count(x => x == i);
            comparisonMap[i] = count;
            similarityScore += i * count;
        }

        Assert.Equal(expectedResult, similarityScore);
    }

    [GeneratedRegex(@"\s+")]
    private static partial Regex WhitespaceRegex();
}
