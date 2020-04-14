using System;

[AttributeUsage(AttributeTargets.Class)]
public class InteractableNodeAttribute : Attribute
{
    public string defaultNodeName;
}
