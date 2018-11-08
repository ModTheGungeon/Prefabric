using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Prefabric.JSON {
    internal class JSONHelper {
        public static object DeserializeObject(JToken token) {
            if (token is JObject) {
                var skele = ((JObject)token).ToObject<PfObjectSkeleton>();
                return skele.TryMakeObject();
            }
            throw new NotSupportedException($"Invalid token type: {token.GetType()}");
        }
    }

    public class ObjectArrayConverter : JsonConverter {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            writer.WriteRawValue(value.ToString());
        }

        public object Deserialize(JToken token) {
            if (token is JValue) return ((JValue)token).Value;
            if (token is JObject) {
                return JSONHelper.DeserializeObject(token);
            }
            throw new NotSupportedException($"Invalid token type: {token.GetType()}");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            JToken token = JToken.Load(reader);
            if (!(token is JArray)) throw new Newtonsoft.Json.JsonSerializationException("Must be an array");
            List<object> objs = new List<object>();
            foreach (var child_tok in ((JArray)token).Children()) {
                objs.Add(Deserialize(child_tok));
            }
            return objs.ToArray();
        }

        public override bool CanConvert(Type objectType) {
            bool can = false;
            can = can || objectType.IsAssignableFrom(typeof(object[]));
            return can;
        }
    }

    public class DataConverter : JsonConverter {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            writer.WriteRawValue(value.ToString());
        }

        public object Deserialize(JToken token) {
            if (token is JValue) return ((JValue)token).Value;
            if (token is JObject) {
                return JSONHelper.DeserializeObject(token);
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
