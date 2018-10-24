using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Prefabric.JSON;

namespace Prefabric {
    //[JsonObject]
    //public class PfVector2 {
    //    [JsonProperty("x")]
    //    public float X;
    //    [JsonProperty("y")]
    //    public float Y;
    //}

    //[JsonObject]
    //public class PfVector3 {
    //    [JsonProperty("x")]
    //    public float X;
    //    [JsonProperty("y")]
    //    public float Y;
    //    [JsonProperty("z")]
    //    public float Z;
    //}

    //[JsonObject]
    //public class PfVector4 {
    //    [JsonProperty("x")]
    //    public float X;
    //    [JsonProperty("y")]
    //    public float Y;
    //    [JsonProperty("z")]
    //    public float Z;
    //    [JsonProperty("w")]
    //    public float W;
    //}

    [JsonObject]
    public class PfTransform {
        [JsonProperty("position")]
        public PfObject Position;
        [JsonProperty("rotation")]
        public PfObject Rotation;
        [JsonProperty("scale")]
        public PfObject Scale;
    }

    public enum PfObjectType {
        None,
        Object,
        Array,
        Dictionary,
        List
    }

    [JsonObject]
    public class PfObject {
        [JsonProperty("type")]
        public string Type;
        [JsonProperty("generic_params")]
        public string[] GenericParams;
        [JsonProperty("data")]
        [JsonConverter(typeof(DataConverter))]
        public Dictionary<string, object> Data;
    }

    [JsonObject]
    public class PfRoot {
        [JsonProperty("version")]
        public int Version = 1;
        [JsonProperty("transform")]
        public PfTransform Transform;
        [JsonProperty("children")]
        public PfObject[] Children;
    }

    // end of json objects
}
