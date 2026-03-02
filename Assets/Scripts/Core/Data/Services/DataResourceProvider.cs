using UnityEngine;

public interface IDataResourceProvider
{
    T Load<T>(string path) where T : Object;
    T[] LoadAll<T>(string path) where T : Object;
}

public sealed class UnityDataResourceProvider : IDataResourceProvider
{
    public T Load<T>(string path) where T : Object
    {
        return Resources.Load<T>(path);
    }

    public T[] LoadAll<T>(string path) where T : Object
    {
        return Resources.LoadAll<T>(path);
    }
}
