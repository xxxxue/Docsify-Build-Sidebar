using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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
