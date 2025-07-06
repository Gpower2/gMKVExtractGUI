using System.Collections.Generic;
using gMKVToolNix;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace gMKVToolnix.Unit.Tests
{
    [TestClass]
    public sealed class gMkvParser_Tests
    {
        private void ActAndAssertVersionParsed(List<string> output, int major, int minor, int priv)
        {
            gMKVVersion gMKVVersion = gMKVVersionParser.ParseVersionOutput(output);

            Assert.AreEqual(major, gMKVVersion.FileMajorPart);
            Assert.AreEqual(minor, gMKVVersion.FileMinorPart);
            Assert.AreEqual(priv, gMKVVersion.FilePrivatePart);
        }

        [TestMethod]
        public void VersionOutput_ShouldBe_Parsed_Successfully()
        {
            ActAndAssertVersionParsed(new List<string>() { "mkvmerge v64.0.0 ('The Last Goodbye') 64-bit" }, 64, 0, 0);
            ActAndAssertVersionParsed(new List<string>() { "mkvmerge v12.5.7 ('Some Name') 32-bit" }, 12, 5, 7);
            ActAndAssertVersionParsed(new List<string>() { "mkvmerge v1.2.3 ('Test') 64-bit" }, 1, 2, 3);
            ActAndAssertVersionParsed(new List<string>() { "mkvmerge v86.0 ('Winter') 64-bit" }, 86, 0, 0);

            ActAndAssertVersionParsed(new List<string>() { "mkvinfo v64.0.0 ('The Last Goodbye') 64-bit" }, 64, 0, 0);
            ActAndAssertVersionParsed(new List<string>() { "mkvinfo v12.5.7 ('Some Name') 32-bit" }, 12, 5, 7);
            ActAndAssertVersionParsed(new List<string>() { "mkvinfo v1.2.3 ('Test') 64-bit" }, 1, 2, 3);
            ActAndAssertVersionParsed(new List<string>() { "mkvinfo v86.0 ('Winter') 64-bit" }, 86, 0, 0);

            ActAndAssertVersionParsed(new List<string>() { "mkvextract v64.0.0 ('The Last Goodbye') 64-bit" }, 64, 0, 0);
            ActAndAssertVersionParsed(new List<string>() { "mkvextract v12.5.7 ('Some Name') 32-bit" }, 12, 5, 7);
            ActAndAssertVersionParsed(new List<string>() { "mkvextract v1.2.3 ('Test') 64-bit" }, 1, 2, 3);
            ActAndAssertVersionParsed(new List<string>() { "mkvextract v86.0 ('Winter') 64-bit" }, 86, 0, 0);
        }

        [TestMethod]
        public void VersionNullOrEmptyOutput_ShouldBe_NotParsed_ReturnDefaultVersion()
        {
            ActAndAssertVersionParsed(new List<string>() { null }, 0, 0, 0);
            ActAndAssertVersionParsed(new List<string>() { "" }, 0, 0, 0);
            ActAndAssertVersionParsed(new List<string>() { " " }, 0, 0, 0);
        }
    }
}