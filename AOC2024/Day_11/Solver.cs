using AOC.Utilities;

namespace AOC2024.Day_11;

public class Solver
{
    [Theory]
    [InlineData("example.txt", 25, 55312)]
    [InlineData("input.txt", 25, 216042)]
    [InlineData("input.txt", 75, 255758646442399)]
    public static void CountStones(string file, int blinks, long expectedResult)
    {
        List<long> stoneBag = [];
        {
            StreamReader streamReader = DataParser.GetStreamReader(file);
            while (!streamReader.EndOfStream)
            {
                string line = streamReader.ReadLine() ?? throw new Exception("");
                stoneBag = line.Split(" ").Select(long.Parse).ToList();
            }
        }

        long stonesCount = 0;
        Dictionary<(long, long), long> cache = [];

        stonesCount = RecursivelyBlink(stoneBag, blinks, cache);

        static long RecursivelyBlink(List<long> stones, int timesToBlink, Dictionary<(long, long), long> cache)
        {
            if (timesToBlink == 0) return stones.Count;

            long count = 0;
            foreach (var stone in stones)
            {
                if (cache.TryGetValue((stone, timesToBlink), out long value))
                {
                    count += value;
                    continue;
                }

                long singleStoneResult = RecursivelyBlink(Blink(stone), timesToBlink - 1, cache);
                cache.Add((stone, timesToBlink), singleStoneResult);

                count += singleStoneResult;
            }

            return count;
        }

        static List<long> Blink(long stone)
        {
            List<long> stoneBag = [];
            if (stone == 0)
            {
                return [1];
            }

            string stoneStr = stone.ToString();
            if (stoneStr.Length % 2 == 0)
            {
                stoneBag.AddRange([
                    int.Parse(stoneStr[..(stoneStr.Length / 2)]),
                    int.Parse(stoneStr[(stoneStr.Length / 2)..]),
                ]);
                return stoneBag;
            }

            stoneBag.Add(stone * 2024);

            return stoneBag;
        }

        Assert.Equal(expectedResult, stonesCount);
    }
}
