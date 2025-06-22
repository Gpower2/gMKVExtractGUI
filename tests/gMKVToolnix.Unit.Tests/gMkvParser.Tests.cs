using gMKVToolNix;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace gMKVToolnix.Unit.Tests
{
    [TestClass]
    public sealed class gMkvParser_Tests
    {
        private void ActAndAssertVersionParsed(string output, int major, int minor, int priv)
        {
            gMKVVersion gMKVVersion = gMKVVersionParser.ParseVersionOutput(output);

            Assert.AreEqual(major, gMKVVersion.FileMajorPart);
            Assert.AreEqual(minor, gMKVVersion.FileMinorPart);
            Assert.AreEqual(priv, gMKVVersion.FilePrivatePart);
        }

        [TestMethod]
        public void VersionOutput_ShouldBe_Parsed_Successfully()
        {
            ActAndAssertVersionParsed("mkvmerge v64.0.0 ('The Last Goodbye') 64-bit", 64, 0, 0);
            ActAndAssertVersionParsed("mkvmerge v12.5.7 ('Some Name') 32-bit", 12, 5, 7);
            ActAndAssertVersionParsed("mkvmerge v1.2.3 ('Test') 64-bit", 1, 2, 3);
            ActAndAssertVersionParsed("mkvmerge v86.0 ('Winter') 64-bit", 86, 0, 0);

            ActAndAssertVersionParsed("mkvinfo v64.0.0 ('The Last Goodbye') 64-bit", 64, 0, 0);
            ActAndAssertVersionParsed("mkvinfo v12.5.7 ('Some Name') 32-bit", 12, 5, 7);
            ActAndAssertVersionParsed("mkvinfo v1.2.3 ('Test') 64-bit", 1, 2, 3);
            ActAndAssertVersionParsed("mkvinfo v86.0 ('Winter') 64-bit", 86, 0, 0);

            ActAndAssertVersionParsed("mkvextract v64.0.0 ('The Last Goodbye') 64-bit", 64, 0, 0);
            ActAndAssertVersionParsed("mkvextract v12.5.7 ('Some Name') 32-bit", 12, 5, 7);
            ActAndAssertVersionParsed("mkvextract v1.2.3 ('Test') 64-bit", 1, 2, 3);
            ActAndAssertVersionParsed("mkvextract v86.0 ('Winter') 64-bit", 86, 0, 0);
        }

        [TestMethod]
        public void VersionNullOrEmptyOutput_ShouldBe_NotParsed_ReturnDefaultVersion()
        {
            ActAndAssertVersionParsed(null, 0, 0, 0);
            ActAndAssertVersionParsed("", 0, 0, 0);
            ActAndAssertVersionParsed(" ", 0, 0, 0);
        }
    }
}