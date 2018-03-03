using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

namespace ReflectionHelper
{
    public class ReflectionHelper
    {
        public const string XmlExtension = ".xml";
        public const string ReflectionParams = "ReflectionParams";
        public const string SpaceName = "SpaceName";
        public const string ClassName = "ClassName";
        public const string MethodName = "MethodName";

        public static void StartReflection(string path)
        {
            string filename = Path.GetFileNameWithoutExtension(path);
            string Dirpath = Path.GetDirectoryName(path);
            string xmlFilename = Dirpath + "\\" + filename + XmlExtension;

            Dictionary<string, string> dic = GetParamsFromXml(xmlFilename);

            Assembly assembly = Assembly.LoadFrom(path);
            string classType = String.Format("{0}.{1}", dic[SpaceName], dic[ClassName]);
            Type type = assembly.GetType(classType);
            if (type != null)
            {
                var classInstance = Activator.CreateInstance(type);

                MethodInfo methodInfo = type.GetMethod(dic[MethodName]);
                if (methodInfo != null)
                {
                    methodInfo.Invoke(classInstance, new object[] { });
                }
            }
            else
                throw new ArgumentException("Reflection Helper couldn't find the type you are trying to activate.");
        }

        private static Dictionary<string, string> GetParamsFromXml(string xmlFilePath)
        {
            if (!File.Exists(xmlFilePath))
                throw new FileNotFoundException("Reflection Helper couldn't find the xml file.\nValidate that there is a file: " + xmlFilePath);

            Dictionary<string, string> dic = new Dictionary<string, string>();
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(xmlFilePath);

            XmlNode paramsNode = xDoc.SelectSingleNode("/" + ReflectionParams);
            string space = paramsNode.SelectSingleNode(SpaceName).InnerText;
            dic.Add(SpaceName, space);
            string className = paramsNode.SelectSingleNode(ClassName).InnerText;
            dic.Add(ClassName, className);
            string methodName = paramsNode.SelectSingleNode(MethodName).InnerText;
            dic.Add(MethodName, methodName);

            return dic;
        }
    }
}
