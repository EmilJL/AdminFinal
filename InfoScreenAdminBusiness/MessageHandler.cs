using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfoScreenAdminDAL.Entities;

namespace InfoScreenAdminBusiness
{
    public class MessageHandler : DBHandler
    {
        public Message GetMessage(int id)
        {
            Message message = Model.Messages.Where(m => m.Id == id).FirstOrDefault();
            return message;
        }
        /// <summary>
        /// Gets the newest message based on logged date in the DB.
        /// </summary>
        /// <returns></returns>
        public Message GetNewestMessage()
        {
            return Model.Messages.OrderByDescending(m => m.Date).FirstOrDefault();
        }
        public bool AddMessage(Message message)
        {
            try
            {
                DbAccess.AddMessage(message);
                Model.Messages.Add(message);
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }
        }
        public bool DeleteMessage(int id)
        {
            try
            {
                DbAccess.DeleteMessage(id);
                Model.Messages.RemoveAt(Model.Messages.IndexOf(Model.Messages.Where(m => m.Id == id).FirstOrDefault()));
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }
        }
        public bool UpdateMessage(Message message)
        {
            try
            {
                DbAccess.UpdateMessage(message);
                var mes = Model.Messages.Where(m => m.Id == message.Id).FirstOrDefault();
                mes.AdminId = message.AdminId;
                mes.Date = message.Date;
                mes.Header = message.Header;
                mes.Text = message.Text;
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }
        }
    }
}
