using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MiniTorrentClient
{
    public class ReflectionHandler
    {
        public readonly string ClassEncryptor = "Encryptor";
        public readonly string ClassDecryptor = "Decryptor";
        public readonly string MethodEnc3 = "EncryptMove3";
        public readonly string MethodEnc6 = "EncryptMove6";
        public readonly string MethodDec3 = "DecryptMove3";
        public readonly string MethodDec6 = "DecryptMove6";
        public readonly string EncString = "MOST BADASS ENCRYPTION STRING";
        public string ManipulatedString { get; set; }

        public void HandleReflection(string path)
        {
            Assembly dll = Assembly.LoadFile(path);
            Type[] classes = dll.GetTypes();

            MessageBox.Show("String before encryption: " + EncString);

            // Handle encryption
            foreach (var type in classes)
            {
                if (type.Name.Equals(ClassEncryptor))
                {
                    HandleEncDec(type, EncString);
                    break;
                }
            }

            MessageBox.Show("String after encryption: " + ManipulatedString);

            // Handle decryption
            foreach (var type in classes)
            {
                if (type.Name.Equals(ClassDecryptor))
                {
                    HandleEncDec(type, ManipulatedString);
                    break;
                }
            }

            MessageBox.Show("String after decryption: " + ManipulatedString);
        }

        private void HandleEncDec(Type type, string encMsg)
        {
            var instance = Activator.CreateInstance(type);
            MethodInfo[] methods = type.GetMethods();
            foreach (var method in methods)
            {
                if(method.Name.Equals(MethodEnc3) || method.Name.Equals(MethodEnc6) || method.Name.Equals(MethodDec3) || method.Name.Equals(MethodDec6))
                    ManipulatedString = (string)method.Invoke(instance, new object[] { encMsg });
            }
        }
    }
}
