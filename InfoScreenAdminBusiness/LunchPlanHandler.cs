using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfoScreenAdminDAL.Entities;

namespace InfoScreenAdminBusiness
{
    public class LunchPlanHandler : DBHandler
    {
        public LunchPlan GetLunchPlan(int id)
        {
            LunchPlan lunchPlan = Model.LunchPlans.Where(m => m.Id == id).FirstOrDefault();
            return lunchPlan;
        }
        public bool AddLunchPlan(LunchPlan lunchPlan)
        {
            try
            {
                DbAccess.AddLunchPlan(lunchPlan);
                Model.LunchPlans.Add(lunchPlan);
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }
        }
        public List<LunchPlan> GetLunchPlansForWeek(int week)
        {
            List<LunchPlan> lunchPlans = DbAccess.GetLunchPlansForWeek(week);
            return lunchPlans;
        }
        public bool DeleteLunchPlan(int id)
        {
            try
            {
                DbAccess.DeleteLunchPlan(id);
                Model.LunchPlans.RemoveAt(Model.LunchPlans.IndexOf(Model.LunchPlans.Where(l => l.Id == id).FirstOrDefault()));
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }
        }
        public bool UpdateLunchPlan(LunchPlan lunchPlan)
        {
            try
            {
                DbAccess.UpdateLunchPlan(lunchPlan);
                var lp = Model.LunchPlans.Where(l => l.Id == lunchPlan.Id).FirstOrDefault();
                lp.Week = lunchPlan.Week;
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }
        }
        public bool AddMealVsLunchPlan(MealsVsLunchPlans mealsVsLunchPlans)
        {
            try
            {
                DbAccess.AddMealsVsLunchPlans(mealsVsLunchPlans);
                Model.MealsVsLunchPlans.Add(mealsVsLunchPlans);
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }
        }
        public bool DeleteMealVsLunchPlan(int id)
        {
            try
            {
                DbAccess.DeleteMealsVsLunchPlan(id);
                Model.MealsVsLunchPlans.Remove(Model.MealsVsLunchPlans.Where(mvsl => mvsl.Id == id).FirstOrDefault());
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
