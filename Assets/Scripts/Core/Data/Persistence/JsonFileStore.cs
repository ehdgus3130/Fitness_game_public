using System.IO;

public sealed class JsonFileStore
{
    private readonly string _basePath;

    public JsonFileStore(string basePath)
    {
        _basePath = basePath;
    }

    private string PathOf(string fileName) => Path.Combine(_basePath, fileName);

    public bool Exists(string fileName) => File.Exists(PathOf(fileName));

    public string Read(string fileName) => File.ReadAllText(PathOf(fileName));

    public void Write(string fileName, string content) => File.WriteAllText(PathOf(fileName), content);

    public string FullPath(string fileName) => PathOf(fileName);
}
