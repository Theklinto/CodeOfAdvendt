using AOC.Utilities;
using System.Text.RegularExpressions;

namespace AOC2024.Day_03;

public partial class Solver
{
    [Theory]
    [InlineData("example_1.txt", 161)]
    [InlineData("input.txt", 182780583)]
    public static void ReadCorruptedMemoryInstructions(string file, int expectedResult)
    {
        StreamReader streamReader = DataParser.GetStreamReader(file);
        string memory = streamReader.ReadToEnd();

        MatchCollection matches = InstructionRegex().Matches(memory);
        int totalInstructionSum = matches.Sum(instruction =>
        {
            MatchCollection numberMatches = NumberRegex().Matches(instruction.Value);
            int[] numbers = numberMatches.Select(x => x.Value).Select(int.Parse).ToArray();
            return numbers[0] * numbers[1];
        });

        Assert.Equal(totalInstructionSum, expectedResult);
    }

    [Theory]
    [InlineData("example_2.txt", 48)]
    [InlineData("input.txt", 90772405)]
    public static void ReadCorruptedMemoryInstructionsExtended(string file, int expectedResult)
    {
        StreamReader streamReader = DataParser.GetStreamReader(file);
        string memory = streamReader.ReadToEnd();

        MatchCollection matches = MultiInstructionRegex().Matches(memory);
        int totalInstructionSum = 0;
        bool allowCalculations = true;
        foreach (Match match in matches)
        {
            if (match.Value == "do()")
            {
                allowCalculations = true;
            }
            else if (match.Value == "don't()")
            {
                allowCalculations = false;
            }
            else if (allowCalculations)
            {
                MatchCollection numberMatches = NumberRegex().Matches(match.Value);
                int[] numbers = numberMatches.Select(x => x.Value).Select(int.Parse).ToArray();
                totalInstructionSum += numbers[0] * numbers[1];
            }
        }

        Assert.Equal(totalInstructionSum, expectedResult);
    }

    [GeneratedRegex(@"mul\(\d{1,3},\d{1,3}\)")]
    private static partial Regex InstructionRegex();

    [GeneratedRegex(@"\d+")]
    private static partial Regex NumberRegex();

    [GeneratedRegex(@"mul\(\d{1,3},\d{1,3}\)|do\(\)|don\'t\(\)")]
    private static partial Regex MultiInstructionRegex();
}
