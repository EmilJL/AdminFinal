
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoScreenAdminDAL.Entities
{
    public class Admin
    {
        public int Id { get; set; }
        private string username;
        private byte[] passwordSalt;
        private byte[] passwordHash;

        public Admin()
        {

        }
        public Admin(string username, byte[] passwordSalt, byte[] passwordHash)
        {
            Username = username;
            PasswordSalt = passwordSalt;
            PasswordHash = passwordHash;
        }
        public string Username
        {
            get { return username; }
            set
            {
                username = value;
            }
        }
        public byte[] PasswordSalt
        {
            get { return passwordSalt; }
            set
            {
                passwordSalt = value;
            }
        }
        public byte[] PasswordHash
        {
            get { return passwordHash; }
            set
            {
                passwordHash = value;
            }
        }

        
    }
}
