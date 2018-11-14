using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoScreenAdminDAL.Entities
{
    public class Meal
    {
        public int Id { get; set; }
        private int timesChosen;
        private string description;
        public Meal()
        {

        }
        public Meal(string description, int timesChosen = 0)
        {
            Description = description;
            TimesChosen = timesChosen;
        }
        public int TimesChosen
        {
            get { return timesChosen; }
            set { timesChosen = value; }
        }
        public string Description
        {
            get { return description; }
            set { description = value; }
        }
        public override string ToString()
        {
            return Description;
        }
    }
}
