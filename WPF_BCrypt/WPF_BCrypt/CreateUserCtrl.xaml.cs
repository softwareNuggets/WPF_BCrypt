using BCrypt.Net;
using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Data.SqlClient;

namespace WPF_DCrypt
{
    /// <summary>
    /// Interaction logic for CreateUserCtrl.xaml
    /// </summary>
    public partial class CreateUserCtrl : UserControl
    {
        public CreateUserCtrl()
        {
            InitializeComponent();
        }

        private void BtnGenerate_Click(object sender, RoutedEventArgs e)
        {
            //The range for the cost factor (workfactor) in bcrypt is 4 to 31,
            //but the exact range may depend on the specific implementation of the bcrypt library. 

            if (cboRounds.SelectedItem != null)
            {
                var selectedItem = cboRounds.SelectedItem as ComboBoxItem;
                if (selectedItem == null) return;
                if (selectedItem.Tag == null) return;

                int workfactor = 12;
                try
                {
                    workfactor = int.Parse(selectedItem.Tag.ToString());
                }
                catch { }


                var bc = BCrypt.Net.BCrypt.GenerateSalt(workfactor, SaltRevision.Revision2B);
                TBRandomSalt.Text = bc;
                hashResult.Text = string.Empty;
            }
        }

        private void passwordBox_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TBShowPassword.Visibility = Visibility.Visible;
            TBShowPassword.Text = this.passwordBox.Password;
        }

        private void passwordBox_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TBShowPassword.Visibility = Visibility.Collapsed;
            TBShowPassword.Text = "";
        }

        private void GenerateHash_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.TbLoginName.Text.ToString().Trim()))
            {
                MessageBox.Show("The Login Name is required.");
                return;
            }

            if (string.IsNullOrWhiteSpace(this.passwordBox.Password.ToString().Trim()))
            {
                MessageBox.Show("The password is required.");
                return;
            }

            if (string.IsNullOrWhiteSpace(this.TBRandomSalt.Text.ToString().Trim()))
            {
                MessageBox.Show("The Salt is required.");
                return;
            }


            string bcrypt_salt = TBRandomSalt.Text.ToString();

            string hash = BCrypt.Net.BCrypt.HashPassword(this.passwordBox.Password, bcrypt_salt);

            hashResult.Text = hash;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.TbLoginName.Clear();
            this.passwordBox.Clear();
            this.TBRandomSalt.Text = "";
            this.hashResult.Text = "";
            this.hashResult.Background = new SolidColorBrush(Colors.White);
        }

        private void VerifyHash_Click(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrEmpty(this.passwordBox.Password.Trim()) == true)
            {
                MessageBox.Show("Please try to login with just your password", "FAILURE");
                return;
            }

            if (string.IsNullOrEmpty(this.TBRandomSalt.Text.ToString().Trim()) == true)
            {
                MessageBox.Show("Please generate a salt", "FAILURE");
                return;
            }

            string salt = this.TBRandomSalt.Text.ToString().Trim();

            if (string.IsNullOrEmpty(salt.Trim()) == true)
            {
                MessageBox.Show("Please generate a SALT", "FAILURE");
                return;
            }

            string hash = hashResult.Text;

            string pwd = this.passwordBox.Password;

            // verify 
            // requires two parameters
            // 1) plain text password - the password entered by the user
            // 2) hash value          - remember the hash value contains the salt
            //                        - the salt is used to recreate the hash value during verification process.

            bool rv = BCrypt.Net.BCrypt.Verify(pwd, hash);

            if (rv == true)
            {
                hashResult.Background = new SolidColorBrush(Colors.LightGreen);
            }
            else
            {
                hashResult.Background = new SolidColorBrush(Colors.Pink);
            }


        }

        private void BtnInsert_Click(object sender, RoutedEventArgs e)
        {
            if (this.TbLoginName.Text.ToString().Length == 0)
            {
                MessageBox.Show("You must have a valid Login/Email value value");
                return;
            }

            if (this.passwordBox.Password.ToString().Length == 0)
            {
                MessageBox.Show("You must have a password value");
                return;
            }

            if (TBRandomSalt.Text.Trim().Length == 0)
            {
                MessageBox.Show("You must have a SALT");
                return;
            }

            if (hashResult.Text.Trim().Length == 0)
            {
                MessageBox.Show("You must have a valid One-Way Encryption Pass Encryption value");
                return;
            }

            DB_SQLServer dB_SQL = new DB_SQLServer();
            var conn = dB_SQL.DbConnect();

            string sql = "insert into SITE_UserProfileV2_M(uid,login_name,salt, pwd_hash)";
            sql += " values(newid(),'" + this.TbLoginName.Text.ToString() + "', '" + TBRandomSalt.Text + "','" + hashResult.Text.Trim() + "')";

            SqlCommand cmd = new SqlCommand(sql, conn);
            int rv = cmd.ExecuteNonQuery();

            if (rv == 1)
            {
                MessageBox.Show("Record has been save... You can now login");
                return;
            }
            else
            {
                MessageBox.Show("Something went wrong, this record was not saved.");
                return;
            }

        }
        
    }
}
