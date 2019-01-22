using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using InfoScreenAdminDAL.Entities;
using InfoScreenAdminDAL;
using InfoScreenAdminBusiness;
using System.Globalization;
using System.Diagnostics;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace InfoScreenAdminGUI
{

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private DBHandler dbHandler;
        private LunchPlanHandler lunchPlanHandler;
        private Model model;
        private AdminHandler adminHandler;
        private MealHandler mealHandler;
        private MessageHandler messageHandler;

        public MainPage()
        {
            this.InitializeComponent();
            lunchPlanHandler = new LunchPlanHandler();
            mealHandler = new MealHandler();
            dbHandler = new DBHandler();
            messageHandler = new MessageHandler();
            model = dbHandler.Model;
            CmbBoxWeekNumbers.ItemsSource = Enumerable.Range(1, 52);
            CmbBoxWeekNumbers.SelectedIndex = GetIso8601WeekOfYear(DateTime.Now) - 1;
            ShowSelectedLunchPlan(GetIso8601WeekOfYear(DateTime.Now));
            BtnDeleteDish.IsHitTestVisible = false;
            BtnDeleteDish.Opacity = 0.4;
            try
            {
                ListViewDatabaseDishes.ItemsSource = model.Meals.OrderByDescending(m => m.TimesChosen);
            }
            catch (ArgumentNullException e)
            {
                Debug.Write(e);
            }
            
        }

        private void UpdateContent()
        {
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateContent();
        }
        public void ShowSelectedLunchPlan(int week)
        {
            if (ListViewDatabaseDishes.SelectedIndex == -1)
            {
                AddingMealButtonAccessorChange(false);
            }
            BtnAddDishToDB.IsHitTestVisible = false;
            BtnAddDishToDB.Opacity = 0.4;


            LunchPlan lunchPlan = new LunchPlan();
            
            if (model.LunchPlans.Any(l => l.Week == week))
            {
                lunchPlan = model.LunchPlans.Where(l => l.Week == week).FirstOrDefault();
                var mealsVsLunchPlan = model.MealsVsLunchPlans.Where(mvl => mvl.LunchPlanId == lunchPlan.Id).ToList();
                int mealId = 0;
                if (mealsVsLunchPlan.Any(mvl => mvl.Weekday.ToLower() == "monday" || mvl.Weekday.ToLower() == "mandag"))
                {
                    mealId = mealsVsLunchPlan.Where(mvl => mvl.Weekday.ToLower() == "monday" || mvl.Weekday.ToLower() == "mandag").FirstOrDefault().MealId;
                    TBoxMonday.Text = model.Meals.Where(m => m.Id == mealId).FirstOrDefault().Description;
                }
                else
                {
                    TBoxMonday.Text = "";
                }
                if (mealsVsLunchPlan.Any(mvl => mvl.Weekday.ToLower() == "tuesday" || mvl.Weekday.ToLower() == "tirsdag"))
                {
                    mealId = mealsVsLunchPlan.Where(mvl => mvl.Weekday.ToLower() == "tuesday" || mvl.Weekday.ToLower() == "tirsdag").FirstOrDefault().MealId;
                    TBoxTuesday.Text = model.Meals.Where(m => m.Id == mealId).FirstOrDefault().Description;
                }
                else
                {
                    TBoxTuesday.Text = "";
                }
                if (mealsVsLunchPlan.Any(mvl => mvl.Weekday.ToLower() == "wednesday" || mvl.Weekday.ToLower() == "onsdag"))
                {
                    mealId = mealsVsLunchPlan.Where(mvl => mvl.Weekday.ToLower() == "wednesday" || mvl.Weekday.ToLower() == "onsdag").FirstOrDefault().MealId;
                    TBoxWednesday.Text = model.Meals.Where(m => m.Id == mealId).FirstOrDefault().Description;
                }
                else
                {
                    TBoxWednesday.Text = "";
                }
                if (mealsVsLunchPlan.Any(mvl => mvl.Weekday.ToLower() == "thursday" || mvl.Weekday.ToLower() == "torsdag"))
                {
                    mealId = mealsVsLunchPlan.Where(mvl => mvl.Weekday.ToLower() == "thursday" || mvl.Weekday.ToLower() == "torsdag").FirstOrDefault().MealId;
                    TBoxThursday.Text = model.Meals.Where(m => m.Id == mealId).FirstOrDefault().Description;
                }
                else
                {
                    TBoxThursday.Text = "";
                }
                if (week % 2 == 0)
                {
                    TBoxFriday.Text = "Fri";
                }
                else
                {
                    if (mealsVsLunchPlan.Any(mvl => mvl.Weekday.ToLower() == "friday" || mvl.Weekday.ToLower() == "fredag"))
                    {
                        mealId = mealsVsLunchPlan.Where(mvl => mvl.Weekday.ToLower() == "friday" || mvl.Weekday.ToLower() == "fredag").FirstOrDefault().MealId;
                        TBoxFriday.Text = model.Meals.Where(m => m.Id == mealId).FirstOrDefault().Description;
                    }
                    else
                    {
                        TBoxFriday.Text = "";
                    }
                }
            }
            else
            {
                TBoxMonday.Text = "";
                TBoxTuesday.Text = "";
                TBoxWednesday.Text = "";
                TBoxThursday.Text = "";
                if (week % 2 == 0)
                {
                    TBoxFriday.Text = "Fri";
                }
                else
                {
                    TBoxFriday.Text = "";
                }
            }
        }

        

        // This presumes that weeks start with Monday.
        // Week 1 is the 1st week of the year with a Thursday in it.
        public static int GetIso8601WeekOfYear(DateTime time)
        {
            // Seriously cheat.  If its Monday, Tuesday or Wednesday, then it'll 
            // be the same week# as whatever Thursday, Friday or Saturday are,
            // and we always get those right
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }

            // Return the week of our adjusted day
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

        private void BtnCurrentFoodPlan_Click(object sender, RoutedEventArgs e)
        {
            CmbBoxWeekNumbers.SelectedIndex = GetIso8601WeekOfYear(DateTime.Now) - 1;
        }

        // MAKE THIS BE ON TEXT CHANGED EVENT - ALSO CHANGE TBOX

        // ADD YEAR TO LUNCHPLAN? OR DELETE LUNCHPLANS EVERY YEAR - ADD TEXTBLOCK FOR ERRORMESSAGE. MAKE IT POSSIBLE TO ADD MEALS TO SEVERAL DAYS OF WEEK?

        private void CheckAndAddMealsVsLunchPlans(LunchPlan lunchPlan)
        {
            List<MealsVsLunchPlans> mealsVsLunchPlansToDelete = new List<MealsVsLunchPlans>();
            List<LunchPlan> lunchPlans = new List<LunchPlan>();
            List<Meal> mealsToAddToMvS = new List<Meal>();
            List<string> weekdaysForMealsToAddToMvS = new List<string>();
            lunchPlans = lunchPlanHandler.GetLunchPlansForWeek(lunchPlan.Week);
            if (lunchPlans.Count > 1)
            {
                
                lunchPlans.RemoveAt(lunchPlans.Count - 1);
                
                foreach (LunchPlan lunchP in lunchPlans)
                {
                    lunchPlanHandler.DeleteMealVsLunchPlan(model.MealsVsLunchPlans.Where(mvsl => mvsl.LunchPlanId == lunchP.Id).FirstOrDefault().Id);
                    lunchPlanHandler.DeleteLunchPlan(lunchP.Id);
                    model.MealsVsLunchPlans.Remove(model.MealsVsLunchPlans.Where(m => m.LunchPlanId == lunchP.Id).FirstOrDefault());
                    model.LunchPlans.Remove(model.LunchPlans.Where(l => l.Id == lunchP.Id).FirstOrDefault());
                }
            }
            if (TBoxMonday.Text != "")
            {
                if (model.MealsVsLunchPlans.Any(mvsl => mvsl.LunchPlanId == lunchPlan.Id && mvsl.Weekday == "Monday"))
                {
                    mealsVsLunchPlansToDelete = model.MealsVsLunchPlans.Where(mvsl => mvsl.LunchPlanId == lunchPlan.Id && mvsl.Weekday == "Monday").ToList();
                    //mealsVsLunchPlansToDelete.RemoveAt(mealsVsLunchPlansToDelete.Count - 1);
                    foreach (var mvslToDelete in mealsVsLunchPlansToDelete)
                    {
                        lunchPlanHandler.DeleteMealVsLunchPlan(mvslToDelete.Id);
                        model.MealsVsLunchPlans.Remove(mvslToDelete);
                    }
                }
                Meal meal = new Meal();
                if (model.Meals.Any(m => m.Description.ToLower() == TBoxMonday.Text.ToLower()))
                {
                    meal = model.Meals.Where(m => m.Description.ToLower() == TBoxMonday.Text.ToLower()).FirstOrDefault();
                    meal.TimesChosen = meal.TimesChosen + 1;
                    mealHandler.UpdateMeal(meal);
                }
                else
                {
                    meal.Description = TBoxMonday.Text;
                    meal.TimesChosen = 1;
                    mealHandler.AddMeal(meal);
                }
                mealsToAddToMvS.Add(meal);
                weekdaysForMealsToAddToMvS.Add("Monday");
            }
            if (TBoxTuesday.Text != "")
            {
                if (model.MealsVsLunchPlans.Any(mvsl => mvsl.LunchPlanId == lunchPlan.Id && mvsl.Weekday == "Tuesday"))
                {
                    mealsVsLunchPlansToDelete = model.MealsVsLunchPlans.Where(mvsl => mvsl.LunchPlanId == lunchPlan.Id && mvsl.Weekday == "Tuesday").ToList();
                    foreach (var mvslToDelete in mealsVsLunchPlansToDelete)
                    {
                        lunchPlanHandler.DeleteMealVsLunchPlan(mvslToDelete.Id);
                        model.MealsVsLunchPlans.Remove(mvslToDelete);
                    }
                }
                Meal meal = new Meal();
                if (model.Meals.Any(m => m.Description.ToLower() == TBoxTuesday.Text.ToLower()))
                {
                    meal = model.Meals.Where(m => m.Description.ToLower() == TBoxTuesday.Text.ToLower()).FirstOrDefault();
                    meal.TimesChosen = meal.TimesChosen + 1;
                    mealHandler.UpdateMeal(meal);
                }
                else
                {
                    meal.Description = TBoxTuesday.Text;
                    meal.TimesChosen = 1;
                    mealHandler.AddMeal(meal);                    
                }
                mealsToAddToMvS.Add(meal);
                weekdaysForMealsToAddToMvS.Add("Tuesday");
            }
            if (TBoxWednesday.Text != "")
            {
                if (model.MealsVsLunchPlans.Any(mvsl => mvsl.LunchPlanId == lunchPlan.Id && mvsl.Weekday == "Wednesday"))
                {
                    mealsVsLunchPlansToDelete = model.MealsVsLunchPlans.Where(mvsl => mvsl.LunchPlanId == lunchPlan.Id && mvsl.Weekday == "Wednesday").ToList();
                    foreach (var mvslToDelete in mealsVsLunchPlansToDelete)
                    {
                        lunchPlanHandler.DeleteMealVsLunchPlan(mvslToDelete.Id);
                        model.MealsVsLunchPlans.Remove(mvslToDelete);
                    }
                }
                Meal meal = new Meal();
                if (model.Meals.Any(m => m.Description.ToLower() == TBoxWednesday.Text.ToLower()))
                {
                    meal = model.Meals.Where(m => m.Description.ToLower() == TBoxWednesday.Text.ToLower()).FirstOrDefault();
                    meal.TimesChosen = meal.TimesChosen + 1;
                    mealHandler.UpdateMeal(meal);
                }
                else
                {
                    meal.Description = TBoxWednesday.Text;
                    meal.TimesChosen = 1;
                    mealHandler.AddMeal(meal);
                }
                mealsToAddToMvS.Add(meal);
                weekdaysForMealsToAddToMvS.Add("Wednesday");
            }
            if (TBoxThursday.Text != "")
            {
                if (model.MealsVsLunchPlans.Any(mvsl => mvsl.LunchPlanId == lunchPlan.Id && mvsl.Weekday == "Thursday"))
                {
                    mealsVsLunchPlansToDelete = model.MealsVsLunchPlans.Where(mvsl => mvsl.LunchPlanId == lunchPlan.Id && mvsl.Weekday == "Thursday").ToList();
                    foreach (var mvslToDelete in mealsVsLunchPlansToDelete)
                    {
                        lunchPlanHandler.DeleteMealVsLunchPlan(mvslToDelete.Id);
                        model.MealsVsLunchPlans.Remove(mvslToDelete);
                    }
                }
                Meal meal = new Meal();
                if (model.Meals.Any(m => m.Description.ToLower() == TBoxThursday.Text.ToLower()))
                {
                    meal = model.Meals.Where(m => m.Description.ToLower() == TBoxThursday.Text.ToLower()).FirstOrDefault();
                    meal.TimesChosen = meal.TimesChosen + 1;
                    mealHandler.UpdateMeal(meal);
                }
                else
                {
                    meal.Description = TBoxThursday.Text;
                    meal.TimesChosen = 1;
                    mealHandler.AddMeal(meal);                    
                }
                mealsToAddToMvS.Add(meal);
                weekdaysForMealsToAddToMvS.Add("Thursday");
            }
            if (TBoxFriday.Text != "")
            {
                if (model.MealsVsLunchPlans.Any(mvsl => mvsl.LunchPlanId == lunchPlan.Id && mvsl.Weekday == "Friday"))
                {
                    mealsVsLunchPlansToDelete = model.MealsVsLunchPlans.Where(mvsl => mvsl.LunchPlanId == lunchPlan.Id && mvsl.Weekday == "Friday").ToList();
                    foreach (var mvslToDelete in mealsVsLunchPlansToDelete)
                    {
                        lunchPlanHandler.DeleteMealVsLunchPlan(mvslToDelete.Id);
                        model.MealsVsLunchPlans.Remove(mvslToDelete);
                    }
                }
                if (TBoxFriday.Text != "Fri")
                {
                    Meal meal = new Meal();
                    if (model.Meals.Any(m => m.Description.ToLower() == TBoxFriday.Text.ToLower()))
                    {
                        meal = model.Meals.Where(m => m.Description.ToLower() == TBoxFriday.Text.ToLower()).FirstOrDefault();
                        meal.TimesChosen = meal.TimesChosen + 1;
                        mealHandler.UpdateMeal(meal);
                    }
                    else
                    {
                        meal.Description = TBoxFriday.Text;
                        meal.TimesChosen = 1;
                        mealHandler.AddMeal(meal);
                    }
                    mealsToAddToMvS.Add(meal);
                    weekdaysForMealsToAddToMvS.Add("Friday");
                }
            }
            model = dbHandler.DbAccess.GetDataAndCreateModel();

            int counter = 0;
            foreach (Meal meal in mealsToAddToMvS)
            {
                meal.Id = model.Meals.Where(m => m.Description == meal.Description).FirstOrDefault().Id;
                AddMealsVsLunchPlans(lunchPlan.Id, meal.Id, weekdaysForMealsToAddToMvS[counter]);
                counter++;
            }
            ListViewDatabaseDishes.ItemsSource = model.Meals.OrderByDescending(m => m.TimesChosen);
        }

        public void AddMealsVsLunchPlans(int lunchPlanId, int mealId, string weekday)
        {
            MealsVsLunchPlans mvs = new MealsVsLunchPlans
            {
                LunchPlanId = lunchPlanId,
                MealId = mealId,
                Weekday = weekday
            };
            lunchPlanHandler.AddMealVsLunchPlan(mvs);
            model.MealsVsLunchPlans.Add(mvs);
        }

        //PERHAPS ENABLE SETTING DATE AND HAVING MULTIPLE MESSAGES STORED?
        private void BtnSaveMessage_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void CmbBoxWeekNumbers_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            ShowSelectedLunchPlan((int)CmbBoxWeekNumbers.SelectedItem);
        }

        private void BtnAddDishMonday_Click_1(object sender, RoutedEventArgs e)
        {
            TBoxMonday.Text = ListViewDatabaseDishes.SelectedItem.ToString();
        }

        private void BtnAddDishTuesdsay_Click_1(object sender, RoutedEventArgs e)
        {
            TBoxTuesday.Text = ListViewDatabaseDishes.SelectedItem.ToString();
        }

        private void BtnAddDishWednesday_Click_1(object sender, RoutedEventArgs e)
        {
            TBoxWednesday.Text = ListViewDatabaseDishes.SelectedItem.ToString();
        }

        private void BtnAddDishThursday_Click_1(object sender, RoutedEventArgs e)
        {
            TBoxThursday.Text = ListViewDatabaseDishes.SelectedItem.ToString();
        }

        private void BtnAddDishFriday_Click_1(object sender, RoutedEventArgs e)
        {
            TBoxFriday.Text = ListViewDatabaseDishes.SelectedItem.ToString();
        }

        private void BtnSavePlan_Click_1(object sender, RoutedEventArgs e)
        {
            List<string> mealsOfWeek = new List<string>();
            mealsOfWeek.Add(TBoxMonday.Text);
            mealsOfWeek.Add(TBoxTuesday.Text);
            mealsOfWeek.Add(TBoxWednesday.Text);
            mealsOfWeek.Add(TBoxThursday.Text);
            mealsOfWeek.Add(TBoxFriday.Text);
            //if (mealsOfWeek.Any(s => s == ""))
            //{
            //    // ADD ERRORMESSAGE
            //    return;
            //}
            LunchPlan lunchPlan = new LunchPlan();
            List<Meal> meals = new List<Meal>();

            int currentWeekNumber = int.Parse(CmbBoxWeekNumbers.SelectedValue.ToString());
            if (model.LunchPlans.Any(l => l.Week == currentWeekNumber))
            {
                lunchPlan = model.LunchPlans.Where(l => l.Week == currentWeekNumber).LastOrDefault();
                CheckAndAddMealsVsLunchPlans(lunchPlan);
            }
            else
            {
                lunchPlan.Week = currentWeekNumber;
                lunchPlanHandler.AddLunchPlan(lunchPlan);
                lunchPlan.Id = lunchPlanHandler.GetLunchPlansForWeek(currentWeekNumber).FirstOrDefault().Id;
                model.LunchPlans.Add(lunchPlan);
                CheckAndAddMealsVsLunchPlans(lunchPlan);
            }
        }

        private void TBoxSearchField_TextChanged(object sender, TextChangedEventArgs e)
        {
            ListViewDatabaseDishes.SelectedIndex = -1;
            if (TBoxSearchField.Text != String.Empty)
            {
                BtnAddDishToDB.IsHitTestVisible = true;
                BtnAddDishToDB.Opacity = 1;
            }
            else
            {
                BtnAddDishToDB.IsHitTestVisible = false;
                BtnAddDishToDB.Opacity = 0.4;
            }
            try
            {
                ListViewDatabaseDishes.ItemsSource = model.Meals.Where(m => m.Description.ToLower().Contains(TBoxSearchField.Text.ToLower())).OrderByDescending(m => m.TimesChosen);
            }
            catch (ArgumentNullException ex)
            {
                Debug.Write(ex);
            }
            
        }

        private void TBoxSearchField_GotFocus(object sender, RoutedEventArgs e)
        {
            if (TBoxSearchField.Text == @"Søg på retter")
            {
                TBoxSearchField.Text = "";
            }
        }

        private void BtnSaveMessage_Click_1(object sender, RoutedEventArgs e)
        {
            Message message = model.Messages.Last();
            message.Text = TBoxMessage.Text;
            message.Header = TBoxTitle.Text;
            //INSERT CURRENT ADMIN
            message.AdminId = 1;
            message.Date = DateTime.Now;
            messageHandler.UpdateMessage(message);
        }

        private void BtnAddDishToDB_Click(object sender, RoutedEventArgs e)
        {
            if (!model.Meals.Any(m=> m.Description.ToLower() == TBoxSearchField.Text.ToLower()))
            {
                Meal meal = new Meal
                {
                    Description = TBoxSearchField.Text,
                    TimesChosen = 0
                };
                mealHandler.AddMeal(meal);
                model.Meals.Add(meal);
                TBoxSearchField.Text = "";
                ListViewDatabaseDishes.ItemsSource = model.Meals.OrderByDescending(m => m.TimesChosen);
            }
        }

        

       

        /// <summary>
        /// Enable or disables the ability to click the buttons that adds meals to a lunchplan, or deletes them from the DB, by calling corresponding functions.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListViewDatabaseDishes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListViewDatabaseDishes.SelectedIndex == -1)
            {
                AddingMealButtonAccessorChange(false);
                DeletingMealButtonAccessorChange(false);
            }
            else
            {
                AddingMealButtonAccessorChange(true);
                DeletingMealButtonAccessorChange(true);
            }
        }
        /// <summary>
        /// Sets the button for deleting meals from DB as either clickable, or not. Also changes the opacity of the button, to reflect its availability.
        /// </summary>
        /// <param name="access"></param>
        private void DeletingMealButtonAccessorChange(bool access)
        {
            BtnDeleteDish.IsHitTestVisible = access;
            if (access == false)
            {
                BtnDeleteDish.Opacity = 0.4;
            }
            else
            {
                BtnDeleteDish.Opacity = 1;
            }
        }
        /// <summary>
        /// Sets the buttons for adding meals to a lunchplan, as either clickable, or not. Also changes the opacity of the buttons, to reflect their availability.
        /// </summary>
        /// <param name="access"></param>
        private void AddingMealButtonAccessorChange(bool access)
        {
            if (access == false)
            {
                BtnAddDishMonday.Opacity = 0.4;
                BtnAddDishTuesdsay.Opacity = 0.4;
                BtnAddDishWednesday.Opacity = 0.4;
                BtnAddDishThursday.Opacity = 0.4;
                BtnAddDishFriday.Opacity = 0.4;
            }
            else
            {
                BtnAddDishMonday.Opacity = 1;
                BtnAddDishTuesdsay.Opacity = 1;
                BtnAddDishWednesday.Opacity = 1;
                BtnAddDishThursday.Opacity = 1;
                BtnAddDishFriday.Opacity = 1;
            }
            BtnAddDishMonday.IsHitTestVisible = access;
            BtnAddDishTuesdsay.IsHitTestVisible = access;
            BtnAddDishWednesday.IsHitTestVisible = access;
            BtnAddDishThursday.IsHitTestVisible = access;
            BtnAddDishFriday.IsHitTestVisible = access;
        }


        /// <summary>
        /// Changes the CBox selection to that of the current week of the year.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCurrentFoodPlan_Click_1(object sender, RoutedEventArgs e)
        {
            ShowSelectedLunchPlan(GetIso8601WeekOfYear(DateTime.Now));
        }

        /// <summary>
        /// Deletes the selected Meal and its corresponding MealsVsLunchPlans, from the model and the DB. Then it refreshes the ListView.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnDeleteDish_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                lunchPlanHandler.DeleteMealVsLunchPlan(model.MealsVsLunchPlans.Where(mvl => mvl.MealId == model.Meals.Where(m => m.Description.ToLower() == ListViewDatabaseDishes.SelectedItem.ToString().ToLower()).FirstOrDefault().Id && mvl.LunchPlanId == CmbBoxWeekNumbers.SelectedIndex - 1).FirstOrDefault().Id);
                model.MealsVsLunchPlans.Remove(model.MealsVsLunchPlans.Where(mvl => mvl.MealId == model.Meals.Where(m => m.Description.ToLower() == ListViewDatabaseDishes.SelectedItem.ToString().ToLower()).FirstOrDefault().Id && mvl.LunchPlanId == CmbBoxWeekNumbers.SelectedIndex - 1).FirstOrDefault());
            }
            catch (NullReferenceException err)
            {
                Debug.Write($"The dish is not used in any lunchplans, working as intended! Error message: {err}");
            }
            
            mealHandler.DeleteMeal(model.Meals.Where(m => m.Description.ToLower() == ListViewDatabaseDishes.SelectedItem.ToString().ToLower()).FirstOrDefault().Id);
            model.Meals.Remove(model.Meals.Where(m => m.Description.ToLower() == ListViewDatabaseDishes.SelectedItem.ToString().ToLower()).FirstOrDefault());

            ListViewDatabaseDishes.ItemsSource = model.Meals.OrderByDescending(m => m.TimesChosen);
            ListViewDatabaseDishes.SelectedIndex = -1;
        }
    }
}
