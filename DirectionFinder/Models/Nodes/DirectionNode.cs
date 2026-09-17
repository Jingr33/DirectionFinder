namespace DirectionFinder.Models.Nodes;

public class DirectionNode : NodeBase
{
    public DirectionNode(NodeBase? parent, string text) : base(parent, text) { }
    public DirectionNode(string text) : base(null, text) { }
}
