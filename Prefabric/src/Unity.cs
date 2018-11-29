using System;
using System.Collections.Generic;
using UnityEngine;

namespace Prefabric {
    public static partial class Prefabric {
        public static readonly Type TransformType;

        static Prefabric() {
            TransformType = typeof(Transform);
        }

        public static object ConvertType(object obj, Type intended_type, Type context_type = null) {
            Console.WriteLine($"Intended type: {intended_type}");
            if (intended_type == typeof(Int16) || intended_type == typeof(short)) return Convert.ToInt16(obj);
            if (intended_type == typeof(Int32) || intended_type == typeof(int)) return Convert.ToInt32(obj);
            if (intended_type == typeof(Int64) || intended_type == typeof(long)) return Convert.ToInt64(obj);
            if (obj is PfObject) return InstantiateObject((PfObject)obj, context_type);
            if (obj is PfArray) return InstantiateArray((PfArray)obj, context_type);
            //TODO implement PfInsert
            return obj;
        }

        public static void FillInFields(object obj, Dictionary<string, object> data, Type type) {
            if (obj.GetType() != type) throw new Exception($"Type mismatch: '{obj.GetType()}' vs '{type}'");
            foreach (var dataent in data) {
                var field = obj.GetType().GetField(dataent.Key);
                if (field == null) {
                    var prop = obj.GetType().GetProperty(dataent.Key);
                    if (prop == null) {
                        throw new Exception($"Type '{type.Name}' does not have a '{dataent.Key}' field or property");
                    }
                    var set_method = prop.GetSetMethod();
                    if (set_method == null) {
                        throw new Exception($"Type '{type.Name}' does not have a '{dataent.Key}' field, and the property with the same name does not have a setter");
                    }
                    set_method.Invoke(obj, new object[] { ConvertType(dataent.Value, prop.PropertyType) });
                } else {
                    field.SetValue(obj, ConvertType(dataent.Value, field.FieldType));
                }
            }
        }

        public static Component InstantiateComponent(GameObject go, PfComponent pfcomp, Type context_type = null) {
            var type = pfcomp.Type.Resolve(context_type);
            var comp = go.AddComponent(type);
            FillInFields(comp, pfcomp.Data, type);
            return comp;
        }

        public static object InstantiateObject(PfObject pfobj, Type context_type = null) {
            var type = pfobj.Type.Resolve(context_type);
            var obj = Activator.CreateInstance(type);
            FillInFields(obj, pfobj.Data, type);
            return obj;
        }

        public static object InstantiateArray(PfArray pfary, Type context_type = null) {
            var el_type = pfary.ArrayType.Resolve(context_type);
            var ary = Array.CreateInstance(el_type, pfary.Entries.Length);
            for (int i = 0; i < pfary.Entries.Length; i++) {
                var ent = pfary.Entries[i];
                ary.SetValue(ConvertType(ent, el_type, context_type: el_type), i);
            }
            return ary;
        }

        public static void VerifyTransform(PfObject obj) {
            var type = obj.Type.Resolve();
            if (type != TransformType) {
                throw new Exception("The transform field must be a UnityEngine.Transform object");
            }
        }

        public static GameObject InstantiateGameObject(PfGameObject pfgo) {
            var go = new GameObject(pfgo.Name);
            FillInFields(go.transform, pfgo.Transform.Data, TransformType);

            for (int i = 0; i < pfgo.Components.Length; i++) {
                var comp = pfgo.Components[i];

                InstantiateComponent(go, comp);
            }
            return go;
        }
    }
}
