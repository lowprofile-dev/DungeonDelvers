using System;

[Serializable]
public abstract class MasteryEffect
{
    public abstract void ApplyEffect(Character character, int level);
}
