using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUi3Sandbox
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public List<DateTimeOffset> DateSelections = new List<DateTimeOffset>();

        public DateTimeOffset currentLowBound { get; set; }
        public DateTimeOffset currentHighBound { get; set; }

        //public DateTimeOffset currentLowBound = new DateTimeOffset();
        //public DateTimeOffset currentHighBound = new DateTimeOffset();

        public MainWindow()
        {
            this.InitializeComponent();
        }

        private void myButton_Click(object sender, RoutedEventArgs e)
        {
            myButton.Content = "Clicked";
        }


        private void MyCalendar_SelectedDatesChanged(CalendarView sender, CalendarViewSelectedDatesChangedEventArgs args)
        {
            // Disable the event handler temporarily
            sender.SelectedDatesChanged -= MyCalendar_SelectedDatesChanged;

            //If clicking on a non-selected date in the UI
            if ( args.AddedDates.Count != 0)
            {
                DateTimeOffset selectedValue = args.AddedDates[0].Date;
               
                if (DateSelections.Count == 0)
                {
                    //Add the first one without any extra logic
                    if (!DateSelections.Contains(selectedValue))
                    {
                        currentLowBound = selectedValue;

                        DateSelections.Add(selectedValue);
                    }
                }
                else { 
                    SelectionLogic(selectedValue);
                }

            }
            //If clicking on a selected date in the UI
            if (args.RemovedDates.Count != 0)
            {
                DateTimeOffset selectedValue = args.RemovedDates[0].Date;

                SelectionLogic(selectedValue);

            }

            //Prepare the UI
            sender.SelectedDates.Clear();
            sender.SelectedDates.Add(currentLowBound);
            sender.SelectedDates.Add(currentHighBound);

            // Loop through all days between the two dates
            for (DateTimeOffset datesBetween = currentLowBound; datesBetween <= currentHighBound; datesBetween = datesBetween.AddDays(1))
            {
                // Do something with the current date
                sender.SelectedDates.Add(datesBetween);
            }

            // Re-enable the event handler
            sender.SelectedDatesChanged += MyCalendar_SelectedDatesChanged;

            string outPutLow = $"LowBound: {currentLowBound}";
            string outPutHigh = $"HighBound: {currentHighBound}";

            //Output to console and UI
            Debug.WriteLine("New User input");
            Debug.WriteLine(outPutLow);
            Debug.WriteLine(outPutHigh);

            LowBoundary.Text = outPutLow;
            HighBoundary.Text = outPutHigh;

        }

        private void SelectionLogic(DateTimeOffset selectedValue)
        {

            if (DateSelections.Count > 0)
            {
                currentLowBound = DateSelections.Min();
                currentHighBound = DateSelections.Max();

                //If the current low bound and high bound are the same, it means there is only one date selected.
                //We then try to identify if it should be a lower or higher bound
                if (currentLowBound.Equals(currentHighBound))
                {
                    if (selectedValue < DateSelections[0])
                    {
                        currentLowBound = selectedValue;
                    }
                    else
                    {
                        currentHighBound = selectedValue;
                    }
                }
                else //For all other cases
                {
                    if(selectedValue == currentLowBound || selectedValue == currentHighBound)
                    {
                        currentLowBound = selectedValue;
                        currentHighBound = selectedValue;
                        DateSelections.Clear();
                        DateSelections.Add(selectedValue);
                        return;
                    }

                    //Replace if between high and low bound
                    if (selectedValue > currentLowBound && selectedValue < currentHighBound)
                    {
                        var dif1 = selectedValue - currentLowBound;
                        var dif2 = currentHighBound - selectedValue;

                        if (dif1 < dif2)
                        {
                            //Remove existing low bound if the changed date is closer to the low bound
                            DateSelections.Remove(currentLowBound);
                            currentLowBound = selectedValue;
                        }
                        else
                        {
                            //Remove existing high bound f the changed date is closer to the low bound
                            DateSelections.Remove(currentHighBound);
                            currentHighBound = selectedValue;
                        }

                    }

                    //Replace high bound
                    if (selectedValue > currentHighBound)
                    {
                        //Remove existing high bound
                        DateSelections.Remove(currentHighBound);
                        currentHighBound = selectedValue;
                    }

                    //Replace high bound
                    else if (selectedValue < currentLowBound)
                    {
                        //Remove existing low bound
                        DateSelections.Remove(currentLowBound);
                        currentLowBound = selectedValue;
                    }
                }
                //Add the selected date to the list

                //Add the first one without any extra logic
                if (!DateSelections.Contains(selectedValue))
                {

                    DateSelections.Add(selectedValue);
                }
                
            }
        }
    }
}
