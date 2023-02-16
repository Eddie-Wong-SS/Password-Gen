using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Password_Gen
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Data Types
        private int passLength;
        private bool flag = false;
        private string password;

        PasswordGenerator generatePass = new PasswordGenerator();

        #endregion

        #region Constructor
        public MainWindow()
        {
            InitializeComponent();
        }
        #endregion

        #region Unused element methods
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void Generated_Password_Box_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Length_Box_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        #endregion

        #region Element Functions
        //Clears out the default text in Length_Box to be more convinient for the user
        private void Length_Box_Focus(object sender, RoutedEventArgs e)
        {
            Length_Box.Clear();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            flag = checkLengthBoxRequirements(Length_Box.Text);
            
            if(flag == true)
            {
                password = generatePass.Generate
                    (passLength, upperCheckBox.IsChecked.GetValueOrDefault(), 
                    numberCheckBox.IsChecked.GetValueOrDefault(), 
                    specialCheckBox.IsChecked.GetValueOrDefault());
                
                Generated_Password_Box.Text = password;

                //Copy button was disabled by default to prevent accidental copying of the default text of Generated_Password_Box 
                Copy_Button.IsEnabled = true;
            }
        }

        //Copies the generated password to the system clipboard
        private void Copy_Button_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(Generated_Password_Box.Text);
            MessageBox.Show("Password Copied!");
        }
        #endregion

        #region Other Functions
        //Checks if the input of the Password Length textbox is an integer, and if it matches the length requirements
        private bool checkLengthBoxRequirements(string length)
        {
            bool fitsReqs = false;
            //TryParse is used instead of a try catch block to allow the user to continue using the app in the case of an error
            //TryParse returns a boolean
            if (Int32.TryParse(length, out passLength))
            {
                //Checks that user specified password length is valid(Default minimum 8 and maximum 32)
                if (passLength < 8 || passLength > 32)
                {
                    MessageBox.Show("Please only " +
                "enter a value between 8 to 32 in the password length box. " +
                "Value entered: " + passLength);
                    return fitsReqs;
                }
                //Flag is only true
                else return fitsReqs = true;
            }
            //If TryParse returns false that indicates the input is NaN
            else
            {
                MessageBox.Show("\"" + Length_Box.Text + "\" is not a digit, please enter only digits" +
                " in range of (8 - 32) in the password length box");
                return fitsReqs;
            }
        }
        #endregion

    }
}
