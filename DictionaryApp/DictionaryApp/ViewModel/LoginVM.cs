using DictionaryApp.Model;
using DictionaryApp.View;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.IO;

namespace DictionaryApp.ViewModel
{
    internal class LoginVM : BaseVM
    {
        public LoginVM()
        {
            _user = new UserModel();

            _users = new List<UserModel>();
            ReadUsersFromFile();
        }

        #region Properties and members

        private List<UserModel> _users;

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

        #endregion

        #region File

        private static readonly string _filePath = @"..\..\resources\Database\users.json";
        private void ReadUsersFromFile()
        {
            if (File.Exists(_filePath))
            {
                string jsonString = File.ReadAllText(_filePath);
                _users = JsonConvert.DeserializeObject<List<UserModel>>(jsonString);
            }
        }

        #endregion

        #region Commands

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
        private void ExecuteLogin(object parameter)
        {
            foreach(UserModel user in _users)
            {
                if (Username.Equals(user.Username) && Password.Equals(user.Password))
                {
                    MessageBox.Show("Welcome!");
                    LoginWindow loginWindow = Application.Current.Windows.OfType<LoginWindow>().First();
                    AdministratorWindow administratorWindow = new AdministratorWindow();
                    administratorWindow.Show();
                    loginWindow.Close();
                    return;
                }
            }
            
            MessageBox.Show("Invalid username or password.");
        }
        private bool CanLogin(object parameter)
        {
            return !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password);
        }

        #endregion

    }
}
