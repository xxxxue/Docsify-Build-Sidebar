using System;
using System.IO;
using System.Text.RegularExpressions;

using Spectre.Console;

namespace DocsifyBuildSidebar;

public static class Utils
{
    /// <summary>
    /// 判断是否是文件
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static bool IsFile(string path)
    {
        return File.Exists(path);
    }

    /// <summary>
    /// 判断是否是文件夹
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static bool IsDir(string path)
    {
        return Directory.Exists(path);
    }

    /// <summary>
    /// 生成markdown中的层级 空格
    /// </summary>
    /// <param name="n"></param>
    /// <returns></returns>
    public static string GenerateSpace(int n)
    {
        var res = "";
        while (n-- > 0)
        {
            res += "  ";
        }

        return res;
    }

    /// <summary>
    /// 替换空格为%20
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static string ReplaceSpace(string data)
    {
        return Regex.Replace(data, @"\s{1,1}", "%20");
    }

    /// <summary>
    /// 获取不包含扩展名的 文件名
    /// </summary>
    /// <param name="file">文件对象</param>
    /// <returns></returns>
    public static string GetFileNameWithoutExtension(this FileInfo file)
    {
        return Path.GetFileNameWithoutExtension(file.Name);
    }

    /// <summary>
    /// 获取文件的相对目录 (扩展方法)
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    public static string GetFileRelativePath(this FileInfo file)
    {
       return file.FullName.Replace(GetCurrentDirectory(), "").TrimStart('\\').Replace("\\", "/");
    }

    /// <summary>
    /// 获取文件夹的相对目录 (扩展方法)
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    public static string GetDirRelativePath(this DirectoryInfo dir)
    {
        string path = dir.FullName.Replace(GetCurrentDirectory(), "").TrimStart('\\');
        if (!string.IsNullOrWhiteSpace(path))
        {
            path += path[^1] == '\\' ? "" : "\\"; // 补齐地址末尾的 "\"
            path = path.Replace("\\", "/");
        }
        return path;
    }

    /// <summary>
    /// 获取当前运行目录
    /// </summary>
    /// <returns></returns>
    public static string GetCurrentDirectory()
    {
        return System.Environment.CurrentDirectory;
    }

    /// <summary>
    /// 设置运行目录
    /// </summary>
    /// <param name="homePath"></param>
    /// <returns></returns>
    public static bool SetCurrentDirectory(string homePath)
    {
        System.Environment.CurrentDirectory = homePath;
        return System.Environment.CurrentDirectory == homePath;
    }

    /// <summary>
    /// 控制台日志
    /// </summary>
    /// <param name="message"></param>
    public static void WriteLogMessage(string message)
    {
        AnsiConsole.MarkupLine("[grey]LOG:[/]{0}", Markup.Escape(message));
    }

    /// <summary>
    /// 输出分割线
    /// </summary>
    public static void WriteDivider()
    {
        Console.WriteLine("-----------------------------");
    }

    /// <summary>
    /// 显示logo
    /// </summary>
    public static void ShowLogo()
    {
        //Console.SetWindowSize(150, Console.WindowHeight);

        var logo = new FigletText("build sidebar")
            .LeftJustified()
            .Color(Color.Yellow);
        AnsiConsole.Write(logo);

        var rule = new Rule("[red]build sidebar for c#[/]")
        {
            Justification = Justify.Center,
            Style = Style.Parse("red dim")
        };

        AnsiConsole.Write(rule);
    }
}