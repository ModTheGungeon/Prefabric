using System;
using System.IO;
using Newtonsoft.Json;

namespace Prefabric {
    public static class Prefabric {
        private static JsonSerializer _Serializer = new JsonSerializer();

        public static PfRoot Load(string path) {
            using (var stream = new StreamReader(File.OpenRead(path)))
            using (var text_reader = new JsonTextReader(stream)) {
                return _Serializer.Deserialize<PfRoot>(text_reader);
            }
        }

        internal static object MakeObject(GameObject parent, PfObject obj) {
        }
    }
}
