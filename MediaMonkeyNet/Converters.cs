using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediaMonkeyNet
{

    public class ExtendedTagsConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);
            if (token.Type == JTokenType.String)
            {
                return JsonConvert.DeserializeObject<List<ExtendedTag>>(token.ToObject<string>());
            }
            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

    public class SerialDateConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return false;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);

            if (token.Type == JTokenType.String)
            {
                double dateVal;
                if (double.TryParse(token.ToObject<string>(), out dateVal))
                {
                    return ConvertSerialToDateTime(dateVal);
                }
            }
            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }


        private static DateTime ConvertSerialToDateTime(double serialDate)
        {
            // Workaround because DateTime.FromOADate is not available in .net standard 1.3 
            if (serialDate < 1)
            {
                return DateTime.MinValue;
            }

            DateTime dateOfReference = new DateTime(1900, 1, 1);
            if (serialDate > 60d)
            {
                serialDate = serialDate - 2;
            }
            else
            {
                serialDate = serialDate - 1;
            }
            return dateOfReference.AddDays(serialDate);
        }
    }

}
