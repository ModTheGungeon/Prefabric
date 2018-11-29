using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Prefabric.JSON;
using System.Reflection;

namespace Prefabric {
    [JsonObject]
    public class PfType {
        [JsonProperty("name")]
        public string Name;
        [JsonProperty("assembly")]
        public string Assembly;
        [JsonProperty("generic_params")]
        public PfType[] GenericParams;

        // Array
        [JsonProperty("dimensions")]
        public int? Dimensions = null;

        public Type Resolve(Type context_type = null) {
            Assembly asm;
            if (context_type != null && Assembly == ".") asm = context_type.Assembly;
            else if (Assembly == ".") throw new Exception($"Tried to use context assembly shorthand, but there is no context assembly to use");
            else asm = System.Reflection.Assembly.Load(Assembly);
            if (asm == null) throw new Exception($"Assembly '{Assembly}' failed to load.");

            Type type;
            if (context_type != null && Name == ".") type = context_type;
            else if (Name == ".") throw new Exception($"Tried to use context type shorthand, but there is no context type to use");
            else type = asm.GetType(Name);
            if (type == null) throw new Exception($"Type '{Name}' from assembly '{Assembly}' doesn't exist.");

            if (GenericParams != null) {
                var generic_params = new List<Type>();
                for (int i = 0; i < GenericParams.Length; i++) {
                    generic_params.Add(GenericParams[i].Resolve());
                }
                if (!type.ContainsGenericParameters) throw new Exception($"Tried to create generic type from non-generic type (assembly '{Assembly}', type '{Name}', # of generic params {GenericParams.Length}).");
                type = type.MakeGenericType(generic_params.ToArray());
            }
            //if (Dimensions != null) {
            //    if (!type.IsArray) throw new Exception($"Tried to create array type from non-array type (assembly '{Assembly}', type '{Name}', dimensions {Dimensions}).");
            //    type = type.MakeArrayType(Dimensions.Value);
            //}
            return type;
        }
    }

    [JsonObject]
    public class PfObjectSkeleton {
        // Array
        [JsonProperty("array_type")]
        public PfType ArrayType;
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
        public PfType ArrayType;
        [JsonProperty("entries")]
        [JsonConverter(typeof(ObjectArrayConverter))]
        public object[] Entries;

        public PfArray(PfType array_type, object[] entries) {
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
        [JsonProperty("name")]
        public string Name;
        [JsonProperty("transform")]
        public PfObject Transform; // must be a Transform
        [JsonProperty("components")]
        public PfComponent[] Components;

        public UnityEngine.GameObject Instantiate() {
            return Prefabric.InstantiateGameObject(this);
        }
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
