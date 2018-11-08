using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Prefabric.JSON;

namespace Prefabric {
    [JsonObject]
    public class PfType {
        [JsonProperty("name")]
        public string Name;
        [JsonProperty("assembly")]
        public string Assembly;
        [JsonProperty("generic_params")]
        public string[] GenericParams;
    }

    [JsonObject]
    public class PfArrayType {
        [JsonProperty("name")]
        public string Name;
        [JsonProperty("assembly")]
        public string Assembly;
        [JsonProperty("generic_params")]
        public string[] GenericParams;
        [JsonProperty("dimensions")]
        public int Dimensions;
    }

    [JsonObject]
    public class PfObjectSkeleton {
        // Array
        [JsonProperty("array_type")]
        public PfArrayType ArrayType;
        [JsonProperty("entries")]
        [JsonConverter(typeof(ObjectArrayConverter))]
        public object[] ArrayEntries;

        // Generic
        [JsonProperty("type")]
        public PfType ObjectType;

        // Insert
        [JsonProperty("insert")]
        public string InsertID;

        // Shared (Insert/Generic)
        [JsonProperty("data")]
        [JsonConverter(typeof(DataConverter))]
        public Dictionary<string, object> Data;

        public object TryMakeObject() {
            var a = ArrayType != null;
            var b = ObjectType != null;
            var c = InsertID != null;

            if (a && c || b && c || a && c) {
                throw new InvalidOperationException("Inconsistent object type");
            }

            object obj = null;
            if (ArrayType != null) {
                obj = new PfArray(ArrayType, ArrayEntries);
            } else if (ObjectType != null) {
                obj = new PfObject(ObjectType, Data);
            } else if (InsertID != null) {
                obj = new PfInsert(InsertID, Data);
            }

            if (obj == null) {
                throw new InvalidOperationException("Unknown object type");
            }

            return obj;
        }
    }

    [JsonObject]
    public class PfObject {
        [JsonProperty("type")]
        public PfType Type;
        [JsonProperty("data")]
        public Dictionary<string, object> Data;

        public PfObject(PfType type, Dictionary<string, object> data) {
            Type = type;
            Data = data;
        }
    }

    [JsonObject]
    public class PfArray {
        [JsonProperty("array_type")]
        public PfArrayType ArrayType;
        [JsonProperty("entries")]
        [JsonConverter(typeof(ObjectArrayConverter))]
        public object[] Entries;

        public PfArray(PfArrayType array_type, object[] entries) {
            ArrayType = array_type;
            Entries = entries;
        }
    }

    [JsonObject]
    public class PfComponent {
        [JsonProperty("id")]
        public string ID;
        [JsonProperty("type")]
        public PfType Type;
        [JsonProperty("data")]
        [JsonConverter(typeof(DataConverter))]
        public Dictionary<string, object> Data;
    }

    [JsonObject]
    public class PfGameObject {
        [JsonProperty("version")]
        public int Version = 1;
        [JsonProperty("transform")]
        public PfObject Transform; // must be a Transform
        [JsonProperty("components")]
        public PfComponent[] Components;
    }

    [JsonObject]
    public class PfInsert {
        [JsonProperty("insert")]
        public string InsertID;
        [JsonProperty("data")]
        [JsonConverter(typeof(DataConverter))]
        public Dictionary<string, object> Data;

        public PfInsert(string insert_id, Dictionary<string, object> data) {
            InsertID = insert_id;
            Data = data;
        }
    }
}
