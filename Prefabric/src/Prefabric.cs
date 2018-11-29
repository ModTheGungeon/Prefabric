using System;
using System.IO;
using Newtonsoft.Json;

namespace Prefabric {
    public static partial class Prefabric {
        private static JsonSerializer _Serializer = new JsonSerializer();

        public static PfGameObject Load(string path) {
            using (var stream = new StreamReader(File.OpenRead(path)))
            using (var text_reader = new JsonTextReader(stream)) {
                return _Serializer.Deserialize<PfGameObject>(text_reader);
            }
        }
    }
}
