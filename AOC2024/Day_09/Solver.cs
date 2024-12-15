using AOC.Utilities;

namespace AOC2024.Day_09;

public class Solver
{
    class Byte(int? fileId, int index)
    {
        public int? FileId { get; set; } = fileId;
        public int Index { get; set; } = index;
        public override string ToString() => FileId?.ToString() ?? "";
    }

    [Theory]
    [InlineData("example.txt", 1928)]
    [InlineData("input.txt", 6288707484810)]
    public static void DefragmentHarddisk(string filePath, long expectedResult)
    {
        StreamReader streamReader = DataParser.GetStreamReader(filePath);
        string harddiskContent = streamReader.ReadLine() ?? throw new Exception("");
        List<int> fragments = harddiskContent.ToCharArray().Select(x => int.Parse(new string([x]))).ToList();
        List<Byte> bytes = [];
        int fileId = 0;
        int index = 0;
        for (int i = 0; i < fragments.Count; i++)
        {
            bool freeSpace = i % 2 == 1;
            for (int j = 0; j < fragments[i]; j++)
            {
                Byte @byte = new(freeSpace ? null : fileId, index);
                bytes.Add(@byte);
                index++;
            }

            if (freeSpace is false)
            {
                fileId++;
            }
        }

        bool spaceAvailable = true;
        while (spaceAvailable)
        {
            Byte? avaiableByte = bytes.FirstOrDefault(x => x.FileId is null);
            spaceAvailable = avaiableByte is not null;

            if (avaiableByte is not null)
            {
                int lastByteIndex = bytes.Count - 1;
                Byte @byte = bytes[lastByteIndex];
                bytes.RemoveAt(lastByteIndex);

                if (@byte.FileId is not null)
                {
                    avaiableByte.FileId = @byte.FileId;
                }
            }
            else
            {
                spaceAvailable = false;
            }
        }

        long checksum = bytes.Select((@byte, index) => (@byte.FileId ?? 0L) * index).Sum();

        Assert.Equal(expectedResult, checksum);
    }

    [Theory]
    [InlineData("example.txt", 2858)]
    [InlineData("input.txt", 6311837662089)]
    public static void DefragmentHarddiskByFile(string filePath, long expectedResult)
    {
        List<Byte> bytes = [];
        List<List<Byte>> freeSpaceSegments = [];
        Dictionary<int, int> fileSizes = [];
        {
            StreamReader streamReader = DataParser.GetStreamReader(filePath);
            string harddiskContent = streamReader.ReadLine() ?? throw new Exception("");
            List<int> fragments = harddiskContent.ToCharArray().Select(x => int.Parse(new string([x]))).ToList();
            int fileId = 0;
            int index = 0;
            for (int i = 0; i < fragments.Count; i++)
            {
                bool freeSpace = i % 2 == 1;
                int fileSize = 0;
                List<Byte> fileBytes = [];

                for (fileSize = 0; fileSize < fragments[i]; fileSize++)
                {
                    Byte @byte = new(freeSpace ? null : fileId, index);
                    fileBytes.Add(@byte);
                    index++;
                }

                if (fileBytes.Count == 0)
                {
                    continue;
                }

                if (freeSpace is false)
                {
                    fileSizes[fileId] = fileSize;
                    fileId++;
                }
                else
                {
                    freeSpaceSegments.Add(fileBytes);
                }
                bytes.AddRange(fileBytes);
            }
        }

        fileSizes = fileSizes.OrderByDescending(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
        foreach ((int fileId, int fileSize) in fileSizes)
        {
            List<Byte> file = bytes.Where(x => x.FileId == fileId).ToList();
            int fileStartingIndex = file.Select(x => x.Index).FirstOrDefault();
            List<Byte>? freeSpaceSegment = freeSpaceSegments.FirstOrDefault(x => x.Count >= fileSize && x.All(y => y.Index < fileStartingIndex));
            if (freeSpaceSegment is null)
                continue;


            for (int i = 0; i < fileSize; i++)
            {
                freeSpaceSegment[i].FileId = fileId;
                file[i].FileId = null;
            }

            freeSpaceSegment.RemoveRange(0, fileSize);
            if (freeSpaceSegment.Count == 0)
            {
                freeSpaceSegments.Remove(freeSpaceSegment);
            }

        }

        long checksum = bytes.Select((@byte, index) => (@byte.FileId ?? 0L) * index).Sum();

        Assert.Equal(expectedResult, checksum);
    }
}
