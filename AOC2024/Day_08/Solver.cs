using AOC.Utilities;

namespace AOC2024.Day_08;
public class Solver
{
    private static IEnumerable<IEnumerable<T>> CombinationsWithoutRepetition<T>(IEnumerable<T> input, int length)
    {
        for (int i = 0; i < input.Count(); i++)
        {
            if (length == 1)
            {
                yield return [input.ElementAt(i)];
            }
            foreach (IEnumerable<T> result in CombinationsWithoutRepetition(input.Skip(i + 1), length - 1))
                yield return [input.ElementAt(i), .. result];
        }
    }
    class Position(int x, int y, char? frequency = null)
    {
        public int X { get; set; } = x;
        public int Y { get; set; } = y;
        public char? Frequency { get; set; } = frequency;

        public override string ToString() => $"{X},{Y}";
    }

    [Theory]
    [InlineData("example.txt", 14)]
    [InlineData("input.txt", 390)]
    public static void AntinodeLoactions(string file, int expectedResult)
    {
        StreamReader streamReader = DataParser.GetStreamReader(file);
        int row = 0;
        List<Position> antennaes = [];
        HashSet<char> frequencies = [];
        while (!streamReader.EndOfStream)
        {
            string line = streamReader.ReadLine() ?? throw new Exception("");
            char[] antennaSpots = [.. line];
            for (int i = 0; i < antennaSpots.Length; i++)
            {
                char? antennaFrequency = antennaSpots[i] != '.' ? antennaSpots[i] : null;
                if (antennaFrequency is not null)
                {
                    frequencies.Add(antennaFrequency.Value);
                }
                Position position = new(i, row, antennaFrequency);
                antennaes.Add(position);
            }
            row++;
        }

        HashSet<string> antinodes = [];

        foreach (char frequency in frequencies)
        {
            IEnumerable<Position> antennaesWithFreqency = antennaes.Where(x => x.Frequency == frequency);
            IEnumerable<IEnumerable<Position>> antennaPairs = CombinationsWithoutRepetition(antennaesWithFreqency, 2);
            foreach (IEnumerable<Position> antennaPair in antennaPairs)
            {
                Position antenna1 = antennaPair.ElementAt(0);
                Position antenna2 = antennaPair.ElementAt(1);
                int xOffset = antenna1.X - antenna2.X;
                int yOffset = antenna1.Y - antenna2.Y;

                Position? antinode1 = antennaes.FirstOrDefault(x => x.X == antenna1.X + xOffset && x.Y == antenna1.Y + yOffset);
                if (antinode1 is not null)
                {
                    antinodes.Add(antinode1.ToString());
                }
                Position? antinode2 = antennaes.FirstOrDefault(x => x.X == antenna2.X + xOffset * -1 && x.Y == antenna2.Y + yOffset * -1);
                if (antinode2 is not null)
                {
                    antinodes.Add(antinode2.ToString());
                }
            }
        }

        Assert.Equal(expectedResult, antinodes.Count);
    }

    [Theory]
    [InlineData("example.txt", 34)]
    [InlineData("input.txt", 1246)]
    public static void AntinodeLoactionsWithResonant(string file, int expectedResult)
    {
        StreamReader streamReader = DataParser.GetStreamReader(file);
        int row = 0;
        List<Position> antennaes = [];
        HashSet<char> frequencies = [];
        while (!streamReader.EndOfStream)
        {
            string line = streamReader.ReadLine() ?? throw new Exception("");
            char[] antennaSpots = [.. line];
            for (int i = 0; i < antennaSpots.Length; i++)
            {
                char? antennaFrequency = antennaSpots[i] != '.' ? antennaSpots[i] : null;
                if (antennaFrequency is not null)
                {
                    frequencies.Add(antennaFrequency.Value);
                }
                Position position = new(i, row, antennaFrequency);
                antennaes.Add(position);
            }
            row++;
        }

        HashSet<string> antinodes = [];

        foreach (char frequency in frequencies)
        {
            IEnumerable<Position> antennaesWithFreqency = antennaes.Where(x => x.Frequency == frequency);
            IEnumerable<IEnumerable<Position>> antennaPairs = CombinationsWithoutRepetition(antennaesWithFreqency, 2);
            foreach (IEnumerable<Position> antennaPair in antennaPairs)
            {
                if (antennaPair.Count() != 2)
                {
                    continue;
                }
                Position antenna1 = antennaPair.ElementAt(0);
                Position antenna2 = antennaPair.ElementAt(1);

                antinodes.Add(antenna1.ToString());
                antinodes.Add(antenna2.ToString());

                int xOffset = antenna1.X - antenna2.X;
                int yOffset = antenna1.Y - antenna2.Y;

                void FindAntinodes(Position startAntenna, int xOffset, int yOffset)
                {
                    bool outOfBounds = false;
                    Position? antinode = null;

                    while (outOfBounds is false)
                    {
                        int xAntinode = antinode?.X ?? startAntenna.X;
                        int yAntinode = antinode?.Y ?? startAntenna.Y;
                        antinode = antennaes.FirstOrDefault(x => x.X == xAntinode + xOffset && x.Y == yAntinode + yOffset);
                        if (antinode is not null)
                        {
                            antinodes.Add(antinode.ToString());
                        }
                        outOfBounds = antinode is null;
                    }
                }

                FindAntinodes(antenna1, xOffset, yOffset);
                FindAntinodes(antenna2, xOffset * -1, yOffset * -1);
            }
        }

        Assert.Equal(expectedResult, antinodes.Count);
    }
}
