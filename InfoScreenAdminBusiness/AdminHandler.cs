using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using InfoScreenAdminDAL.Entities;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace InfoScreenAdminBusiness
{
    public class AdminHandler : DBHandler
    {
        public Admin GetAdmin(int id)
        {
            Admin admin = Model.Admins.Where(m => m.Id == id).FirstOrDefault();
            return admin;
        }
        public Admin GetAdminByUsername(string username)
        {
            Admin admin = Model.Admins.Where(m => m.Username == username).FirstOrDefault();
            return admin;
        }
        public bool AddAdmin(Admin admin)
        {
            try
            {
                DbAccess.AddAdmin(admin);
                Model.Admins.Add(admin);
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }
        }
        public bool DeleteAdmin(int id)
        {
            try
            {
                DbAccess.DeleteAdmin(id);
                Model.Admins.RemoveAt(Model.Admins.IndexOf(Model.Admins.Where(m => m.Id == id).FirstOrDefault()));
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }
        }
        public bool UpdateAdmin(Admin admin)
        {
            try
            {
                DbAccess.UpdateAdmin(admin);
                var ad = Model.Admins.Where(a => a.Id == admin.Id).FirstOrDefault();
                ad.PasswordHash = admin.PasswordHash;
                ad.PasswordSalt = admin.PasswordSalt;
                ad.Username = admin.Username;
                return true;
            }
            catch (Exception e)
            {
                Debug.Write(e);
                return false;
            }
            
        }

        public bool ChangePassword(int id, string currentPassword, string newPassword)
        {
            Admin admin = GetAdmin(id);
            if (HashPassword(currentPassword, admin.PasswordSalt) == admin.PasswordHash)
            {
                admin.PasswordSalt = CreateSalt();
                admin.PasswordHash = HashPassword(newPassword, admin.PasswordSalt);
                UpdateAdmin(admin);
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool VerifyPassword(string username, string providedPassword)
        {
            Admin admin = GetAdminByUsername(username);
            return VerifyHashedPassword(admin.PasswordHash, admin.PasswordSalt, providedPassword);
        }
        public byte[] CreateSalt()
        {
            var rng = RandomNumberGenerator.Create();
            const int saltSize = 128 / 8;
            var salt = new byte[saltSize];
            rng.GetBytes(salt);
            return salt;
        }
        public byte[] HashPassword(string password, byte[] salt)
        {
            var prf = KeyDerivationPrf.HMACSHA256;
            const int iterCount = 100000;
            const int numBytesRequested = 256 / 8;
            const int saltSize = 128 / 8;
            var subkey = KeyDerivation.Pbkdf2(password, salt, prf, iterCount, numBytesRequested);
            var outputBytes = new byte[13 + salt.Length + subkey.Length];
            outputBytes[0] = 0x01; // format marker
            WriteNetworkByteOrder(outputBytes, 1, (uint)prf);
            WriteNetworkByteOrder(outputBytes, 5, iterCount);
            WriteNetworkByteOrder(outputBytes, 9, saltSize);
            Buffer.BlockCopy(salt, 0, outputBytes, 13, salt.Length);
            Buffer.BlockCopy(subkey, 0, outputBytes, 13 + saltSize, subkey.Length);
            return outputBytes;
        }
        private static void WriteNetworkByteOrder(byte[] buffer, int offset, uint value)
        {
            buffer[offset + 0] = (byte)(value >> 24);
            buffer[offset + 1] = (byte)(value >> 16);
            buffer[offset + 2] = (byte)(value >> 8);
            buffer[offset + 3] = (byte)(value >> 0);
        }

        private static uint ReadNetworkByteOrder(byte[] buffer, int offset)
        {
            return ((uint)(buffer[offset + 0]) << 24)
                | ((uint)(buffer[offset + 1]) << 16)
                | ((uint)(buffer[offset + 2]) << 8)
                | ((uint)(buffer[offset + 3]));
        }
        public bool VerifyHashedPassword(byte[] hashedPassword, byte[] passwordSalt, string providedPassword)
        {
            var decodedHashedPassword = hashedPassword;

            // Wrong version
            if (decodedHashedPassword[0] != 0x01)
                return false;

            // Read header information
            var prf = (KeyDerivationPrf)ReadNetworkByteOrder(decodedHashedPassword, 1);
            var iterCount = (int)ReadNetworkByteOrder(decodedHashedPassword, 5);
            var saltLength = (int)ReadNetworkByteOrder(decodedHashedPassword, 9);

            // Read the salt: must be >= 128 bits
            if (saltLength < 128 / 8)
            {
                return false;
            }
            // *WARNING*  Made a change here
            var salt = new byte[passwordSalt.Length];
            Buffer.BlockCopy(decodedHashedPassword, 13, salt, 0, salt.Length);

            // Read the subkey (the rest of the payload): must be >= 128 bits
            var subkeyLength = decodedHashedPassword.Length - 13 - salt.Length;
            if (subkeyLength < 128 / 8)
            {
                return false;
            }
            var expectedSubkey = new byte[subkeyLength];
            Buffer.BlockCopy(decodedHashedPassword, 13 + salt.Length, expectedSubkey, 0, expectedSubkey.Length);

            // Hash the incoming password and verify it
            var actualSubkey = KeyDerivation.Pbkdf2(providedPassword, salt, prf, iterCount, subkeyLength);
            return actualSubkey.SequenceEqual(expectedSubkey);
        }
    }
}
