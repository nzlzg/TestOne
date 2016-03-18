using System;
using System.Collections.Generic;
using System.Xml;
using ServiceStack.Redis;
using ServiceStack.Redis.Support;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDB.Bson.Serialization.Attributes;

namespace test
{
    class Program
    {
        static void Main(string[] args)
        {
            // GetXmlInitialization() //遍历xml节点(递归)
            //  simpleRedisGetSet(); //redis插入，读取数据
            simpleMongoDBUse();//mongodb使用
            Console.Read();
        }

        #region simpleMongoDBUse()

        static void simpleMongoDBUse()
        {
            string _connectionString = "Server=127.0.0.1";
            string _dbName = "MyNorthwind";
            string collectionName = "collectionName";
            //  MongoServer server =MongoServer.Create(_connectionString);
            // MongoDatabase mongoDatabase = server.GetDatabase(_dbName);
            //MongoCollection<BsonDocument> collection = mongoDatabase.GetCollection<BsonDocument>(collectionName);
            //BsonDocument dom = new BsonDocument { { "CustomerName", "niezl" }, { "Address", "cnki" }, { "Tel", "12580" } };

            MongoClient mongoclient = new MongoClient(_connectionString);
            MongoServer server = mongoclient.GetServer();
            MongoDatabase mongoDatabase = server.GetDatabase(_dbName);
            MongoCollection<Customer> collection = mongoDatabase.GetCollection<Customer>(collectionName);
            List<Customer> customerlist = new List<Customer>() {
            new Customer() { CustomerName = "xiaojie", Address = "_cnki", Tel = "12306" },
            new Customer() {CustomerName="niezl",Address="cnki",Tel="12580" } };
            Customer customer = new Customer() { CustomerName = "niezl", Address = "cnki", Tel = "12580" };
            Dictionary<string, List<Customer>> dic = new Dictionary<string, List<Customer>>();
            dic.Add("Customer",customerlist);
            //BsonDocument dom = new BsonDocument(dic);
            //BsonDocument dom = customerlist.ToBsonDocument<List<Customer>>();
            IMongoUpdate newdom = Update.Set("Address", "new_cnki");
            IMongoQuery iq = new QueryDocument("CustomerName", "niezl");
            IMongoQuery iqand = Query.And(Query.GTE("Tel", "12580"), Query.Matches("CustomerName", "/^nie/"));
            try
            {
                // collection.Insert(dom);
                //consoleBsonDocument(collection.Find(iq));
                //collection.Update(iq,newdom);
                //consoleBsonDocument(collection.Find(iq));
                //collection.Remove(iq);
                //consoleBsonDocument(collection.Find(iq));

                // collection.Insert(customer);
                collection.Update(iq, newdom);
                var bsoncursor=collection.Find(iqand); 
                foreach (Customer bson in bsoncursor)
                {
                    Console.WriteLine("CustomerName:" + bson.CustomerName);
                    Console.WriteLine("Address:" + bson.Address);
                    Console.WriteLine("Tel:" + bson.Tel);
                }
            }
            catch (Exception e)
            {
                server.Disconnect();
            }
            finally {
                server.Disconnect();
            }
        }

        static void consoleBsonDocument(MongoCursor<BsonDocument> bsoncursor)
        {
            if (bsoncursor.Count() > 0)
            {
                foreach (BsonDocument bson in bsoncursor)
                {
                    Console.WriteLine("CustomerName:" + bson["CustomerName"].AsString);
                    Console.WriteLine("Address:" + bson["Address"].AsString);
                    Console.WriteLine("Tel:" + bson["Tel"].AsString);
                }
            }
            else
            {
                Console.WriteLine("未找到所需要数据");
            }
        }

        #endregion

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

    [BsonIgnoreExtraElements]
    sealed class Customer {
       // [BsonId]
        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string ContactName { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string Tel { get; set; }

    }
}
