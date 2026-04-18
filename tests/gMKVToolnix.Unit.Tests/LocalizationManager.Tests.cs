using gMKVToolNix.Localization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace gMKVToolNix.Unit.Tests
{
    [TestClass]
    public class LocalizationManager_Tests
    {
        [TestInitialize]
        public void InitializeLocalization()
        {
            LocalizationManager.Reload("en");
        }

        [TestMethod]
        public void GetString_WithSingleStringFormatArgument_ShouldFormatCurrentCultureString()
        {
            string actual = LocalizationManager.GetString("UI.MainForm2.OutputDirectory.UseDefaultWithValue", "Not set");

            Assert.AreEqual("Use Currently Set Default Directory: (Not set)", actual);
        }

        [TestMethod]
        public void GetString_WithStringAndNumericFormatArguments_ShouldNotTreatFirstArgumentAsCulture()
        {
            string actual = LocalizationManager.GetString("UI.MainForm2.ContextMenu.CheckTrackGroup", "Video Tracks", 1, 2);

            Assert.AreEqual("Check Video Tracks... (1/2)", actual);
        }

        [TestMethod]
        public void Reload_WithUnavailableCulture_ShouldNormalizeToEnglish()
        {
            LocalizationManager.Reload("missing-culture");

            Assert.AreEqual("en", LocalizationManager.CurrentCulture);
            Assert.AreEqual("Are you sure?", LocalizationManager.GetString("UI.Common.Dialog.AreYouSureTitle"));
        }
    }
}
