using JetBrains.Annotations;
using XNode;

public abstract class InteractionBase : Node
{
    [CanBeNull] public abstract InteractionBase GetNextNode();
}
