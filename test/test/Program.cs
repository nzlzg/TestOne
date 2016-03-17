using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using ServiceStack.Redis;
using ServiceStack.Redis.Support;

namespace test
{
    class Program
    {
        static void Main(string[] args)
        {
            // GetXmlInitialization() //遍历xml节点(递归)
            simpleRedisGetSet();
            Console.Read();
        }
        #region GetXmlInitialization()

        static void GetXmlInitialization() {
            string xmlstring = "<?xml version='1.0' encoding='utf - 8'?> <FileSystem><DriverC><Dir Dirname = 'MSQCOS22'><File FileName = 'Commond.tt'>"
              + "</File><Dir DirName = 'MSQD58'><File FileName = 'Request.oci'></File></Dir></Dir><File FileName = '_desktop.ini'></File>"
     + " <File FileName = 'io.sys'></File></DriverC></FileSystem> ";
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlstring);
            GetXmlPropertyValue(xmlDoc.ChildNodes);
        }

      static  void GetXmlPropertyValue(XmlNodeList nodeList)
        {
            foreach (XmlNode node in nodeList)
            {
                if (node.Name == "File")
                {
                    Console.WriteLine(node.Attributes["FileName"].Value);
                }
                else if(node.ChildNodes.Count>0){
                    GetXmlPropertyValue(node.ChildNodes);
                }
            }
        }

        #endregion

        #region simpleRedisGetSet()

        static void simpleRedisGetSet() {
            string host = "localhost";
            string elementKey = "testKeyRedis";
            RedisClient redisclient = new RedisClient(host);
            redisclient.Remove(elementKey);
            var storeMembers = new List<string> { "asd","dsd","bb"};
            // storeMembers.ForEach(x=>redisclient.AddItemToList(elementKey,x));

            redisclient.AddRangeToList(elementKey,storeMembers);
            var members = redisclient.GetAllItemsFromList(elementKey);
            var list = redisclient.Lists[elementKey];
            list.Clear();
            var ser = new ObjectSerializer();
            bool result = redisclient.Set<byte[]>(elementKey, ser.Serialize(storeMembers));
            if (result)
            {
                var resultlist = ser.Deserialize(redisclient.Get<byte[]>(elementKey));
            }
            members.ForEach(s=>Console.WriteLine("list:"+s));
         //   Console.WriteLine("index2:"+redisclient.GetItemFromList(elementKey,1));
            //if (redisclient.Get<string>(elementKey) == null)
            //{
            //    redisclient.Set(elementKey, "defaultValue");
            //}
            //else {
            //    redisclient.Set(elementKey, "new default value");
            //}
            //Console.WriteLine("redise:value:"+redisclient.Get<string>(elementKey));
            Console.WriteLine(redisclient.Info);
        }

        #endregion
    }
}
