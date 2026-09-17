using DirectionFinder.Constants;
using DirectionFinder.Models;
using DirectionFinder.Models.Nodes;
using DirectionFinder.Parsers;

public class Program
{
    public static void Main(string[] args)
    {
        try
        {
            var filePath = Path.Combine(AppContext.BaseDirectory, SourceDataConstants.SourceDataFileName);
            var parsedData = LoadAndParseSourceData(filePath);

            DisplayItemNodes(parsedData);

            Console.WriteLine("What item would you like to search for?");
            var userInput = Console.ReadLine();

            if (!int.TryParse(userInput, out var inputNodeNumber))
            {
                throw new ArgumentException("User input is not a valid number");
            }

            var nodeItemIndex = inputNodeNumber - 1;

            if (nodeItemIndex < 0 || nodeItemIndex >= parsedData.OrderedItemNodes.Count())
            {
                throw new ArgumentOutOfRangeException(null, "Number of a specified item is not in the list");
            }

            Console.WriteLine();
            var selectedNode = parsedData.OrderedItemNodes[nodeItemIndex];
            DisplayParentInstructions(selectedNode.Parent!);
        }
        catch (Exception ex)
        {
            Console.WriteLine();
            Console.WriteLine($"Error: {ex.Message}, application terminated");
            Environment.Exit(1);
        }
    }

    private static ParsedData LoadAndParseSourceData(string filePath)
    {
        try
        {
            var lines = File.ReadAllLines(filePath);
            return FileDataParser.Parse(lines);
        }
        catch (FileNotFoundException)
        {
            throw new FileNotFoundException($"File {filePath} was not found");
        }
        catch (UnauthorizedAccessException)
        {
            throw new UnauthorizedAccessException($"Access to the file '{filePath}' is denied");
        }
        catch (IOException)
        {
            throw new IOException($"The file {filePath} could not be read");
        }
    }

    private static void DisplayItemNodes(ParsedData parsedData)
    {
        Console.WriteLine("Available items:\n");

        for (int i = 0; i < parsedData.OrderedItemNodes.Length; i++)
        {
            Console.WriteLine($"[{i + 1}] - {parsedData.OrderedItemNodes[i].Text}");
        }

        Console.WriteLine();
    }

    private static void DisplayParentInstructions(NodeBase node)
    {
        if (node.Parent != null)
        {
            DisplayParentInstructions(node.Parent);
        }

        Console.WriteLine(node.Text);
    }

}