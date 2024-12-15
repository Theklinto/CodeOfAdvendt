using AOC.Utilities;

namespace AOC2024.Day_06;

public class Solver
{
    enum Direction
    {
        Right,
        Left,
        Up,
        Down,
    }
    class Position(int x, int y)
    {
        public int X { get; set; } = x;
        public int Y { get; set; } = y;

        public Position(Position start, Position offset) : this(start.X + offset.X, start.Y + offset.Y) { }

        public override string ToString() => $"{X},{Y}";
    }
    class Map : List<List<bool>>
    {
        public bool HasObstacleAt(Position position) => this[position.Y][position.X];
        public Map AddObstaclesAt(List<Position> positions)
        {
            Map map = (Map)this.Select(x => x.ToList()).ToList();
            foreach (Position obstaclePosition in positions)
            {
                map[obstaclePosition.Y][obstaclePosition.X] = true;
            }
            return map;
        }
        public List<Position> GetObstaclePositions()
        {
            List<Position> obstacles = this.SelectMany((row, rowIndex) => row.Select((obstacle, columnIndex) => (obstacle, columnIndex)).Where(column => column.obstacle).Select(column => new Position(column.columnIndex, rowIndex))).ToList();
            return obstacles;
        }
    }
    class Loop(Position topLeftObstacle, Position topRightObstacle, Position bottomLeftObstacle, Position bottomrightObstacle)
    {
        public Position TopLeftObstacle { get; set; } = topLeftObstacle;
        public Position TopRightObstacle { get; set; } = topRightObstacle;
        public Position BottomLeftObstacle { get; set; } = bottomLeftObstacle;
        public Position BottomRightObstacle { get; set; } = bottomrightObstacle;
    }

    private static (Map Map, Position GuardPosition) ReadMap(string file)
    {
        StreamReader streamReader = DataParser.GetStreamReader(file);

        Map map = [];
        Position? guardPosition = null;
        int rowIndex = 0;
        while (!streamReader.EndOfStream)
        {
            string line = streamReader.ReadLine() ?? throw new Exception("");
            char[] positions = line.ToCharArray();
            List<bool> obstacles = positions.Select(x => x == '#').ToList();
            if (guardPosition is null)
            {
                int guardIndex = Array.IndexOf(positions, '^');
                if (guardIndex != -1)
                {
                    guardPosition = new(guardIndex, rowIndex);
                }
                else
                {
                    rowIndex++;
                }
            }
            map.Add(obstacles);
        }

        return (map, guardPosition ?? new(0, 0));
    }
    private static Position GetNextPosition(Position guardPosition, Direction direction)
    {
        Position move = direction switch
        {
            Direction.Up => new(0, -1),
            Direction.Down => new(0, 1),
            Direction.Left => new(-1, 0),
            Direction.Right => new(1, 0),
            _ => new(0, 0)
        };

        return new(guardPosition.X + move.X, guardPosition.Y + move.Y);
    }
    private static Direction Rotate(Direction currentDirection)
    {
        return currentDirection switch
        {
            Direction.Up => Direction.Right,
            Direction.Down => Direction.Left,
            Direction.Left => Direction.Up,
            Direction.Right => Direction.Down,
            _ => throw new Exception("")
        };
    }

    [Theory]
    [InlineData("example.txt", 41)]
    [InlineData("input.txt", 5242)]
    public static void GetDistinctPositions(string file, int expectedResult)
    {
        (Map map, Position guardPosition) = ReadMap(file);

        bool guardOutOfBounds = false;
        Dictionary<string, bool> visitedPositions = [];
        Direction direction = Direction.Up;

        visitedPositions[guardPosition.ToString()] = true;

        while (!guardOutOfBounds)
        {
            try
            {
                Position newGuardPosition = GetNextPosition(guardPosition, direction);
                if (map[newGuardPosition.Y][newGuardPosition.X])
                {
                    direction = Rotate(direction);
                    newGuardPosition = GetNextPosition(guardPosition, direction);
                    if (map[newGuardPosition.Y][newGuardPosition.X])
                    {
                        continue;
                    }
                }
                guardPosition = newGuardPosition;
                visitedPositions[newGuardPosition.ToString()] = true;
            }
            catch
            {
                guardOutOfBounds = true;
            }
        }

        int distinctPositions = visitedPositions.Count;

        Assert.Equal(expectedResult, distinctPositions);
    }


    [Theory]
    [InlineData("example.txt", 6)]
    [InlineData("input.txt", -1)]
    public static void FindAvaiableLoops(string file, int expectedResult)
    {
        (Map map, Position guardPosition) = ReadMap(file);

        List<Loop> confirmedLoops = [];
        List<Loop> possibleLoops = map.GetObstaclePositions().SelectMany(obstacle => GetPossibleLoopPatterns(map, obstacle)).ToList();

        foreach (Loop possibleLoop in possibleLoops)
        {
            bool guardOutOfBounds = false;
            Direction direction = Direction.Up;
            Dictionary<string, int> loopObstacleHitCounter = new()
            {
                [possibleLoop.TopRightObstacle.ToString()] = 0,
                [possibleLoop.TopLeftObstacle.ToString()] = 0,
                [possibleLoop.BottomRightObstacle.ToString()] = 0,
                [possibleLoop.BottomLeftObstacle.ToString()] = 0,
            };
            void obstacleHit(Position position)
            {
                if (loopObstacleHitCounter.ContainsKey(position.ToString()))
                {
                    loopObstacleHitCounter[position.ToString()]++;
                }
            }
            while (!guardOutOfBounds)
            {
                try
                {
                    Position newGuardPosition = GetNextPosition(guardPosition, direction);
                    if (map.HasObstacleAt(newGuardPosition))
                    {
                        obstacleHit(newGuardPosition);
                        direction = Rotate(direction);
                        newGuardPosition = GetNextPosition(guardPosition, direction);
                        if (map.HasObstacleAt(newGuardPosition))
                        {
                            obstacleHit(newGuardPosition);
                            continue;
                        }
                    }
                    guardPosition = newGuardPosition;

                    bool confirmedLoop = loopObstacleHitCounter.All(obstacle => obstacle.Value > 5);
                    if (confirmedLoop)
                    {
                        confirmedLoops.Add(possibleLoop);
                        break;
                    }
                }
                catch
                {
                    guardOutOfBounds = true;
                }
            }
        }

        Assert.Equal(expectedResult, confirmedLoops.Count);
    }

    class LoopPattern(Direction direction, Position increment, Position startOffset)
    {
        public Direction Direction { get; set; } = direction;
        public Position Increment { get; set; } = increment;
        public Position StartOffset { get; set; } = startOffset;
    }

    private static readonly List<LoopPattern> LoopPatternOffsets = [
        new(Direction.Down, new(0, 1), new(-1, 0)),
        new(Direction.Left, new(-1, 0), new(0, -1)),
        new(Direction.Up, new(0, -1), new(1, 0)),
        new(Direction.Right, new(1, 0), new(0, 1))
    ];


    private static List<LoopPattern> GetLoopPatternOffsetsByStartDirection(Direction direction)
    {
        int directionIndex = LoopPatternOffsets.FindIndex(x => x.Direction == direction);
        List<LoopPattern> pushToEnd = [.. LoopPatternOffsets.GetRange(0, directionIndex)];
        List<LoopPattern> patterns = [.. LoopPatternOffsets.GetRange(directionIndex, LoopPatternOffsets.Count - directionIndex), .. pushToEnd];
        patterns.RemoveAt(0);

        return patterns;
    }

    private static List<Loop> GetPossibleLoopPatterns(Map map, Position startObstacle)
    {
        List<Loop> possibleLoops = [];
        foreach (Direction startDirection in Enum.GetValues<Direction>())
        {
            List<LoopPattern> loopPatterns = GetLoopPatternOffsetsByStartDirection(direction: startDirection);
            List<Position> positions = [startObstacle];
            Dictionary<string, bool> obstacles = new()
            {
                [startObstacle.ToString()] = true
            };
            Position lastPosition = startObstacle;
            foreach (LoopPattern loopPattern in loopPatterns)
            {
                Position? nextObstaclePosition = GetObstaclePosition(map, new(lastPosition, loopPattern.StartOffset), loopPattern.Increment);
                //obstacles[nextObstaclePosition.ToString()];
                positions.Add(nextObstaclePosition);
                lastPosition = nextObstaclePosition;
            }

            if (positions.Count == 4)
            {
                Loop possibleLoop = new(
                    positions[0],
                    positions[1],
                    positions[2],
                    positions[3]
                );
                possibleLoops.Add(possibleLoop);
            }
        }

        return possibleLoops;
    }

    private static Position? GetObstaclePosition(Map map, Position start, Position increments)
    {
        Position? obstacle = null;
        Position? lastPosition = null;
        while (obstacle is null)
        {
            try
            {
                Position nextPosition = lastPosition is null ? start : new(lastPosition, increments);
                if (map.HasObstacleAt(nextPosition))
                {
                    obstacle = nextPosition;
                }
                lastPosition = nextPosition;

            }
            catch
            {
                break;
            }
        }

        return obstacle;
    }
}
