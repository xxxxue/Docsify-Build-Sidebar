using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Spectre.Console;

namespace DocsifyBuildSidebar
{
    internal class Program
    {
        private static string _homePath = string.Empty;
        private static string _sidebarFileName = "_sidebar.md";
        private static string _readmeFileName = "README.md";
        private static string _jsonConfigPath = "./Config/Config.json";
        private static List<string> _ignoreDirList = new List<string>(){
            ".git", //git目录
            };

        private static List<string> _ignoreFileList = new List<string>(){
            "_sidebar.md", // 侧边栏文件
            "README.md",//侧边栏文件
            };

        private static List<string> _ignoreStringList = new List<string>(){
            ".assets" //Typora软件 存放图片的目录 *.assets
            };

        /// <summary>
        /// 记录包含的子目录, 在这些目录中生成侧边栏文件
        /// </summary>
        private static List<string> _includeDirList = new List<string>();

        /// <summary>
        /// 文档目录级别
        /// </summary>
        private static int _level = 0;

        private static string Entry(string rootPath, bool isHome = false)
        {

            var rootDir = new DirectoryInfo(rootPath);
            var sidebarData = string.Empty;

            // 生成目录文件夹
            sidebarData += $"{Utils.GenerateSpace(_level)}- [{rootDir.Name}]({Utils.ReplaceSpace(rootDir.GetDirRelativePath())})\n";
            _level++;

            // 首页 校正数据
            if (rootPath == _homePath)
            {
                sidebarData = string.Empty;
                _level = 0;
            }
            // 获取目录中所有的文件夹和文件
            var fileList = Directory.EnumerateFileSystemEntries(rootPath).ToList();
            foreach (var item in fileList)
            {
                if (Utils.IsFile(item))
                {
                    // 文件处理
                    var file = new FileInfo(item);

                    // 判断扩展名 && 检查忽略
                    if (file.Extension == ".md" && !_ignoreFileList.Contains(file.Name))
                    {
                        sidebarData += $"{Utils.GenerateSpace(_level)}- [{file.GetFileNameWithoutExtension()}]({Utils.ReplaceSpace(file.GetFileRelativePath())})\n";
                    }
                }
                else if (Utils.IsDir(item))
                {
                    // 文件夹处理
                    var dir = new DirectoryInfo(item);
                    // 检查忽略
                    if (!_ignoreDirList.Contains(dir.Name) && !_ignoreStringList.Exists(igString => dir.Name.Contains(igString)))
                    {
                        // 只有 home path 记录子目录, 生成子目录时不需要.
                        if (isHome)
                        {
                            _includeDirList.Add(dir.FullName); //记录文件夹地址, 用于之后生成子目录侧边栏
                        }

                        sidebarData += Entry(dir.FullName, isHome);
                        Utils.WriteLogMessage($"[{dir.Name}] Done!");
                        _level--;
                    }
                }
            }

            return sidebarData;
        }

        private static void Build()
        {
            Utils.SetCurrentDirectory(_homePath);
            // 生成home目录的 侧边栏
            var homeData = Entry(_homePath, true);

            // Console.WriteLine($"home menu :\n{homeData}");
            WriteDataToFile(_homePath, homeData);           
            Utils.WriteLogMessage("[home] Done!");

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
                //Console.WriteLine($"child menu :\n{includeData}");

            }
        }

        /// <summary>
        /// 将 数据 写入文件
        /// </summary>
        /// <param name="homePath"></param>
        /// <param name="data"></param>
        private static void WriteDataToFile(string homePath, string data)
        {
            string sidebarPath = Path.Combine(homePath, _sidebarFileName);
            string readmePath = Path.Combine(homePath, _readmeFileName);

            File.WriteAllText(sidebarPath, data);
            File.WriteAllText(readmePath, data);
        }

        private static void Init()
        {
            AnsiConsole.MarkupLine("[yellow]Start ReadConfig...[/]");
            var configJson = new JsonConfigHelper(_jsonConfigPath);

            var homePath = configJson["HomePath"];

            if (string.IsNullOrWhiteSpace(homePath))
            {
                throw new Exception("Config.json HomePath 获取失败");
            }
            _homePath = homePath;

            Utils.WriteLogMessage("HomePath: " + _homePath);
            AnsiConsole.MarkupLine("[yellow]ReadConfig Done![/]");
            AnsiConsole.WriteLine();
        }

        private static void Main(string[] args)
        {
            try
            {
                Utils.ShowLogo();
                AnsiConsole.MarkupLine("[yellow]Initializing sidebar[/]...");
                Init();
                Build();
                AnsiConsole.MarkupLine("[green]Done![/]");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                AnsiConsole.WriteException(e);
                Console.ReadLine();
            }
        }

    }
}