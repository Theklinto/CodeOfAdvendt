using AOC.Utilities;

namespace AOC2024.Day_10;

public class Solver
{
    enum Direction
    {
        Left,
        Right,
        Up,
        Down,
    }
    class Position(int x, int y)
    {
        public int X { get; set; } = x;
        public int Y { get; set; } = y;
        public override string ToString() => $"{X},{Y}";

        public Position OffsetBy(Position position)
        {
            return new(X + position.X, Y + position.Y);
        }

        public override bool Equals(object? obj)
        {
            if (obj is null)
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != typeof(Position))
                return false;

            Position position = (Position)obj;
            return position.X == X && position.Y == Y;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
    }
    class Map : Dictionary<Position, int>
    {
        public List<Position> GetStartingPositions()
        {
            return this
                .Where(x => x.Value == 0)
                .Select(x => x.Key)
                .ToList();
        }

    }
    private static Position GetDirectionOffset(Direction direction)
    {
        return direction switch
        {
            Direction.Up => new(0, -1),
            Direction.Down => new(0, 1),
            Direction.Left => new(-1, 0),
            Direction.Right => new(1, 0),
            _ => throw new NotSupportedException()
        };
    }

    [Theory]
    [InlineData("example.txt", 36)]
    [InlineData("input.txt", 624)]
    public static void GetTrailheadSum(string file, int expectedResult)
    {
        Map map = new();
        {
            StreamReader streamReader = DataParser.GetStreamReader(file);
            int y = 0;
            while (!streamReader.EndOfStream)
            {
                string line = streamReader.ReadLine() ?? throw new Exception("");
                List<int> heights = line.ToCharArray().Select(x => int.Parse(new([x]))).ToList();
                for (int i = 0; i < heights.Count; i++)
                {
                    Position position = new(i, y);
                    map[position] = heights[i];
                }
                y++;
            }
        }

        List<Position> startingPositions = map.GetStartingPositions();
        int trailheadSum = 0;
        foreach (Position startingPosition in startingPositions)
        {
            HashSet<Position> trailTopsfound = [];
            Trailwalker(startingPosition, 0, map, trailTopsfound);
            trailheadSum += trailTopsfound.Count;
        }


        Assert.Equal(expectedResult, trailheadSum);
    }

    [Theory]
    [InlineData("example.txt", 81)]
    [InlineData("input.txt", 1483)]
    public static void GetTrailheadRating(string file, int expectedResult)
    {
        Map map = new();
        {
            StreamReader streamReader = DataParser.GetStreamReader(file);
            int y = 0;
            while (!streamReader.EndOfStream)
            {
                string line = streamReader.ReadLine() ?? throw new Exception("");
                List<int> heights = line.ToCharArray().Select(x => int.Parse(new([x]))).ToList();
                for (int i = 0; i < heights.Count; i++)
                {
                    Position position = new(i, y);
                    map[position] = heights[i];
                }
                y++;
            }
        }

        List<Position> startingPositions = map.GetStartingPositions();
        int trailheadSum = 0;
        foreach (Position startingPosition in startingPositions)
        {
            List<Position> trailTopsfound = [];
            Trailwalker(startingPosition, 0, map, trailTopsfound);
            trailheadSum += trailTopsfound.Count;
        }

        Assert.Equal(expectedResult, trailheadSum);
    }

    //Returns true if the trail is completable from the start position
    private static void Trailwalker(Position startPosition, int previousHeight, Map map, ICollection<Position> trailTopsFound)
    {
        foreach (Direction direction in Enum.GetValues<Direction>())
        {
            Position directionOffset = GetDirectionOffset(direction);
            Position nextPosition = startPosition.OffsetBy(directionOffset);

            if (map.TryGetValue(nextPosition, out int height) is false)
                continue;

            if (height - previousHeight != 1)
                continue;

            if (height == 9)
            {
                trailTopsFound.Add(nextPosition);
                continue;
            }

            Trailwalker(nextPosition, height, map, trailTopsFound);
        }
    }
}
