namespace DirectionFinder.Models.Nodes;

public abstract class NodeBase(NodeBase? parent, string text)
{
    public NodeBase? Parent { get; init; } = parent;
    public string Text { get; init; } = text;
    public List<NodeBase> Children { get; } = [];
}
