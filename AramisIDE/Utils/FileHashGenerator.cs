using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace AramisIDE.Utils
    {
    class FileHashGenerator
        {
        public static string GetFileHash(string filePath, HashType type = HashType.SHA512)
            {
            if (!File.Exists(filePath))
                return string.Empty;

            System.Security.Cryptography.HashAlgorithm hasher;
            hasher = GetHashType(type);
            StringBuilder buff = new StringBuilder();
            try
                {
                using (FileStream f = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 8192))
                    {
                    hasher.ComputeHash(f);
                    Byte[] hash = hasher.Hash;
                    foreach (Byte hashByte in hash)
                        {
                        buff.Append(string.Format("{0:x2}", hashByte));
                        }
                    }
                }
            catch
                {
                return "Error reading file." + new System.Random(DateTime.Now.Second * DateTime.Now.Millisecond).Next().ToString();
                }
            return buff.ToString();
            }

        private static System.Security.Cryptography.HashAlgorithm GetHashType(HashType type)
            {
            switch (type)
                {
                case HashType.SHA1:
                default:
                    return new SHA1CryptoServiceProvider();

                case HashType.SHA256:
                    return new SHA256Managed();

                case HashType.SHA384:
                    return new SHA384Managed();

                case HashType.SHA512:
                    return new SHA512Managed();

                case HashType.MD5:
                    return new MD5CryptoServiceProvider();

                case HashType.RIPEMD160:
                    return new RIPEMD160Managed();
                }
            }

        public static string GetHash(byte[] content, HashType type = HashType.SHA512)
            {
            System.Security.Cryptography.HashAlgorithm hasher;
            hasher = GetHashType(type);
            StringBuilder buff = new StringBuilder();
            try
                {
                hasher.ComputeHash(content);
                Byte[] hash = hasher.Hash;
                foreach (Byte hashByte in hash)
                    {
                    buff.Append(string.Format("{0:x2}", hashByte));
                    }
                }
            catch
                {
                return "Error reading file." + new System.Random(DateTime.Now.Second * DateTime.Now.Millisecond).Next().ToString();
                }
            return buff.ToString();
            }

        public enum HashType
            {
            [Description("SHA-1")]
            SHA1,
            [Description("SHA-256")]
            SHA256,
            [Description("SHA-384")]
            SHA384,
            [Description("SHA-512")]
            SHA512,
            [Description("MD5")]
            MD5,
            [Description("RIPEMD-160")]
            RIPEMD160

            }
        }
    }
