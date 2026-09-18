using DirectionFinder.Models;
using DirectionFinder.Models.Nodes;
using DirectionFinder.Validators;

namespace DirectionFinder.Parsers;

public sealed class FileDataParser
{
    public static ParsedData Parse(IEnumerable<string> lines)
    {
        var itemNodes = new Dictionary<string, ItemNode>();
        var nodeStack = new Stack<DirectionNode>();
        DirectionNode? rootNode = null;

        foreach (var line in lines)
        {
            var parsedLine = ParseLine(line);

            DataValidator.ValidateNodeHierarchy(line, parsedLine.Depth, nodeStack.Count);
            DataValidator.ValidateSingleRootNode(line, parsedLine.Depth, rootNode);

            while (nodeStack.Count > parsedLine.Depth)
            {
                var completeDirection = nodeStack.Pop();
                DataValidator.ValidateDirectionPathEndsWithItem(completeDirection);
            }

            NodeBase node;

            if (nodeStack.Count == 0)
            {
                DataValidator.ValidateRootNodeIsDirection(parsedLine);
                rootNode = new DirectionNode(parsedLine.Text);
                node = rootNode;
                nodeStack.Push((DirectionNode)node);
            }
            else
            {
                var parentNode = nodeStack.Peek();

                if (parsedLine.IsItem)
                {
                    var itemNode = new ItemNode(parentNode, parsedLine.Text);

                    if (!itemNodes.TryAdd(itemNode.Text, itemNode))
                    {
                        throw new InvalidOperationException($"This is a duplicate item node '{itemNode.Text}'");
                    }

                    node = itemNode;
                }
                else
                {
                    node = new DirectionNode(parentNode, parsedLine.Text);
                    nodeStack.Push((DirectionNode)node);
                }

                parentNode.AddChild(node);
            }
        }

        while (nodeStack.Count > 0)
        {
            var completeDirection = nodeStack.Pop();
            DataValidator.ValidateDirectionPathEndsWithItem(completeDirection);
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
        var content = line[contentOffset..].TrimStart();

        if (content.StartsWith("Item: "))
        {
            var text = content["Item: ".Length..].Trim();

            DataValidator.ValidateNodeText(line, text);

            return new ParsedLine(depth, text, true);
        }
        else if (content.StartsWith("+ "))
        {
            var text = content["+ ".Length..].Trim();

            DataValidator.ValidateNodeText(line, text);

            return new ParsedLine(depth, text, false);
        }

        throw new InvalidOperationException($"Invalid direction/item prefix in line: '{line}'");

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
}
