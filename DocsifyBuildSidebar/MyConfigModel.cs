using System.Collections.Generic;

namespace DocsifyBuildSidebar
{
    public class MyConfigModel
    {
        public string HomePath { get; set; }

        public List<string> IgnoreFile { get; set; }
        public List<string> IgnoreDir { get; set; }
        public List<string> IgnoreDirNameContain { get; set; }
    }
}