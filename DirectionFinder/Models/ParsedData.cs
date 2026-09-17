using DirectionFinder.Models.Nodes;

namespace DirectionFinder.Models;

public record ParsedData(DirectionNode RootNode, ItemNode[] OrderedItemNodes);
