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
using Windows.UI.Popups;

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
                if (model.LunchPlans.Where(l => l.Week == week).ToList().Count > 1)
                {
                    for (int i = 0; i < model.LunchPlans.Where(l => l.Week == week).ToList().Count - 1; i++)
                    {
                        lunchPlanHandler.DeleteLunchPlan(model.LunchPlans.Where(l => l.Week == week).ToList()[i].Id);
                    }
                }
                lunchPlan = model.LunchPlans.Where(l => l.Week == week).FirstOrDefault();
                List<MealsVsLunchPlans> mealsVsLunchPlan = new List<MealsVsLunchPlans>();
                mealsVsLunchPlan = model.MealsVsLunchPlans.Where(mvl => mvl.LunchPlanId == lunchPlan.Id).ToList();
                int mealId = 0;
                List<MealsVsLunchPlans> mvsToDelete = new List<MealsVsLunchPlans>();
                if (mealsVsLunchPlan.Any(mvl => mvl.Weekday.ToLower() == "monday" || mvl.Weekday.ToLower() == "mandag"))
                {
                    if (mealsVsLunchPlan.Where(mvl => mvl.Weekday.ToLower() == "monday").ToList().Count > 1)
                    {
                        for (int i = 0; i < mealsVsLunchPlan.Where(mvl => mvl.Weekday.ToLower() == "monday").ToList().Count - 1; i++)
                        {
                            mvsToDelete.Add(mealsVsLunchPlan.Where(mvl => mvl.Weekday.ToLower() == "monday").ToList()[i]);
                            lunchPlanHandler.DeleteMealVsLunchPlan(mvsToDelete.Last().Id);
                        }
                    }
                    mealId = mealsVsLunchPlan.Where(mvl => mvl.Weekday.ToLower() == "monday" || mvl.Weekday.ToLower() == "mandag").LastOrDefault().MealId;
                    TBoxMonday.Text = model.Meals.Where(m => m.Id == mealId).FirstOrDefault().Description;
                }
                else
                {
                    TBoxMonday.Text = "";
                }
                if (mealsVsLunchPlan.Any(mvl => mvl.Weekday.ToLower() == "tuesday" || mvl.Weekday.ToLower() == "tirsdag"))
                {
                    if (mealsVsLunchPlan.Where(mvl => mvl.Weekday.ToLower() == "tuesday").ToList().Count > 1)
                    {
                        for (int i = 0; i < mealsVsLunchPlan.Where(mvl => mvl.Weekday.ToLower() == "tuesday").ToList().Count - 1; i++)
                        {
                            mvsToDelete.Add(mealsVsLunchPlan.Where(mvl => mvl.Weekday.ToLower() == "tuesday").ToList()[i]);
                            lunchPlanHandler.DeleteMealVsLunchPlan(mvsToDelete.Last().Id);
                        }
                    }
                    mealId = mealsVsLunchPlan.Where(mvl => mvl.Weekday.ToLower() == "tuesday" || mvl.Weekday.ToLower() == "tirsdag").LastOrDefault().MealId;
                    TBoxTuesday.Text = model.Meals.Where(m => m.Id == mealId).FirstOrDefault().Description;
                }
                else
                {
                    TBoxTuesday.Text = "";
                }
                if (mealsVsLunchPlan.Any(mvl => mvl.Weekday.ToLower() == "wednesday" || mvl.Weekday.ToLower() == "onsdag"))
                {
                    if (mealsVsLunchPlan.Where(mvl => mvl.Weekday.ToLower() == "wednesday").ToList().Count > 1)
                    {
                        for (int i = 0; i < mealsVsLunchPlan.Where(mvl => mvl.Weekday.ToLower() == "wednesday").ToList().Count - 1; i++)
                        {
                            mvsToDelete.Add(mealsVsLunchPlan.Where(mvl => mvl.Weekday.ToLower() == "wednesday").ToList()[i]);
                            lunchPlanHandler.DeleteMealVsLunchPlan(mvsToDelete.Last().Id);
                        }
                    }
                    mealId = mealsVsLunchPlan.Where(mvl => mvl.Weekday.ToLower() == "wednesday" || mvl.Weekday.ToLower() == "onsdag").LastOrDefault().MealId;
                    TBoxWednesday.Text = model.Meals.Where(m => m.Id == mealId).FirstOrDefault().Description;
                }
                else
                {
                    TBoxWednesday.Text = "";
                }
                if (mealsVsLunchPlan.Any(mvl => mvl.Weekday.ToLower() == "thursday" || mvl.Weekday.ToLower() == "torsdag"))
                {
                    if (mealsVsLunchPlan.Where(mvl => mvl.Weekday.ToLower() == "thursday").ToList().Count > 1)
                    {
                        for (int i = 0; i < mealsVsLunchPlan.Where(mvl => mvl.Weekday.ToLower() == "thursday").ToList().Count - 1; i++)
                        {
                            mvsToDelete.Add(mealsVsLunchPlan.Where(mvl => mvl.Weekday.ToLower() == "thursday").ToList()[i]);
                            lunchPlanHandler.DeleteMealVsLunchPlan(mvsToDelete.Last().Id);
                        }
                    }
                    mealId = mealsVsLunchPlan.Where(mvl => mvl.Weekday.ToLower() == "thursday" || mvl.Weekday.ToLower() == "torsdag").LastOrDefault().MealId;
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
                        if (mealsVsLunchPlan.Where(mvl => mvl.Weekday.ToLower() == "friday").ToList().Count > 1)
                        {
                            for (int i = 0; i < mealsVsLunchPlan.Where(mvl => mvl.Weekday.ToLower() == "friday").ToList().Count - 1; i++)
                            {
                                mvsToDelete.Add(mealsVsLunchPlan.Where(mvl => mvl.Weekday.ToLower() == "friday").ToList()[i]);
                                lunchPlanHandler.DeleteMealVsLunchPlan(mvsToDelete.Last().Id);
                            }
                        }
                        mealId = mealsVsLunchPlan.Where(mvl => mvl.Weekday.ToLower() == "friday" || mvl.Weekday.ToLower() == "fredag").LastOrDefault().MealId;
                        TBoxFriday.Text = model.Meals.Where(m => m.Id == mealId).FirstOrDefault().Description;
                    }
                    else
                    {
                        TBoxFriday.Text = "";
                    }
                }
                foreach (MealsVsLunchPlans mvs in mvsToDelete)
                {
                    model.MealsVsLunchPlans.Remove(mvs);
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
            model = dbHandler.DbAccess.GetDataAndCreateModel();
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
            List<Meal> mealsToAddToMvS = new List<Meal>();
            List<string> weekdaysForMealsToAddToMvS = new List<string>();
            if (TBoxMonday.Text != "")
            {
                if (model.MealsVsLunchPlans.Any(mvsl => mvsl.LunchPlanId == lunchPlan.Id && mvsl.Weekday == "Monday") && TBoxMonday.Text.ToLower() == model.Meals.Where(m => m.Id == model.MealsVsLunchPlans.Where(mvsl => mvsl.LunchPlanId == lunchPlan.Id && mvsl.Weekday == "Monday").FirstOrDefault().MealId).FirstOrDefault().Description.ToLower())
                {
                }
                else
                {
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
                        TBlockConsoleLog.Text += $"\nRetten '{meal.Description}' er blevet tilføjet til databasen.";
                    }
                    mealsToAddToMvS.Add(meal);
                    weekdaysForMealsToAddToMvS.Add("Monday");
                }
            }
            else
            {
                if (model.MealsVsLunchPlans.Any(mvs => mvs.LunchPlanId == lunchPlan.Id && mvs.Weekday == "Monday"))
                {
                    lunchPlanHandler.DeleteMealVsLunchPlan(model.MealsVsLunchPlans.Where(mvs => mvs.LunchPlanId == lunchPlan.Id && mvs.Weekday == "Monday").FirstOrDefault().Id);
                }
            }
            if (TBoxTuesday.Text != "")
            {
                if (model.MealsVsLunchPlans.Any(mvsl => mvsl.LunchPlanId == lunchPlan.Id && mvsl.Weekday == "Tuesday") && TBoxTuesday.Text.ToLower() == model.Meals.Where(m => m.Id == model.MealsVsLunchPlans.Where(mvsl => mvsl.LunchPlanId == lunchPlan.Id && mvsl.Weekday == "Tuesday").FirstOrDefault().MealId).FirstOrDefault().Description.ToLower())
                {
                }
                else
                {
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
                        TBlockConsoleLog.Text += $"\nRetten '{meal.Description}' er blevet tilføjet til databasen.";
                    }
                    mealsToAddToMvS.Add(meal);
                    weekdaysForMealsToAddToMvS.Add("Tuesday");
                }
            }
            else
            {
                if (model.MealsVsLunchPlans.Any(mvs => mvs.LunchPlanId == lunchPlan.Id && mvs.Weekday == "Tuesday"))
                {
                    lunchPlanHandler.DeleteMealVsLunchPlan(model.MealsVsLunchPlans.Where(mvs => mvs.LunchPlanId == lunchPlan.Id && mvs.Weekday == "Tuesday").FirstOrDefault().Id);
                }
            }
            if (TBoxWednesday.Text != "")
            {
                if (model.MealsVsLunchPlans.Any(mvsl => mvsl.LunchPlanId == lunchPlan.Id && mvsl.Weekday == "Wednesday") && TBoxWednesday.Text.ToLower() == model.Meals.Where(m => m.Id == model.MealsVsLunchPlans.Where(mvsl => mvsl.LunchPlanId == lunchPlan.Id && mvsl.Weekday == "Wednesday").FirstOrDefault().MealId).FirstOrDefault().Description.ToLower())
                {
                }
                else
                {
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
                        TBlockConsoleLog.Text += $"\nRetten '{meal.Description}' er blevet tilføjet til databasen.";
                    }
                    mealsToAddToMvS.Add(meal);
                    weekdaysForMealsToAddToMvS.Add("Wednesday");
                }
            }
            else
            {
                if (model.MealsVsLunchPlans.Any(mvs => mvs.LunchPlanId == lunchPlan.Id && mvs.Weekday == "Wednesday"))
                {
                    lunchPlanHandler.DeleteMealVsLunchPlan(model.MealsVsLunchPlans.Where(mvs => mvs.LunchPlanId == lunchPlan.Id && mvs.Weekday == "Wednesday").FirstOrDefault().Id);
                }
            }
            if (TBoxThursday.Text != "")
            {
                if (model.MealsVsLunchPlans.Any(mvsl => mvsl.LunchPlanId == lunchPlan.Id && mvsl.Weekday == "Thursday") && TBoxThursday.Text.ToLower() == model.Meals.Where(m => m.Id == model.MealsVsLunchPlans.Where(mvsl => mvsl.LunchPlanId == lunchPlan.Id && mvsl.Weekday == "Thursday").FirstOrDefault().MealId).FirstOrDefault().Description.ToLower())
                {
                }
                else
                {
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
                        TBlockConsoleLog.Text += $"\nRetten '{meal.Description}' er blevet tilføjet til databasen.";
                    }
                    mealsToAddToMvS.Add(meal);
                    weekdaysForMealsToAddToMvS.Add("Thursday");
                }
            }
            else
            {
                if (model.MealsVsLunchPlans.Any(mvs => mvs.LunchPlanId == lunchPlan.Id && mvs.Weekday == "Thursday"))
                {
                    lunchPlanHandler.DeleteMealVsLunchPlan(model.MealsVsLunchPlans.Where(mvs => mvs.LunchPlanId == lunchPlan.Id && mvs.Weekday == "Thursday").FirstOrDefault().Id);
                }
            }
            if (TBoxFriday.Text != "")
            {
                if (model.MealsVsLunchPlans.Any(mvsl => mvsl.LunchPlanId == lunchPlan.Id && mvsl.Weekday == "Friday") && TBoxFriday.Text.ToLower() == model.Meals.Where(m => m.Id == model.MealsVsLunchPlans.Where(mvsl => mvsl.LunchPlanId == lunchPlan.Id && mvsl.Weekday == "Friday").FirstOrDefault().MealId).FirstOrDefault().Description.ToLower())
                {
                }
                else
                {
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
                            TBlockConsoleLog.Text += $"\nRetten '{meal.Description}' er blevet tilføjet til databasen.";
                        }
                        mealsToAddToMvS.Add(meal);
                        weekdaysForMealsToAddToMvS.Add("Friday");
                    }
                }
            }
            else
            {
                if (model.MealsVsLunchPlans.Any(mvs => mvs.LunchPlanId == lunchPlan.Id && mvs.Weekday == "Friday"))
                {
                    lunchPlanHandler.DeleteMealVsLunchPlan(model.MealsVsLunchPlans.Where(mvs => mvs.LunchPlanId == lunchPlan.Id && mvs.Weekday == "Friday").FirstOrDefault().Id);
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
            string searchedText = TBoxSearchField.Text;
            TBoxSearchField.Text = "";
            TBoxSearchField.Text = searchedText;
            TBlockConsoleLog.Text += $"\nMadplanen er blevet gemt i databasen.";
            model = new Model();
            ShowSelectedLunchPlan((int)CmbBoxWeekNumbers.SelectedItem);
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
            if (TBoxSearchField.Text != String.Empty && TBoxSearchField.Text != @"Søg på retter")
            {
                BtnAddDishToDB.IsHitTestVisible = true;
                BtnAddDishToDB.Opacity = 1;
                try
                {
                    ListViewDatabaseDishes.ItemsSource = model.Meals.Where(m => m.Description.ToLower().Contains(TBoxSearchField.Text.ToLower())).OrderByDescending(m => m.TimesChosen);
                }
                catch (ArgumentNullException ex)
                {
                    Debug.Write(ex);
                }
            }
            else
            {
                BtnAddDishToDB.IsHitTestVisible = false;
                BtnAddDishToDB.Opacity = 0.4;
                try
                {
                    ListViewDatabaseDishes.ItemsSource = model.Meals.OrderByDescending(m => m.TimesChosen);
                }
                catch (ArgumentNullException ex)
                {
                    Debug.Write(ex);
                }
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
            TBlockConsoleLog.Text += $"\nBeskeden er blevet gemt i databasen.";
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
                TBlockConsoleLog.Text += $"\nRetten '{meal.Description}' er blevet tilføjet til databasen.";
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
            CmbBoxWeekNumbers.SelectedIndex = GetIso8601WeekOfYear(DateTime.Now) - 1;
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
                string test = ListViewDatabaseDishes.SelectedItem.ToString().ToLower();
                var mealsVsLunchPlansToDelete = model.MealsVsLunchPlans.Where(mvl => mvl.MealId == model.Meals.Where(m => m.Description.ToLower() == ListViewDatabaseDishes.SelectedItem.ToString().ToLower()).FirstOrDefault().Id).ToList();
                foreach (MealsVsLunchPlans mealsVsLunchPlan in mealsVsLunchPlansToDelete)
                {
                    lunchPlanHandler.DeleteMealVsLunchPlan(mealsVsLunchPlan.Id);
                    model.MealsVsLunchPlans.Remove(mealsVsLunchPlan);
                }
            }
            catch (NullReferenceException err)
            {
                Debug.Write($"The dish is not used in any lunchplans, working as intended! Error message: {err}");
            }
            var mealToDelete = model.Meals.Where(m => m.Description.ToLower() == ListViewDatabaseDishes.SelectedItem.ToString().ToLower()).FirstOrDefault();
            mealHandler.DeleteMeal(mealToDelete.Id);
            TBlockConsoleLog.Text += $"\nRetten '{mealToDelete.Description}' er blevet slettet fra databasen, samt tilhørende madplaner.";
            model.Meals.Remove(mealToDelete);
            ListViewDatabaseDishes.ItemsSource = model.Meals.OrderByDescending(m => m.TimesChosen);
            ListViewDatabaseDishes.SelectedIndex = -1;
            ShowSelectedLunchPlan((int)CmbBoxWeekNumbers.SelectedItem);
        }

        private void TBoxTitle_GotFocus(object sender, RoutedEventArgs e)
        {
            if (TBoxTitle.Text == @"Titel her!")
            {
                TBoxTitle.Text = "";
            }
        }

        private void TBoxMessage_GotFocus(object sender, RoutedEventArgs e)
        {
            if (TBoxMessage.Text == @"Besked her!")
            {
                TBoxMessage.Text = "";
            }
        }

        private async void TBoxMessage_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TBoxMessage.Text.Count() >= 200)
            {
                var dialog = new MessageDialog($"Beskeden er nu på {TBoxMessage.Text.Count()} tegn. Det er ikke muligt at gemme en besked, med mere end 200.");
                await dialog.ShowAsync();
            }
        }

        private void TBoxTitle_LostFocus(object sender, RoutedEventArgs e)
        {
            if (TBoxTitle.Text == string.Empty)
            {
                TBoxTitle.Text = @"Titel her!";
            }
        }

        private void TBoxMessage_LostFocus(object sender, RoutedEventArgs e)
        {
            if (TBoxMessage.Text == string.Empty)
            {
                TBoxMessage.Text = @"Besked her!";
            }
        }
    }
}
