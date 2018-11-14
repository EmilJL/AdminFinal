using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoScreenAdminDAL.Entities
{
    public class MealsVsLunchPlans
    {
        public int Id { get; set; }

        private int lunchPlanId;
        private int mealId;
        private string weekday;

        public MealsVsLunchPlans()
        {

        }
        public MealsVsLunchPlans(int lunchPlanId, int mealId, string weekday)
        {
            LunchPlanId = lunchPlanId;
            MealId = mealId;
            Weekday = weekday;
        }

        public string Weekday
        {
            get { return weekday; }
            set { weekday = value; }
        }
        public int MealId
        {
            get { return mealId; }
            set { mealId = value; }
        }
        public int LunchPlanId
        {
            get { return lunchPlanId; }
            set { lunchPlanId = value; }
        }
    }
}
