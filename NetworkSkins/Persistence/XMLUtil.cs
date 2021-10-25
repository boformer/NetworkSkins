namespace NetworkSkins.Persistence {
    using System;
    using System.IO;
    using System.Xml.Serialization;
    using UnityEngine;

    public class XMLUtil {
        static XmlSerializer Serilizer<T>() => new XmlSerializer(typeof(T));
        static XmlSerializerNamespaces NoNamespaces {
            get {
                var ret = new XmlSerializerNamespaces();
                ret.Add("", "");
                return ret;
            }
        }
        static void Serialize<T>(TextWriter writer, T value) => Serilizer<T>().Serialize(writer, value, NoNamespaces);
        static T Deserialize<T>(TextReader reader) => (T)Serilizer<T>().Deserialize(reader);
        public static string Serialize<T>(T value) {
            try {
                using(TextWriter writer = new StringWriter()) {
                    Serialize(writer, value);
                    return writer.ToString();
                }
            } catch(Exception ex) {
                Debug.LogException(ex);
                return null;
            }
        }

        public static T Deserialize<T>(string data) {
            try {
                using(TextReader reader = new StringReader(data)) {
                    return Deserialize<T>(reader);
                }
            } catch(Exception ex) {
                Debug.LogException(new Exception("data=" +data, ex));
                return default;
            }
        }

    }
}
