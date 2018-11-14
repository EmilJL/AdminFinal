using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfoScreenAdminDAL.Entities;

namespace InfoScreenAdminDAL
{
    public class Model
    {
        DbAccess dbAccess = new DbAccess();
        public Model()
        {
            dbAccess = new DbAccess();
            Model model = dbAccess.GetDataAndCreateModel();
            Admins = model.Admins;
            LunchPlans = model.LunchPlans;
            Messages = model.Messages;
            Meals = model.Meals;
            MealsVsLunchPlans = model.MealsVsLunchPlans;
        }
        public Model(ObservableCollection<Admin> admins, ObservableCollection<LunchPlan> lunchPlans, ObservableCollection<Message> messages, ObservableCollection<Meal> meals, ObservableCollection<MealsVsLunchPlans> mealsVsLunchPlans)
        {
            Admins = admins;
            LunchPlans = lunchPlans;
            Messages = messages;
            Meals = meals;
            MealsVsLunchPlans = mealsVsLunchPlans;
        }
        // PRIVATE SETTERS?
        public ObservableCollection<Meal> Meals { get; set; }
        public ObservableCollection<Admin> Admins { get; set; }
        public ObservableCollection<LunchPlan> LunchPlans { get; set; }
        public ObservableCollection<Message> Messages { get; set; }
        public ObservableCollection<MealsVsLunchPlans> MealsVsLunchPlans { get; set; }
    }
}
