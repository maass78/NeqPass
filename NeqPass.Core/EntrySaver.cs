using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UsefulExtensions.Security;
using UsefulExtensions.Security.Types;

namespace NeqPass.Core
{
    public static class EntrySaver
    {
        private static readonly object _lock = new object();

        private static SymmetricCipherParams GetParams(string password) 
        {
            return new SymmetricCipherParams()
            {
                Salt = Encoding.UTF8.GetBytes("VtwhaKmvPQWu36I1"),
                KeySize = 256,
                HashAlgorithm = UsefulExtensions.Security.Types.HashAlgorithm.MD5,
                Iterations = 2,
                IV = Encoding.UTF8.GetBytes("VtwhaKmvPQWu36I1"),
                Password = password
            };
        } 

        public static void Save(string fileName, List<Entry> entries, string password)
        {
            lock (_lock)
            {
                var cipher = new SymmetricCipher<AesCryptoServiceProvider>(GetParams(password));

                File.WriteAllBytes(fileName, cipher.Encrypt(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(entries))));
            }
        }

        public static bool Load(string fileName, string password, out List<Entry> entries)
        {
            try
            {
                var cipher = new SymmetricCipher<AesCryptoServiceProvider>(GetParams(password));

                byte[] bytes = cipher.Decrypt(File.ReadAllBytes(fileName));

                entries = JsonConvert.DeserializeObject<List<Entry>>(Encoding.UTF8.GetString(bytes));
                return true;
            }
            catch (Exception ex)
            {
                entries = null;
                return false;
            }
        }

        public static byte[] Encrypt(byte[] bytes, string password) 
        {
            return new SymmetricCipher<AesCryptoServiceProvider>(GetParams(password)).Encrypt(bytes);
        }

        public static byte[] Decrypt(byte[] bytes, string password)
        {
            return new SymmetricCipher<AesCryptoServiceProvider>(GetParams(password)).Decrypt(bytes); 
        }
    }
}
