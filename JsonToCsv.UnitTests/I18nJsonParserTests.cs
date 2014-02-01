using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

namespace i18n.Helper.UnitTests
{
    [TestClass]
    public class I18nJsonParserTests
    {
        [TestMethod]
        public void GenerateDictionaryTest()
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

            var parser = new I18NJsonParser(); //using System.Web.Script.Serialization;
            var reader = new JsonFx.Json.JsonReader();
            dynamic output = reader.Read(json);
            var dict = new Dictionary<string, object>();

            parser.GenerateDictionary((System.Dynamic.ExpandoObject)output, dict, "");
            Assert.IsNotNull(dict);
            Assert.AreEqual(dict["Dashboard"], "Dashboard");
        }

        [TestMethod]
        public void GenerateJsonObjectTest()
        {
            Dictionary<string, object> localizationValues = new Dictionary<string, object>();
            localizationValues.Add("Dashboard", "Dashboard");
            localizationValues.Add("Analyze", "Analyze");
            localizationValues.Add("actions.create", "Create");
            localizationValues.Add("actions.save", "Save");
            localizationValues.Add("actions.edit.default", "editor");
            localizationValues.Add("actions.edit.edit", "update");

            var parser = new I18NJsonParser(); //using System.Web.Script.Serialization;
            object jsonObject = parser.GenerateJsonObject(localizationValues, "");

            string json = JsonConvert.SerializeObject(jsonObject, new KeyValuePairConverter());

            Assert.IsNotNull(json);
        }
    }
}
