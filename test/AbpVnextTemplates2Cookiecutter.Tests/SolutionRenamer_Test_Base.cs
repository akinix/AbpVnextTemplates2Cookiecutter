using System;
using System.IO;
using System.IO.Compression;

namespace AbpVnextTemplates2Cookiecutter.Tests
{
    public class SolutionRenamer_Test_Base : IDisposable
    {
        private const string TemporaryName = "temp";

        private readonly string TestTemplateZipName = Path.Combine("..", "..", "..", "templates", "module.zip");

        public string TemplatepZipPath { get; }

        public string TemporaryPath { get; }

        public SolutionRenamer_Test_Base()
        {
            TemplatepZipPath = Path.Combine(Environment.CurrentDirectory, TestTemplateZipName);
            TemporaryPath = Path.Combine(Environment.CurrentDirectory, TemporaryName);
            PreInitialize();
        }

        public virtual void Dispose()
        {
            
        }

        private void PreInitialize()
        {
            DelTempDirectory();
            Unzip(TemplatepZipPath, TemporaryPath);
        }

        public void DelTempDirectory()
        {
            if (!Directory.Exists(TemporaryPath))
                return;
            Directory.Delete(TemporaryPath, true);
        }

        private void Unzip(string zipPath, string targetPath)
        {
            ZipFile.ExtractToDirectory(zipPath, targetPath);
        }
    }
}
