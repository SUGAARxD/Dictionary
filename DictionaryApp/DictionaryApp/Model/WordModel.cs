using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace DictionaryApp.Model
{
    internal class WordModel
    {
        public static string DefaultImagePath = "./resources/Images/";

        public WordModel()
        {
            _text = string.Empty;
            _description = string.Empty;
            _category = string.Empty;
            _imagePath = DefaultImagePath;
        }
        public WordModel(string text, string description, string category, string imagePath)
        {
            _text = text;
            _description = description;
            _category = category;
            _imagePath = imagePath;
        }

        private string _text;
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
            }
        }
        private string _description;
        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
            }
        }
        private string _imagePath;
        public string ImagePath
        {
            get
            {
                return _imagePath;
            }
            set
            {
                _imagePath = value;
            }
        }
        private string _category;
        public string Category
        {
            get
            {
                return _category;
            }
            set
            {
                _category = value;
            }
        }

    }
}
