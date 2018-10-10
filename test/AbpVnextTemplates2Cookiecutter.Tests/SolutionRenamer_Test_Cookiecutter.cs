using Xunit;
using System.IO;
using System.Linq;

namespace AbpVnextTemplates2Cookiecutter.Tests
{
    public  class SolutionRenamer_Test_Cookiecutter : SolutionRenamer_Test_Base
    {

        public override void Dispose()
        {
            // DelTempDirectory(); // don't delete
        }

        [Fact]
        public void SolutionRenamer_Run_Cookiecutter()
        {
            // Act
            var solutionRename = new SolutionRenamer(TemporaryPath, "MyCompanyName", "MyProjectName", "{{cookiecutter.cn}}", "{{cookiecutter.pn}}");
            solutionRename.Run();

            // Assert check all directories.
            var directories = Directory
                .GetDirectories(TemporaryPath, "*.*", SearchOption.AllDirectories);

            var directory_name_contain_old_keyword = directories.Any(d => d.IndexOf("MyCompanyName") >= 0 || d.IndexOf("MyProjectName") >= 0);

            Assert.False(directory_name_contain_old_keyword);

            // Assert check name of all files.
            var files = Directory
               .GetFiles(TemporaryPath, "*.*", SearchOption.AllDirectories);

            var file_name_contain_old_keyword = files.Any(f =>
                Path.GetFileNameWithoutExtension(f).IndexOf("MyCompanyName") >= 0 ||
                Path.GetFileNameWithoutExtension(f).IndexOf("MyProjectName") >= 0);

            Assert.False(file_name_contain_old_keyword);

            // Assert check content of all files.
            var file_content_contain_old_keyword = false;
            foreach (var file in files)
            {
                var text = File.ReadAllText(file);
                file_content_contain_old_keyword = text.IndexOf("MyCompanyName") >= 0 || text.IndexOf("MyProjectName") >= 0;
                Assert.False(file_content_contain_old_keyword);
            }
        }
    }
}
