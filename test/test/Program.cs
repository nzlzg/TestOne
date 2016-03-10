using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace test
{
    class Program
    {
        static void Main(string[] args)
        {
            string xmlstring = "<?xml version='1.0' encoding='utf - 8'?> <FileSystem><DriverC><Dir Dirname = 'MSQCOS22'><File FileName = 'Commond.tt'>"
                +"</File><Dir DirName = 'MSQD58'><File FileName = 'Request.oci'></File></Dir></Dir><File FileName = '_desktop.ini'></File>"
       +" <File FileName = 'io.sys'></File></DriverC></FileSystem> ";
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlstring);
            GetXmlPropertyValue(xmlDoc.ChildNodes);
            Console.Read();
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
    }
}
