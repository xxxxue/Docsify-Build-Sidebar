using System.Text.Json;

using Spectre.Console;
using DocsifyBuildSidebar;

string _homePath = string.Empty;
string _sidebarFileName = "_sidebar.md";
string _readmeFileName = "README.md";
string _jsonConfigPath = "./Config/Config.json";

// 规则: 全名相等
List<string> _ignoreFileList = new()
        {
            "_sidebar.md", // 侧边栏文件
            "README.md" //侧边栏文件
        };

// 规则: 全名相等
List<string> _ignoreDirList = new();

// 规则: 名称包含
List<string> _ignoreDirNameContainList = new();

/// <summary>
/// 记录包含的子目录, 在这些目录中生成侧边栏文件
/// </summary>
List<string> _includeDirList = new();

/// <summary>
/// 文档目录级别
/// </summary>
int _level = 0;

/// <summary>
/// 不符合规则的文件 (发出警告)
/// </summary>
List<string> _warnFileList = new();

/// <summary>
/// 不生成 README.md
/// </summary>

bool _disableGenerateReadmeFile = false;

try
{
    Utils.ShowLogo();

    AnsiConsole.MarkupLine("[yellow]Initializing sidebar[/]...");

    // 初始化配置项
    Init();

    // 开始构建
    Build();

    ShowWarnFileList();

    Utils.WriteDivider();

    AnsiConsole.MarkupLine("○ [green]{0} ->>> Done![/]",Markup.Escape(_homePath));

    Utils.WriteDivider();

    Console.ReadLine();
}
catch (Exception e)
{
    AnsiConsole.WriteException(e);
    Console.ReadLine();
}


string Entry(string rootPath, bool isHome = false)
{
    var rootDir = new DirectoryInfo(rootPath);
    var sidebarData = string.Empty;

    // 首页 校正数据
    if (rootPath == _homePath)
    {
        sidebarData = string.Empty;
        _level = 0;
    }
    else
    {
        // 生成目录文件夹
        sidebarData += $"{Utils.GenerateSpace(_level)}- [{rootDir.Name}]({Utils.ReplaceSpace(rootDir.GetDirRelativePath())})\n";
        _level++;
    }

    // 获取目录中所有的文件夹和文件
    var fileList = Directory.EnumerateFileSystemEntries(rootPath).ToList();

    // 对文件夹与文件进行排序
    var sortFileList = new List<string>();
    var sortDirList = new List<string>();

    foreach (var item in fileList)
    {
        if (Utils.IsFile(item))
        {
            sortFileList.Add(item);
        }
        else if (Utils.IsDir(item))
        {
            sortDirList.Add(item);
        }
    }

    fileList.Clear();

    // 先放入 文件夹, 再放入 文件
    fileList.AddRange(sortDirList);
    fileList.AddRange(sortFileList);

    // 开始处理
    foreach (var item in fileList)
    {
        if (Utils.IsFile(item))
        {
            // 文件处理
            var file = new FileInfo(item);

            if (file.Extension != ".md")
            {
                _warnFileList.Add(item);
                continue;
            }

            if (_ignoreFileList.Contains(file.Name))
            {
                continue;
            }

            sidebarData += $"{Utils.GenerateSpace(_level)}- [{file.GetFileNameWithoutExtension()}]({Utils.ReplaceSpace(file.GetFileRelativePath())})\n";
        }
        else if (Utils.IsDir(item))
        {
            // 文件夹处理
            var dir = new DirectoryInfo(item);

            // 检查忽略
            if (!_ignoreDirList.Contains(dir.Name) && !_ignoreDirNameContainList.Exists(igString => dir.Name.Contains(igString)))
            {
                // 只有 home path 记录子目录, 生成子目录时不需要.
                if (isHome)
                {
                    _includeDirList.Add(dir.FullName); //记录文件夹地址, 用于之后生成子目录侧边栏
                }

                sidebarData += Entry(dir.FullName, isHome);

                Utils.WriteLogMessage($"[{dir.Name}] --- Done!");

                _level--;
            }
        }
    }

    return sidebarData;
}

void Build()
{
    Utils.SetCurrentDirectory(_homePath);
    // 生成home目录的 侧边栏
    var homeData = Entry(_homePath, true);

    WriteDataToFile(_homePath, homeData);

    // 生成 子目录的 侧边栏
    foreach (var item in _includeDirList)
    {
        _level = 0; // 层级归0
        var includeData = Entry(item);

        // 给 子目录添加 "返回上一级"
        var parentDir = new DirectoryInfo(item).Parent;
        if (parentDir.FullName == _homePath)
        {
            includeData = $"- [返回首页](/)\n" + includeData;
        }
        else
        {
            includeData = $"- [返回上一级 [{parentDir.Name}]]({parentDir.GetDirRelativePath()})\n" + includeData;
        }

        WriteDataToFile(item, includeData);
    }
}

void WriteDataToFile(string homePath, string data)
{
    var sidebarPath = Path.Combine(homePath, _sidebarFileName);
    File.WriteAllText(sidebarPath, data);

    if (_disableGenerateReadmeFile == false)
    {
        var readmePath = Path.Combine(homePath, _readmeFileName);
        File.WriteAllText(readmePath, data);
    }
}

void Init()
{
    AnsiConsole.MarkupLine("[yellow]Start ReadConfig...[/]");

    // 读取 Config.json
    var fileData = File.ReadAllText(_jsonConfigPath);

    // json 转为对象
    var config = JsonSerializer.Deserialize<MyConfigModel>(fileData);

    if (string.IsNullOrWhiteSpace(config.HomePath))
    {
        throw new Exception("Config.json HomePath 获取失败");
    }

    // 将 Config.json 内容保存到全局变量中
    _homePath = config.HomePath;
    _ignoreFileList.AddRange(config.IgnoreFile);
    _ignoreDirList.AddRange(config.IgnoreDir);
    _ignoreDirNameContainList.AddRange(config.IgnoreDirNameContain);
    _disableGenerateReadmeFile = config.DisableGenerateReadmeFile;

    Utils.WriteLogMessage("HomePath: " + _homePath);

    AnsiConsole.MarkupLine("[yellow]ReadConfig Done![/]");

    Utils.WriteDivider();
}

void ShowWarnFileList()
{
    if (_warnFileList.Count > 0)
    {
        Utils.WriteDivider();
        AnsiConsole.MarkupLine("[yellow]Excluded files:[/]");
        Console.WriteLine("-");
    }
    foreach (var item in _warnFileList)
    {
        AnsiConsole.MarkupLine("[yellow]{0}[/]",Markup.Escape(item));
    }
}
