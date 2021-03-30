using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Web
{
    [Serializable]
    public class DataStore
    {
        private static string DataStoreFile
        {
            get
            {
                var path = System.AppDomain.CurrentDomain.BaseDirectory + @"\DataStore.xml";

                return path;
            }
        }

        public List<DataItem> Items { get; set; }

        public DataStore()
        {
            Items = new List<DataItem>();
        }

        public string ToXml()
        {
            try
            {
                var xmlSerializer = new XmlSerializer(this.GetType());

                using (var textWriter = new StringWriter())
                {
                    xmlSerializer.Serialize(textWriter, this);

                    return textWriter.ToString();
                }
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public static DataStore FromXml(string xml)
        {
            try
            {
                var xmlSerializer = new XmlSerializer(typeof(DataStore));

                var textReader = new StringReader(xml);

                return (DataStore)xmlSerializer.Deserialize(textReader);
            }
            catch
            {
                return new DataStore();
            }
        }

        public static void WriteToDisk(DataStore data)
        {
            File.WriteAllText(DataStoreFile, data.ToXml());
        }

        public static DataStore ReadFromDisk()
        {
            return !File.Exists(DataStoreFile)
                ? new DataStore()
                : DataStore.FromXml(File.ReadAllText(DataStoreFile));
        }
    }

    [Serializable]
    public class DataItem
    {
        public DataItem()
        {
            Headers = new List<KeyValue>();

            Body = string.Empty;

            TimeStamp = DateTime.Now;
        }

        public int Id { get; set; }

        public List<KeyValue> Headers { get; set; }

        public string Body { get; set; }

        public DateTime TimeStamp { get; set; }
    }

    [Serializable]
    public class KeyValue
    {
        public KeyValue()
        {
        }

        public KeyValue(string key, string value)
        {
            Key = key;

            Value = value;
        }

        public string Key { get; set; }

        public string Value { get; set; }
    }
}
