using Doc.ECM.APIHelper.DTO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Doc.ECM.APIHelper.Helpers
{

    public class ContentTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(ContentTypeDTO));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jObject = JObject.Load(reader);

            var result = JsonConvert.DeserializeObject<ContentTypeDTO>(jObject.ToString());
            result.DefFields = new List<DefFieldDTO>();

            foreach (var item in jObject["DefFields"].ToArray())
            {
                string type = item["Type"].Value<string>();
                switch (type)
                {
                    case "Numeric":
                        result.DefFields.Add(JsonConvert.DeserializeObject<DefFieldNumericDTO>(item.ToString()));
                        break;
                    case "DateTime":
                        result.DefFields.Add(JsonConvert.DeserializeObject<DefFieldDateTimeDTO>(item.ToString()));
                        break;
                    case "String":
                        result.DefFields.Add(JsonConvert.DeserializeObject<DefFieldStringDTO>(item.ToString()));
                        break;
                    case "List":
                        DefFieldListDTO defFieldList = JsonConvert.DeserializeObject<DefFieldListDTO>(item.ToString());
                        if (item["DynamicTable"] != null)
                        {
                            defFieldList.DynamicTable = JsonConvert.DeserializeObject<DefFieldListDynamicTableDTO>(item["DynamicTable"].ToString());
                            try
                            {
                                defFieldList.DynamicTableID = item["DynamicTableID"].Value<int>();
                            }
                            catch
                            {
                                defFieldList.DynamicTableID = -1;
                            }
                        }
                        result.DefFields.Add(defFieldList);
                        break;
                }
            }

            return result;
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}