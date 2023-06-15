using DocsifyBuildSidebar;

using Xunit.Abstractions;

namespace DocsifyBuildSidebarTests
{
    public class UtilsTests
    {

        public ITestOutputHelper Log { get; set; }
        public UtilsTests(ITestOutputHelper log)
        {
            Log = log;
        }

        [Fact]
        public void Test_GenerateSpace()
        {

            Assert.Equal("  ", Utils.GenerateSpace(1));
            Assert.Equal("    ", Utils.GenerateSpace(2));
            Assert.Equal("      ", Utils.GenerateSpace(3));
        }

        [Fact]
        public void Test_ReplaceSpace()
        {
            var res = Utils.ReplaceSpace("测试数据 嘿嘿嘿 按按 问问");
            Assert.Equal("测试数据%20嘿嘿嘿%20按按%20问问", res);
        }

    }
}