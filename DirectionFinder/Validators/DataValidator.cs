using DirectionFinder.Models.Nodes;
using DirectionFinder.Parsers;

namespace DirectionFinder.Validators;

public static class DataValidator
{
    public static void ValidateEmptyDataLine(string line)
    {
        if (string.IsNullOrWhiteSpace(line))
        {
            throw new InvalidDataException("There is an empty or whitespace data line");
        }
    }

    public static void ValidateRootNode(DirectionNode? rootNode)
    {
        if (rootNode == null)
        {
            throw new InvalidDataException("First node is invalid or doesn't exist");
        }
    }

    public static void ValidateRootNodeIsDirection(ParsedLine parsedLine)
    {
        if (parsedLine.IsItem)
        {
            throw new InvalidDataException("Root node is not a direction");
        }
    }

    public static void ValidateNodeHierarchy(string line, int currentNodeDepth, int lastNodeDepth)
    {
        if (currentNodeDepth > lastNodeDepth)
        {
            throw new InvalidDataException($"Invalid hierarchy at line '{line}'");
        }
    }

    public static void ValidateSingleRootNode(string line, int nodeDepth, DirectionNode? rootNode)
    {
        if (nodeDepth == 0 && rootNode is not null)
        {
            throw new InvalidCastException($"Multiple root nodes are not allowed at line '{line}'");
        }
    }

    public static void ValidateDirectionPathEndsWithItem(DirectionNode directionNode)
    {
        if (directionNode.Children.Count == 0)
        {
            throw new InvalidDataException($"Direction '{directionNode.Text}' does not lead to an item");
        }
    }

    public static void ValidateBranchPrefix(string line, int position)
    {
        if (position + 3 >= line.Length)
        {
            throw new InvalidDataException($"Line '{line}' contains an invalid branch prefix");
        }

        var branch = line[position..(position + 3)];

        if (branch != "├──" && branch != "└──")
        {
            throw new InvalidDataException($"Invalid branch prefix '{branch}' at position '{position}' in line '{line}'");
        }
    }

    public static void ValidateNodeText(string line, string nodeText)
    {
        if (string.IsNullOrWhiteSpace(nodeText))
        {
            throw new InvalidDataException($"Node text is empty or whitespace in line '{line}'");
        }
    }
}
