using System;
using System.Windows;


namespace WPF_DCrypt
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public CreateUserCtrl?  _createUserCtrl;
        public LoginCtrl?       _loginCtrl;


        public MainWindow()
        {
            InitializeComponent();
        }

        private void ResetMyStack()
        {
            if (MyStack.Children.Count > 0)
            {
                var child = MyStack.Children[0];
                MyStack.Children.Clear();
            }
        }
        private void BtnLoadLogin_Click(object sender, RoutedEventArgs e)
        {
            ResetMyStack();

            _loginCtrl = new LoginCtrl();
            _loginCtrl.Focus();
            _loginCtrl.OnCloseForm += _loginCtrl_OnCloseForm;
            _loginCtrl.OnLoginStatus += _loginCtrl_OnLoginStatus;
            MyStack.Children.Add(_loginCtrl);
        }

        private void _loginCtrl_OnLoginStatus(string message)
        {
            MessageBox.Show(message);
        }

        private void _loginCtrl_OnCloseForm()
        {


            _loginCtrl = null;
            MyStack.Children.Clear();

        }

        private void BtnNewUser_Click(object sender, RoutedEventArgs e)
        {
            ResetMyStack();

            _createUserCtrl = new CreateUserCtrl();
            _createUserCtrl.Focus();
            MyStack.Children.Add(_createUserCtrl);
        }
    }
}
