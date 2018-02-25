using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Kingwaytek.TrafficFlow
{
    public static class XmlSerializerHelper
    {
        //==================================================================================================Serialize
        /// <summary>將傳入物件序列化，以XmlSerializer</summary>
        public static String SerializeByXml(Object target)
        {
            var xSer = new XmlSerializer(target.GetType());
            using (var memoryStream = new MemoryStream())
            {
                var xmlWriter = new XmlTextWriter(memoryStream, new UTF8Encoding(false))
                {
                    Formatting = Formatting.Indented,
                    Indentation = 2,
                };

                XmlSerializerNamespaces xsn = new XmlSerializerNamespaces();
                //empty namespaces
                xsn.Add(String.Empty, String.Empty);

                xSer.Serialize(xmlWriter, target, xsn);
                var buffer = memoryStream.ToArray();
                return Encoding.UTF8.GetString(buffer);
            }
        }

        //==================================================================================================Deserialize
        /// <summary>將Xml內容反序列化為物件</summary>
        public static TType DeserializeByXml<TType>(String getXMLString)
        {
            return (TType)DeserializeByXml(typeof(TType), getXMLString);
        }

        /// <summary>將Xml內容反序列化為物件</summary>
        public static Object DeserializeByXml(Type getType, String getXMLString)
        {
            var xSer = new XmlSerializer(getType);
            using (var sr = new StringReader(getXMLString))
            {
                var targetObject = xSer.Deserialize(sr);
                if (targetObject == null) throw new Exception("反序列化失敗");
                return targetObject;
            }
        }

        /// <summary>將Xml Stream讀取進來，反序列化為物件</summary>
        public static TType DeserializeByXml<TType>(Stream stream)
        {
            var xSer = new XmlSerializer(typeof(TType));
            var targetObject = xSer.Deserialize(stream);
            if (targetObject == null) throw new Exception("反序列化失敗");
            return (TType)targetObject;
        }

        /// <summary>
        /// 判斷此xml能否序列化該物件
        /// </summary>
        /// <param name="getXMLString">xml內容</param>
        /// <returns>True/False</returns>
        public static bool CanDeserializeByXml<T>(String getXMLString)
        {
            var xSer = new XmlSerializer(typeof(T));
            bool result;

            using (XmlReader reader = XmlReader.Create(new StringReader(getXMLString)))
            { result = xSer.CanDeserialize(reader); }

            return result;
        }
    }
}