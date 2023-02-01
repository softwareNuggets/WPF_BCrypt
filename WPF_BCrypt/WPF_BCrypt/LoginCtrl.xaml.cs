using Microsoft.Data.SqlClient;
using System;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WPF_DCrypt
{

    public partial class LoginCtrl : UserControl
    {
        public delegate void CloseFormHandler();
        public event CloseFormHandler OnCloseForm;

        public delegate void LoginStatusHandler(string message);
        public event LoginStatusHandler OnLoginStatus;

        public LoginCtrl()
        {
            InitializeComponent();
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

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            DB_SQLServer dB_SQL = new DB_SQLServer();
            var conn = dB_SQL.DbConnect();

            if (conn == null) return;
            if (conn.State != ConnectionState.Open) return;

            DataTable? dt = GetUser(this.TbLoginName.Text, conn);

            if (dt == null)
            {
                return;
            }

            var salt = dt.Rows[0]["salt"].ToString();
            var pwd_hash = dt.Rows[0]["pwd_hash"].ToString();

            var test_hv = BCrypt.Net.BCrypt.HashPassword(this.passwordBox.Password, salt);

            if (pwd_hash == test_hv)
            {
                LoginCtrl c = new LoginCtrl();

                c.passwordBox.Background = new SolidColorBrush(Colors.LightGreen);

                if (OnLoginStatus != null)
                {
                    OnLoginStatus("Success:  hash values match");
                }
            }
            else
            {
                if (OnLoginStatus != null)
                {
                    OnLoginStatus("Failure:  hash values did not match");
                }
            }

            conn.Close();

        }

        private DataTable? GetUser(string login_name, SqlConnection conn)
        {
            System.Text.StringBuilder sql = new StringBuilder();

            try
            {
                sql.Append("select uid, login_name, salt, pwd_hash ");
                sql.Append(" from SITE_UserProfileV2_M");
                sql.Append(" where login_name = '" + login_name + "'");

                SqlDataAdapter da;
                DataTable dt = new DataTable("results");

                da = new SqlDataAdapter(sql.ToString(), conn);

                //load the data table object
                da.Fill(dt);

                //free the data adapter object
                da.Dispose();

                return (dt);
            }
            catch
            {
                return (null);
            }
        }


        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.TbLoginName.Text = string.Empty;
            this.passwordBox.Clear();

            if (OnCloseForm != null)
            {
                OnCloseForm();
            }
        }
    }
}
