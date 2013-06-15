using System.IO;
using System.Xml.Serialization;

namespace Iridescent.Utils.Common
{
    public static class XmlSerializerUtils
    {
        public static T Deserialize<T>(string filePath) where T:class 
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (Stream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                T result = serializer.Deserialize(stream) as T;
                return result;
            }
        }

        public static void Serializer(string filePath,object obj)
        {
            XmlSerializer serializer = new XmlSerializer(obj.GetType());
            using (FileStream file = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                serializer.Serialize(file, obj);
            }
        }
    }
}
