using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfoScreenAdminDAL;

namespace InfoScreenAdminBusiness
{
    public class DBHandler
    {
        private DbAccess dbAccess;
        private Model model;
        public DBHandler()
        {
            DbAccess = new DbAccess();
            Model = new Model();
        }
        public Model Model
        {
            get { return model; }
            set { model = value; }
        }
        public DbAccess DbAccess
        {
            get { return dbAccess; }
            set { dbAccess = value; }
        }
    }
}
