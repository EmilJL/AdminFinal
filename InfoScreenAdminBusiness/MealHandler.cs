using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfoScreenAdminDAL.Entities;

namespace InfoScreenAdminBusiness
{
    public class MealHandler : DBHandler
    {
        public Meal GetMeal(int id)
        {
            Meal meal = Model.Meals.Where(m => m.Id == id).FirstOrDefault();
            return meal;
        }
        public List<Meal> GetMealsForLunchPlan(int id)
        {
            var mvls = Model.MealsVsLunchPlans.Where(mvl => mvl.LunchPlanId == id);
            List<Meal> meals = new List<Meal>();
            foreach (var mvl in mvls)
            {
                Meal meal = new Meal();
                meal = Model.Meals.Where(m => m.Id == mvl.MealId).FirstOrDefault();
                meals.Add(meal);
            }
            return meals;
        }
        public bool AddMeal(Meal meal)
        {
            try
            {
                DbAccess.AddMeal(meal);
                Model.Meals.Add(meal);
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }
        }
        public bool DeleteMeal(int id)
        {
            try
            {
                DbAccess.DeleteMeal(id);
                Model.Meals.RemoveAt(Model.Meals.IndexOf(Model.Meals.Where(m => m.Id == id).FirstOrDefault()));
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }
        }
        public bool UpdateMeal(Meal meal)
        {
            try
            {
                DbAccess.UpdateMeal(meal);
                var me = Model.Meals.Where(m => m.Id == meal.Id).FirstOrDefault();
                me.Description = meal.Description;
                me.TimesChosen = meal.TimesChosen;
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
