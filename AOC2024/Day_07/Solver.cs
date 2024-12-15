using AOC.Utilities;

namespace AOC2024.Day_07;

public class Solver
{
    //From StackOverflow
    //https://stackoverflow.com/questions/25824376/combinations-with-repetitions-c-sharp
    private static IEnumerable<string> CombinationsWithRepetition(IEnumerable<string> input, int length)
    {
        if (length <= 0)
            yield return "";
        else
        {
            foreach (var i in input)
                foreach (var c in CombinationsWithRepetition(input, length - 1))
                    yield return i.ToString() + c;
        }
    }

    [Theory]
    [InlineData("example.txt", 3749)]
    [InlineData("input.txt", 7885693428401)]
    public static void GetTotalCalibrationResult(string file, long expectedResult)
    {
        StreamReader streamReader = DataParser.GetStreamReader(file);

        long calibrationSum = 0;

        while (!streamReader.EndOfStream)
        {
            string line = streamReader.ReadLine() ?? throw new Exception("");
            List<long> numbers = line
                .Split([':', ' '])
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(long.Parse)
                .ToList();

            long expectedValue = numbers[0];
            numbers.RemoveAt(0);

            int operatorCount = numbers.Count - 1;
            long combinationsCount = (long)Math.Pow(2, operatorCount);
            //List<string> combinations = [];
            for (int i = 0; i < combinationsCount; i++)
            {
                string combination = Convert.ToString(i, 2).PadLeft(operatorCount, '0');
                long sum = numbers[0];
                List<long> localNumbers = numbers[1..];
                for (int j = 0; j < combination.Length; j++)
                {
                    if (combination[j] == '1')
                    {
                        sum += localNumbers[j];
                    }
                    else
                    {
                        sum *= localNumbers[j];
                    }
                }
                if (sum == expectedValue)
                {
                    calibrationSum += sum;
                    break;
                }
            }
        }

        Assert.Equal(expectedResult, calibrationSum);
    }

    [Theory]
    [InlineData("example.txt", 11387)]
    [InlineData("input.txt", 348360680516005)]
    public static void GetTotalCalibrationResultExtended(string file, long expectedResult)
    {
        StreamReader streamReader = DataParser.GetStreamReader(file);

        long calibrationSum = 0;

        while (!streamReader.EndOfStream)
        {
            string line = streamReader.ReadLine() ?? throw new Exception("");
            List<long> numbers = line
                .Split([':', ' '])
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(long.Parse)
                .ToList();

            long expectedValue = numbers[0];
            numbers.RemoveAt(0);

            List<string> operators = ["*", "+", "|"];
            List<string> combinations = CombinationsWithRepetition(operators, numbers.Count - 1).ToList();


            foreach (string combination in combinations)
            {
                long sum = numbers[0];
                List<long> localNumbers = numbers[1..];

                for (int j = 0; j < localNumbers.Count; j++)
                {
                    if (combination[j] == '|')
                    {
                        sum = long.Parse(sum.ToString() + localNumbers[j].ToString());
                    }
                    else if (combination[j] == '+')
                    {
                        sum += localNumbers[j];
                    }
                    else if (combination[j] == '*')
                    {
                        sum *= localNumbers[j];
                    }
                }

                if (sum == expectedValue)
                {
                    calibrationSum += sum;
                    break;
                }
            }
        }

        Assert.Equal(expectedResult, calibrationSum);
    }
}
