using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoScreenAdminDAL.Entities
{
    public class Message
    {
        public int Id { get; set; }
        public int AdminId { get; set; }
        private DateTime date;
        private string text;
        private string header;

        public Message()
        {

        }
        public Message(int adminId, DateTime date, string header, string text)
        {
            AdminId = adminId;
            Date = date;
            Header = header;
            Text = text;
        }
        public DateTime Date
        {
            get { return date; }
            set
            {
                date = value;
            }
        }
        public string Text
        {
            get { return text; }
            set
            {
                text = value;
            }
        }
        public string Header
        {
            get { return header; }
            set
            {
                header = value;
            }
        }
    }
}
