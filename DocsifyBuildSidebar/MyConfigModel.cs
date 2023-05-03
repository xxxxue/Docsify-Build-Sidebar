using System.Collections.Generic;

namespace DocsifyBuildSidebar;

public class MyConfigModel
{
    public string HomePath { get; set; } = string.Empty;

    public List<string> IgnoreFile { get; set; } = new List<string>();
    public List<string> IgnoreDir { get; set; } = new List<string>();
    public List<string> IgnoreDirNameContain { get; set; } = new List<string>();

    public bool DisableGenerateReadmeFile { get; set; } = false;
}