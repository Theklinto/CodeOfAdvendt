using AOC.Utilities;
using System.Data;

namespace AOC2024.Day_05;

public class Solver
{
    private class PrintRule(int FirstPage, int LastPage)
    {
        public int FirstPage { get; set; } = FirstPage;
        public int LastPage { get; set; } = LastPage;
        public bool ValidateRule(List<int> queue)
        {
            int firstPageIndex = queue.IndexOf(FirstPage);
            int lastPageIndex = queue.IndexOf(LastPage);
            return firstPageIndex == -1 || lastPageIndex == -1 || lastPageIndex > firstPageIndex;
        }


    }

    private class PrintQueue(List<int> Queue, List<PrintRule> Rules)
    {
        class Page(int PageNumber) : IComparable<Page>
        {
            public int PageNumber { get; set; } = PageNumber;
            public List<int> AfterPages { get; set; } = [];
            public List<int> BeforePages { get; set; } = [];

            public int CompareTo(Page? other)
            {
                if (other is null)
                {
                    return 1;
                }
                return other.BeforePages.Any(pageNumber => pageNumber == PageNumber) ? -1 : other.AfterPages.Any(pageNumber => pageNumber == PageNumber) ? 1 : 0;
            }

            public override string ToString()
            {
                return PageNumber.ToString();
            }
        }
        public List<int> CorrectQueue()
        {
            List<Page> pages = [];
            for (int i = 0; i < Queue.Count; i++)
            {
                int pageNumber = Queue[i];
                Page page = new(pageNumber)
                {
                    BeforePages = Rules.Where(rule => rule.LastPage == pageNumber).Select(rule => rule.FirstPage).ToList(),
                    AfterPages = Rules.Where(rule => rule.FirstPage == pageNumber).Select(rule => rule.LastPage).ToList()
                };
                pages.Add(page);
            }

            pages = [.. pages.Order()];
            return [.. pages.Select(x => x.PageNumber)];
        }
    }

    private static (List<List<int>> Rueue, List<PrintRule> Rules) ReadFile(string file)
    {
        StreamReader streamReader = DataParser.GetStreamReader(file);
        List<PrintRule> printRules = [];
        List<List<int>> queues = [];
        while (!streamReader.EndOfStream)
        {
            string line = streamReader.ReadLine() ?? throw new Exception("");
            if (line.Contains('|'))
            {
                List<int> rule = line.Split('|').Select(int.Parse).ToList();
                printRules.Add(new(rule[0], rule[1]));
            }
            else if (line.Contains(','))
            {
                queues.Add([.. line.Split(',').Select(int.Parse)]);
            }
        }
        return (queues, printRules);
    }

    [Theory]
    [InlineData("example.txt", 143)]
    [InlineData("input.txt", 7365)]
    public static void GetPageSumOfValidQueues(string file, int expectedResult)
    {
        (List<List<int>> queues, List<PrintRule> printRules) = ReadFile(file);

        List<List<int>> validatedQueues = queues
            .Where(queue => printRules
            .All(rule => rule.ValidateRule(queue)))
            .ToList();
        int sumOfMiddlePages = validatedQueues.Sum(queue => queue[(queue.Count - 1) / 2]);

        Assert.Equal(expectedResult, sumOfMiddlePages);
    }

    [Theory]
    [InlineData("example.txt", 123)]
    [InlineData("input.txt", 5770)]
    public static void GetPageSumOfCorrectedQueues(string file, int expectedResult)
    {
        (List<List<int>> queues, List<PrintRule> rules) = ReadFile(file);

        List<List<int>> invalidQueues = queues
            .Where(queue => rules.Any(rule => !rule.ValidateRule(queue)))
            .ToList();
        List<List<int>> correctedQueues = [];

        foreach (var queue in invalidQueues)
        {
            PrintQueue printQueue = new(queue, rules);
            correctedQueues.Add(printQueue.CorrectQueue());
        }

        int sumOfMiddlePages = correctedQueues.Sum(queue => queue[(queue.Count - 1) / 2]);


        Assert.Equal(sumOfMiddlePages, expectedResult);
    }
}