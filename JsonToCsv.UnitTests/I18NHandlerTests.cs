using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using i18n.Helper.Contracts;
using i18n.LocaleTool.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;

namespace i18n.Helper.UnitTests
{
    [TestClass]
    public class I18NHandlerTests
    {
        string json = @"{
            ""Dashboard"": ""Dashboard"",
	        ""Analyze"": ""Analyze"",
            ""actions"": {
                ""create"": ""Create"",
                ""save"": ""Save"",
                ""cancel"": ""Cancel"",
                ""edit"": ""Edit"",
                ""add"": ""Add"",
                ""delete"": ""Delete"",
                ""remove"": ""Remove"",
                ""movedown"": ""Move Down"",
                ""moveup"": ""Move Up"",
                ""addrow"": ""Add Row"",
                ""addrule"": ""Add Rule"",
                ""addline"": ""Add Line"",
                ""yes"": ""Yes"",
                ""no"": ""No"",
                ""ok"":""OK"",
                ""activate"": ""Activate"",
		        ""submitquery"": ""Submit Query"",
		        ""deactivate"": ""Deactivate"",
		        ""and"": ""And"",
		        ""or"": ""Or"",
		        ""addselectedroles"": ""Add Selected Roles"",
		        ""removeselectedroles"": ""Remove Selected Roles""
            },
            ""grid"": {
                ""nodata"": ""This list does not contain any items.""
            }}";

        [TestMethod]
        public void GenerateLongestStringsFile_Tests()
        {
            string inputPath = "<SomeFile>";
            string outputPath = "<OutputFile>";

            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            
            var fileHandler = fixture.Freeze<Mock<IFileHandler>>();
            var jsonParser = fixture.Freeze<Mock<Ii18nJsonParser>>();

            var directories = new List<I18NDirectoryFile>();
            var i18nDictionary = new Dictionary<string, object>();
            i18nDictionary.Add("Dashboard", "Dashboard");
            i18nDictionary.Add("actions.create", "Create");
            directories.Add(new I18NDirectoryFile()
            {
                Dictionary = i18nDictionary,
            });

            fileHandler.Setup(x => x.LoadJsonFileOrFolder(inputPath)).Returns(directories);
            fileHandler.Setup(x => x.ConstructFilePath(outputPath, It.IsAny<FileInfo>(),SaveType.Json, "dev_Longest" )).Returns(outputPath);
            fileHandler.Setup(x => x.SaveJson(It.IsAny<string>(), It.IsAny<string>()));

            jsonParser.Setup(y => y.GenerateJsonObject(It.IsAny<Dictionary<string, object>>(), "")).Returns(new object());

            var sut = fixture.Create<i18nHandler>();

            sut.GenerateLongestStringsFile(inputPath, outputPath);

            Assert.IsTrue(true);           
        }

        [TestMethod]
        public void GenerateShortStringsFile_Tests()
        {
            string inputPath = "<SomeFile>";
            string outputPath = "<OutputFile>";

            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var fileHandler = fixture.Freeze<Mock<IFileHandler>>();
            var jsonParser = fixture.Freeze<Mock<Ii18nJsonParser>>();

            var directories = new List<I18NDirectoryFile>();
            var i18nDictionary = new Dictionary<string, object>();
            i18nDictionary.Add("Dashboard", "Dashboard");
            i18nDictionary.Add("actions.create", "Create");
            directories.Add(new I18NDirectoryFile()
            {
                Dictionary = i18nDictionary,
            });

            fileHandler.Setup(x => x.LoadJsonFileOrFolder(inputPath)).Returns(directories);
            fileHandler.Setup(x => x.ConstructFilePath(outputPath, It.IsAny<FileInfo>(), SaveType.Json, "dev_Longest")).Returns(outputPath);
            fileHandler.Setup(x => x.SaveJson(It.IsAny<string>(), It.IsAny<string>()));

            jsonParser.Setup(y => y.GenerateJsonObject(It.IsAny<Dictionary<string, object>>(), "")).Returns(new object());

            var sut = fixture.Create<i18nHandler>();

            sut.GenerateShortestStringsFile(inputPath, outputPath);

            Assert.IsTrue(true);
        }

        [TestMethod]
        public void ConvertFlattenedFile_SingleFile()
        {
            string inputPath = "<SomeFile>";
            string outputPath = "<OutputFile>";

            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var fileHandler = fixture.Freeze<Mock<IFileHandler>>();
            var jsonParser = fixture.Freeze<Mock<Ii18nJsonParser>>();

            var directories = new List<I18NDirectoryFile>();
            var i18nDictionary = new Dictionary<string, object>();
            i18nDictionary.Add("Dashboard", "Dashboard");
            i18nDictionary.Add("actions.create", "Create");
            directories.Add(new I18NDirectoryFile()
            {
                Dictionary = i18nDictionary,
            });

            fileHandler.Setup(x => x.IsDirectory(inputPath)).Returns(false);
            fileHandler.Setup(x => x.SaveJson(It.IsAny<string>(), It.IsAny<string>()));

            jsonParser.Setup(y => y.GenerateJsonObject(It.IsAny<Dictionary<string, object>>(), "")).Returns(new object());

            var sut = fixture.Create<i18nHandler>();

            sut.ConvertFlattenedFile(inputPath, outputPath);

            Assert.IsTrue(true);
        }

        [TestMethod]
        public void ConvertFlattenedFile_Directory()
        {
            string inputPath = "<SomeFile>";
            string outputPath = "<OutputFile>";

            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var fileHandler = fixture.Freeze<Mock<IFileHandler>>();
            var jsonParser = fixture.Freeze<Mock<Ii18nJsonParser>>();

            var directories = new List<I18NDirectoryFile>();
            var i18nDictionary = new Dictionary<string, object>();
            i18nDictionary.Add("Dashboard", "Dashboard");
            i18nDictionary.Add("actions.create", "Create");
            directories.Add(new I18NDirectoryFile()
            {
                Dictionary = i18nDictionary,
            });

            fileHandler.Setup(x => x.IsDirectory(inputPath)).Returns(true);
            fileHandler.Setup(x => x.DirSearch(inputPath)).Returns(new List<string> { inputPath });
            fileHandler.Setup(x => x.SaveJson(It.IsAny<string>(), It.IsAny<string>()));

            jsonParser.Setup(y => y.GenerateJsonObject(It.IsAny<Dictionary<string, object>>(), "")).Returns(new object());

            var sut = fixture.Create<i18nHandler>();

            sut.ConvertFlattenedFile(inputPath, outputPath);

            Assert.IsTrue(true);
        }
    }
}
