using System.Text.Json.Serialization;

namespace DocsifyBuildSidebar;

public class MyConfigModel
{
    public string HomePath { get; set; } = string.Empty;
    public List<string> IgnoreFile { get; set; } = new List<string>();
    public List<string> IgnoreDir { get; set; } = new List<string>();
    public List<string> IgnoreDirNameContain { get; set; } = new List<string>();
    public bool DisableGenerateReadmeFile { get; set; } = false;
}

[JsonSerializable(typeof(MyConfigModel))]
[JsonSourceGenerationOptions(ReadCommentHandling = System.Text.Json.JsonCommentHandling.Skip)] // 支持带注释的 json
public partial class SourceGenerationContext : JsonSerializerContext { }