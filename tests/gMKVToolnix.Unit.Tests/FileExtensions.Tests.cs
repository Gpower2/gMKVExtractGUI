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
            string filename = "test.tc.txt";
            string expectedFilename = filename;

            ActAndAssertGetOutputFilename(filename, false, expectedFilename);

            File.Create(expectedFilename).Dispose();

            expectedFilename = "test.tc.1.txt";
            ActAndAssertGetOutputFilename(filename, false, expectedFilename);

            File.Create(expectedFilename).Dispose();

            expectedFilename = "test.tc.2.txt";
            ActAndAssertGetOutputFilename(filename, false, expectedFilename);

            File.Delete("test.tc.txt");
            File.Delete("test.tc.1.txt");
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
