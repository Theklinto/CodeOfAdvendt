using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AOC.Utilities;

public class DataParser
{
    public static StreamReader GetStreamReader(string relativePath, [CallerFilePath] string callerFilePath = "")
    {
        string filePath = Path.GetFullPath(Path.Combine(Directory.GetParent(callerFilePath)?.FullName ?? "", relativePath));

        FileStream fileStream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        return new(fileStream);
    }

    public static List<string> GetLinesFromFile(string relativePath, [CallerFilePath] string callerFilePath = "")
    {
        StreamReader streamReader = GetStreamReader(relativePath, callerFilePath);
        List<string> lines = [];
        while (!streamReader.EndOfStream)
        {
            string line = streamReader.ReadLine() ?? throw new Exception("Can't read line from file");
            lines.Add(line);
        }
        return lines;
    }
}
