using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace gMKVToolNix.Unit.Tests
{
    [TestClass]
    public class DataReceivedEventArgs_Tests
    {
        private void ActAndAssertDataReceivedEventArgs(string data)
        {
            DataReceivedEventArgs dataReceivedEventArgs = ProcessExtensions.GetDataReceivedEventArgs(data);

            Assert.IsNotNull(dataReceivedEventArgs);
            Assert.AreEqual(dataReceivedEventArgs.Data, data);                       
        }

        [TestMethod]
        public void GetDataReceivedEventArgs_ShouldBe_Successful()
        {
            ActAndAssertDataReceivedEventArgs("mkvmerge v64.0.0 ('The Last Goodbye') 64-bit");
            ActAndAssertDataReceivedEventArgs("");
            ActAndAssertDataReceivedEventArgs(null);
        }
    }
}