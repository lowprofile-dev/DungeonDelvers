using System;

[Obsolete]
public interface ISerializable<T>
{
    string Serialize();
    T Deserialize(string json);
}