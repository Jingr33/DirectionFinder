using DirectionFinder.Models;
using DirectionFinder.Models.Nodes;
using DirectionFinder.Validators;

namespace DirectionFinder.Parsers;

public sealed class FileDataParser
{
    public static ParsedData Parse(IEnumerable<string> lines)
    {
        var itemNodes = new Dictionary<string, ItemNode>();
        var nodeStack = new Stack<NodeBase>();
        DirectionNode? rootNode = null;

        foreach (var line in lines)
        {
            var parsedLine = ParseLine(line);

            DataValidator.ValidateNodeHierarchy(line, parsedLine.Depth, nodeStack.Count);

            while (nodeStack.Count > parsedLine.Depth)
            {
                nodeStack.Pop();
            }

            NodeBase node;

            if (nodeStack.Count == 0)
            {
                DataValidator.ValidateRootNodeIsDirection(parsedLine);
                rootNode = new DirectionNode(parsedLine.Text);
                node = rootNode;
            }
            else
            {
                var parentNode = nodeStack.Peek();

                if (parsedLine.IsItem)
                {
                    var itemNode = new ItemNode(parentNode, parsedLine.Text);

                    if (!itemNodes.TryAdd(itemNode.Text, itemNode))
                    {
                        throw new InvalidOperationException($"This is a duplicate item node {itemNode.Text}.");
                    }

                    node = itemNode;
                }
                else
                {
                    node = new DirectionNode(parentNode, parsedLine.Text);
                }

                parentNode.Children.Add(node);
            }

            nodeStack.Push(node);

        }

        DataValidator.ValidateRootNode(rootNode);

        var orderedItemNodes = itemNodes.Values
            .OrderBy(item => item.Text, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return new ParsedData(rootNode!, orderedItemNodes);
    }

    private static ParsedLine ParseLine(string line)
    {
        var (depth, contentOffset) = ParsePrefix(line);
        var content = GetContent(line, contentOffset);

        if (content.StartsWith("Item: "))
        {
            content = content["Item: ".Length..];
            return new ParsedLine(depth, content, true);
        }
        else if (content.StartsWith("+ "))
        {
            content = content["+ ".Length..];
            return new ParsedLine(depth, content, false);
        }

        throw new InvalidOperationException($"Invalid line prefix in line: {line}");

    }

    private static (int depth, int contentOffset) ParsePrefix(string line)
    {
        DataValidator.ValidateEmptyDataLine(line);

        if (line.StartsWith("+ ") || line.StartsWith("Item: "))
        {
            return (0, 0);
        }

        var depth = 1;
        var position = 0;

        while (position + 3 < line.Length)
        {
            var prefix = line[position..(position + 3)];

            if (prefix != "|  " && prefix != "   ")
            {
                break;
            }

            depth++;
            position += 3;
        }

        DataValidator.ValidateBranchPrefix(line, position);

        position += 3;

        return (depth, position);
    }

    private static string GetContent(string line, int offset)
    {
        var nodeText = line[offset..].Trim();

        DataValidator.ValidateNodeText(line, nodeText);

        return nodeText;
    }
}
