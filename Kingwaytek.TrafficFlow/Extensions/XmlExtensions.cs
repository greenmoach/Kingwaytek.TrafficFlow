namespace Kingwaytek.TrafficFlow
{
    public static class XmlExtensions
    {
        public static string ToXml(this object target)
        {
            return XmlSerializerHelper.SerializeByXml(target);
        }

        public static T ToTypedObjectByXml<T>(this string s)
        {
            if (s.IsNullOrEmpty())
            {
                return default(T);
            }

            return XmlSerializerHelper.DeserializeByXml<T>(s);
        }

        public static bool CanDeserializeByXml<T>(this string s)
        {
            return XmlSerializerHelper.CanDeserializeByXml<T>(s);
        }
    }
}