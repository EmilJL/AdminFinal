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

        //See below.
        private void UpdateContent()
        {
        }

        /// <summary>
        /// This is ment for the possibility of making the application responsive.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateContent();
        }

        /// <summary>
        /// Display lunchplan for given week and checks weither there's any duplicates in the DB, and if so, removes them.
        /// </summary>
        /// <param name="week">The week number of the lunchplan to display.</param>
        public void ShowSelectedLunchPlan(int week)
        {
            //If no meal is selected, calls function to make the buttons for adding meals to weekdays unavailable to click and greyed out.
            if (ListViewDatabaseDishes.SelectedIndex == -1)
            {
                AddingMealButtonAccessorChange(false);
            }
            
            //Not sure why this is done here, but unables, and greys out, the button that adds a meal to the DB from the searchfield.
            BtnAddDishToDB.IsHitTestVisible = false;
            BtnAddDishToDB.Opacity = 0.4;


            LunchPlan lunchPlan = new LunchPlan();
            //If more than one lunchplan exists for the selected weeknumber, it's/they're removed from the DB.
            if (model.LunchPlans.Any(l => l.Week == week))
            {
                if (model.LunchPlans.Where(l => l.Week == week).ToList().Count > 1)
                {
                    for (int i = 0; i < model.LunchPlans.Where(l => l.Week == week).ToList().Count - 1; i++)
                    {
                        lunchPlanHandler.DeleteLunchPlan(model.LunchPlans.Where(l => l.Week == week).ToList()[i].Id);
                    }
                }

                //Gets the remaining lunchplan.
                lunchPlan = model.LunchPlans.Where(l => l.Week == week).FirstOrDefault();

                //Gets any mealvslunchplans that corresponds to the lunchplan, and adds them to a list.
                List<MealsVsLunchPlans> mealsVsLunchPlan = new List<MealsVsLunchPlans>();
                mealsVsLunchPlan = model.MealsVsLunchPlans.Where(mvl => mvl.LunchPlanId == lunchPlan.Id).ToList();

                int mealId = 0;

                List<MealsVsLunchPlans> mvsToDelete = new List<MealsVsLunchPlans>();

                //Checks weither there's any mvsl for "monday", in the list of mvsl of the current lunchplan.
                if (mealsVsLunchPlan.Any(mvl => mvl.Weekday.ToLower() == "monday" || mvl.Weekday.ToLower() == "mandag"))
                {
                    //If there is more than one, it removes the earlier entries from the DB. Prob ugly way to do it, but since lists are reference types, I decided to do it like this.
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
                //If there is no mvsl for the current weekday, sets the text of the textblock to an empty string.
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
                //If the friday is an even week, sets the text as "Fri"
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
                //Removes the same mvsl that's been removed from the DB, from the Model. Pretty useless since model is refreshed in the end of the function.
                foreach (MealsVsLunchPlans mvs in mvsToDelete)
                {
                    model.MealsVsLunchPlans.Remove(mvs);
                }
            }
            //If there is no lunchplan for the selected week, it sets the textblocks of the weekdays to empty, except for friday, if it's an even week.
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
            //Gets a new model by calling the DB.
            model = dbHandler.DbAccess.GetDataAndCreateModel();
        }

        //Below code and comments is a copypaste:

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

        
        /// <summary>
        /// Is called when a new lunchplan is saved. Checks weither the meals of the lunchplan exists in the DB or not, and adds to their TimesChosen property.
        /// </summary>
        /// <param name="lunchPlan">The lunchplan for which to check the mvsl and meals.</param>
        private void CheckAndAddMealsVsLunchPlans(LunchPlan lunchPlan)
        {
            List<MealsVsLunchPlans> mealsVsLunchPlansToDelete = new List<MealsVsLunchPlans>();
            List<Meal> mealsToAddToMvS = new List<Meal>();
            List<string> weekdaysForMealsToAddToMvS = new List<string>();

            // Checks weither the textfield has any text in it.
            if (TBoxMonday.Text != "")
            {
                // Checks weither there already is a mealsvslunchplan where the week, weekday and meal is identical. If so, it does nothing.
                if (model.MealsVsLunchPlans.Any(mvsl => mvsl.LunchPlanId == lunchPlan.Id && mvsl.Weekday == "Monday") && TBoxMonday.Text.ToLower() == model.Meals.Where(m => m.Id == model.MealsVsLunchPlans.Where(mvsl => mvsl.LunchPlanId == lunchPlan.Id && mvsl.Weekday == "Monday").FirstOrDefault().MealId).FirstOrDefault().Description.ToLower())
                {
                }
                else
                {
                    Meal meal = new Meal();
                    //Checks to see if there is a meal corresponding to the textfield in the DB. If so, it adds 1 to its TimesChosen and updates it in the DB.
                    if (model.Meals.Any(m => m.Description.ToLower() == TBoxMonday.Text.ToLower()))
                    {
                        meal = model.Meals.Where(m => m.Description.ToLower() == TBoxMonday.Text.ToLower()).FirstOrDefault();
                        meal.TimesChosen = meal.TimesChosen + 1;
                        mealHandler.UpdateMeal(meal);
                    }
                    //If there is not a corresponding meal, it adds a new one to the DB. Also outputs it to the log TBlock.
                    else
                    {
                        meal.Description = TBoxMonday.Text;
                        meal.TimesChosen = 1;
                        mealHandler.AddMeal(meal);
                        TBlockConsoleLog.Text += $"\nRetten '{meal.Description}' er blevet tilføjet til databasen.";
                    }
                    //Adds the meal and the day of week to lists used for adding to mealvslunchplans.
                    mealsToAddToMvS.Add(meal);
                    weekdaysForMealsToAddToMvS.Add("Monday");
                }
            }
            //If the textfield is empty, checks weither there's a mealvslunchplan saved for the corresponding week and weekday, if so, it deletes it from the DB.
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
            //Gets a new and updated model, from the DB. This can probably be avoided, if the model gets updated simultationsly to the DB, in the methodcalls above.
            //The issue is that the meals don't have IDs until they've been added to the DB.
            model = dbHandler.DbAccess.GetDataAndCreateModel();

            int counter = 0;
            //Adds the IDs to the meals, then adds them to mvsl.
            foreach (Meal meal in mealsToAddToMvS)
            {
                meal.Id = model.Meals.Where(m => m.Description == meal.Description).FirstOrDefault().Id;
                AddMealsVsLunchPlans(lunchPlan.Id, meal.Id, weekdaysForMealsToAddToMvS[counter]);
                counter++;
            }
            //Refreshes the list of meals, since some might've been added, and others might've gotten a higher number of TimesChosen, than some.
            ListViewDatabaseDishes.ItemsSource = model.Meals.OrderByDescending(m => m.TimesChosen);

            //Refreshes the searchbox to trigger the TextChanged method.
            string searchedText = TBoxSearchField.Text;
            TBoxSearchField.Text = "";
            TBoxSearchField.Text = searchedText;

            //Outputs to the log TBlock
            TBlockConsoleLog.Text += $"\nMadplanen er blevet gemt i databasen.";

            //Refreshes the Model again. (The model calls the DB). There should be no difference between this way of doing it, and the one done a bit earlier.
            model = new Model();

            //Calls the method that displays the lunchplan.
            ShowSelectedLunchPlan((int)CmbBoxWeekNumbers.SelectedItem);
        }
        /// <summary>
        /// Adds MealVsLunchPlan to DB.
        /// </summary>
        /// <param name="lunchPlanId"></param>
        /// <param name="mealId"></param>
        /// <param name="weekday"></param>
        public void AddMealsVsLunchPlans(int lunchPlanId, int mealId, string weekday)
        {
            MealsVsLunchPlans mvs = new MealsVsLunchPlans
            {
                LunchPlanId = lunchPlanId,
                MealId = mealId,
                Weekday = weekday
            };
            //I don't know if this is included to Model anywhere else? Doing that might make some calls to the DB unnecessary.
            lunchPlanHandler.AddMealVsLunchPlan(mvs);
        }
        /*PERHAPS ENABLE SETTING DATE AND HAVING MULTIPLE MESSAGES STORED?
         Pretty sure i implemented the function below, but I might have reverted it through reverting a github commit.*/
        private void BtnSaveMessage_Click(object sender, RoutedEventArgs e)
        {     
        }
        /// <summary>
        /// Displays the corresponding lunchplan, when a new weeknumber is selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CmbBoxWeekNumbers_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            ShowSelectedLunchPlan((int)CmbBoxWeekNumbers.SelectedItem);
        }

        /*The functions below adds the selected meal to the corresponding weekday, depending on which button is clicked. 
        There should prob be some try/catching here.*/
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
        /// <summary>
        /// Creates/grabs a lunchplan and calls a method, to add or update it, to the DB.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSavePlan_Click_1(object sender, RoutedEventArgs e)
        {
            //List<string> mealsOfWeek = new List<string>();
            //mealsOfWeek.Add(TBoxMonday.Text);
            //mealsOfWeek.Add(TBoxTuesday.Text);
            //mealsOfWeek.Add(TBoxWednesday.Text);
            //mealsOfWeek.Add(TBoxThursday.Text);
            //mealsOfWeek.Add(TBoxFriday.Text);
            LunchPlan lunchPlan = new LunchPlan();
            //List<Meal> meals = new List<Meal>();

            int currentWeekNumber = int.Parse(CmbBoxWeekNumbers.SelectedValue.ToString());
            //If lunchplan exists, grabs it from model, and uses it to call method.
            if (model.LunchPlans.Any(l => l.Week == currentWeekNumber))
            {
                lunchPlan = model.LunchPlans.Where(l => l.Week == currentWeekNumber).LastOrDefault();
                CheckAndAddMealsVsLunchPlans(lunchPlan);
            }
            //If lunchplan doesn't exists, creates a new one, grabs the ID from the DB, and calls a method to add it to DB.
            //Note that using an SQL statement, that returns the ID at the same time as adding it, would probably be more clean.
            else
            {
                lunchPlan.Week = currentWeekNumber;
                lunchPlanHandler.AddLunchPlan(lunchPlan);
                lunchPlan.Id = lunchPlanHandler.GetLunchPlansForWeek(currentWeekNumber).FirstOrDefault().Id;
                model.LunchPlans.Add(lunchPlan);
                CheckAndAddMealsVsLunchPlans(lunchPlan);
            }
        }
        /// <summary>
        /// Updates the ListView of meals, and shows only those, which description includes the searchterm
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// <summary>
        /// Adds meal to DB and outputs it to the logbox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
