using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Prefabric.JSON {
    public class DataConverter : JsonConverter {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            writer.WriteRawValue(value.ToString());
        }

        public object Deserialize(JToken token) {
            if (token is JValue) return ((JValue)token).Value;
            if (token is JObject) {
                return ((JObject)token).ToObject<PfObject>();
            }
            throw new NotSupportedException($"Invalid token type: {token.GetType()}");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            JToken token = JToken.Load(reader);
            if (!(token is JObject)) throw new Newtonsoft.Json.JsonSerializationException("Must be an object");
            Dictionary<string, object> dict = new Dictionary<string, object>();
            foreach (var child_tok in ((JObject)token).Children()) {
                if (child_tok is JProperty) {
                    var prop = (JProperty)child_tok;
                    dict[prop.Name] = Deserialize(prop.Value);
                }
            }
            return dict;
        }

        public override bool CanConvert(Type objectType) {
            bool can = false;
            can = can || objectType.IsAssignableFrom(typeof(Dictionary<string, object>));
            return can;
        }
    }
}
