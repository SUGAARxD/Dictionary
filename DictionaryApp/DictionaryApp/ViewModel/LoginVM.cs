using DictionaryApp.Model;
using DictionaryApp.View;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.IO;
using System;
using System.Reflection;

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
        private static readonly string _folderPath = @"..\..\resources\Database\users.json";
        private readonly List<UserModel> _users;
        public LoginVM()
        {
            _user = new UserModel();

            _users = new List<UserModel>();
            if (File.Exists(_folderPath))
            {
                string jsonString = File.ReadAllText(_folderPath);
                _users = JsonConvert.DeserializeObject<List<UserModel>>(jsonString);
            }
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
        private void ExecuteLogin(object parameter)
        {
            foreach(UserModel user in _users)
            {
                if (Username.Equals(user.Username) && Password.Equals(user.Password))
                {
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
    }
}
