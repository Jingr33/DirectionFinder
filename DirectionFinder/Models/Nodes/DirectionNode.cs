namespace DirectionFinder.Models.Nodes;

public sealed class DirectionNode : NodeBase
{
    private readonly List<NodeBase> children = new();

    public DirectionNode(DirectionNode? parent, string text) : base(parent, text) { }
    public DirectionNode(string text) : base(null, text) { }

    public IReadOnlyList<NodeBase> Children => children;

    public void AddChild(NodeBase child)
    {
        children.Add(child);
    }
}
