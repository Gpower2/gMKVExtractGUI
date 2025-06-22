using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace gMKVToolNix.Unit.Tests
{
    [TestClass]
    public class FileExtensions_Tests
    {
        private void ActAndAssertGetOutputFilename(string filename, bool overwriteExisting, string expectedFilename)
        {
            string actualFilename = filename.GetOutputFilename(overwriteExisting);

            Assert.AreEqual(expectedFilename, actualFilename);
        }

        [TestMethod]
        public void GetOutputFilename_WithNoOverwrite_ShouldBe_Successful()
        {
            string filename = "test.txt";
            string expectedFilename = filename;

            ActAndAssertGetOutputFilename(filename, false, expectedFilename);

            File.Create(expectedFilename).Dispose();

            expectedFilename = "test.1.txt";
            ActAndAssertGetOutputFilename(filename, false, expectedFilename);

            File.Create(expectedFilename).Dispose();

            expectedFilename = "test.2.txt";
            ActAndAssertGetOutputFilename(filename, false, expectedFilename);

            File.Delete("test.txt");
            File.Delete("test.1.txt");
        }

        [TestMethod]
        public void GetOutputFilenameWithMultipleExtensions_WithNoOverwrite_ShouldBe_Successful()
        {
            string filename = "test.ogm.txt";
            string expectedFilename = filename;

            ActAndAssertGetOutputFilename(filename, false, expectedFilename);

            File.Create(expectedFilename).Dispose();

            expectedFilename = "test.ogm.1.txt";
            ActAndAssertGetOutputFilename(filename, false, expectedFilename);

            File.Create(expectedFilename).Dispose();

            expectedFilename = "test.ogm.2.txt";
            ActAndAssertGetOutputFilename(filename, false, expectedFilename);

            File.Delete("test.ogm.txt");
            File.Delete("test.ogm.1.txt");
        }

        [TestMethod]
        public void GetOutputFilename_WithOverwrite_ShouldBe_Successful()
        {
            string filename = "test.txt";
            string expectedFilename = filename;

            ActAndAssertGetOutputFilename(filename, true, expectedFilename);

            File.Create(expectedFilename).Dispose();

            ActAndAssertGetOutputFilename(filename, true, expectedFilename);
            ActAndAssertGetOutputFilename(filename, true, expectedFilename);
            ActAndAssertGetOutputFilename(filename, true, expectedFilename);

            File.Delete("test.txt");
        }
    }
}
