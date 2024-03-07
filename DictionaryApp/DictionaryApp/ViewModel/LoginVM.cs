using DictionaryApp.Model;
using DictionaryApp.View;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace DictionaryApp.ViewModel
{
    internal class LoginVM : BaseVM
    {
        private readonly UserModel _user;
        public string Username
        {
            get
            {
                return _user.Username;
            }
            set 
            {
                _user.Username = value;
            }
        }
        public string Password
        {
            get 
            {
                return _user.Password;
            }
            set
            {
                _user.Password = value;
            }
        }
        public LoginVM()
        {
            _user = new UserModel();
        }
        private ICommand _loginCommand;
        public ICommand LoginCommand
        {
            get
            {
                if (_loginCommand == null)
                    _loginCommand = new RelayCommand(ExecuteLogin, CanLogin);
                return _loginCommand;
            }
        }
        public void ExecuteLogin(object parameter)
        {
            if (Username.Equals("Megatron") && Password.Equals("Optimus"))
            {

                LoginWindow loginWindow = Application.Current.Windows.OfType<LoginWindow>().First();
                AdministratorWindow administratorWindow = new AdministratorWindow();
                administratorWindow.Show();
                loginWindow.Close();

            }
            else
            {
                MessageBox.Show("Invalid username or password.");
            }
        }
        public bool CanLogin(object parameter)
        {
            return !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password);
        }
    }
}
