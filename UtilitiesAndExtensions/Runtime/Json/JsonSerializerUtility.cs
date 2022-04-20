using Newtonsoft.Json;
using System.IO;

namespace Paulsams.MicsUtils
{
    public static class JsonSerializerUtility
    {
        public static void Serialize<T>(T serializedObject, string filePath, Formatting formatting = Formatting.Indented)
        {
            using (StreamWriter file = File.CreateText(filePath))
            {
                JsonSerializer serializer = new JsonSerializer()
                {
                    Formatting = formatting,
                };

                serializer.Serialize(file, serializedObject);
            }
        }

        public static bool TryDeserialize<T>(out T needObject, string filePath)
        {
            needObject = default(T);

            if (File.Exists(filePath) == false)
                return false;

            using (StreamReader file = File.OpenText(filePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                needObject = (T)serializer.Deserialize(file, typeof(T));
                return true;
            }
        }
    }
}