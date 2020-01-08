public interface ISerializable<T>
{
    string Serialize(T instance);
    T Deserialize(string json);
}