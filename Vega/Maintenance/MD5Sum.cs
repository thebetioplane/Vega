using System;
using System.IO;

namespace Vega.Maintenance
{
    public class MD5Sum
    {
        public byte[] Hash { get; private set; }
        public MD5Sum(Stream stream)
        {
            using (var m = System.Security.Cryptography.MD5.Create())
            {
                this.Hash = m.ComputeHash(stream);
            }
        }

        private MD5Sum()
        {
            this.Hash = new byte[16];
        }

        public static implicit operator MD5Sum (byte[] aob)
        {
            if (aob.Length != 16)
                throw new ArgumentException("MD5 hashes must have a length of 16 bytes");
            MD5Sum obj = new MD5Sum();
            for (int i = 0; i < 16; ++i)
                obj.Hash[i] = aob[i];
            return obj;
        }

        public bool Equals(MD5Sum other)
        {
            if (other == null)
                return false;
            for (int i = 0; i < 16; ++i)
            {
                if (this.Hash[i] != other.Hash[i])
                    return false;
            }
            return true;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as MD5Sum);
        }

        public override int GetHashCode()
        {
            int hashCode = 5381;
            for (int i = 0; i < 16; ++i)
            {
                hashCode += (hashCode << 5) + this.Hash[i];
            }
            return hashCode;
        }

        public static implicit operator byte[] (MD5Sum obj)
        {
            return obj.Hash;
        }
    }
}