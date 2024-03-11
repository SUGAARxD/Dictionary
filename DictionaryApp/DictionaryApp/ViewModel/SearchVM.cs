using System.Collections.ObjectModel;
using System.Windows.Input;

namespace DictionaryApp.ViewModel
{
    internal class SearchVM :BaseVM
    {
        private string _word;
        public string Word
        {
            get => _word;
            set
            {
                _word = value;
            }
        }
        public SearchVM()
        {
            _word = string.Empty;
            CategoryList = new ObservableCollection<string>();
        }
        private ICommand _searchCommand;
        public ICommand SearchCommand
        {
            get
            {
                if (_searchCommand == null)
                    _searchCommand = new RelayCommand(ExecuteSearch);
                return _searchCommand;
            }
        }
        private void ExecuteSearch(object parameter)
        {

        }
        public ObservableCollection<string> CategoryList { get; set; }
    }
}