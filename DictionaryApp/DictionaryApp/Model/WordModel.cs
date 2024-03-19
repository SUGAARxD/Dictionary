namespace DictionaryApp.Model
{
    internal class WordModel
    {

        public WordModel()
        {
            Text = string.Empty;
            Description = string.Empty;
            Category = string.Empty;
            ImagePath = DefaultImagePath;
        }
        public WordModel(string text, string description, string category, string imagePath)
        {
            Text = text;
            Description = description;
            Category = category;
            ImagePath = imagePath;
        }

        #region Properties and members
        
        public static string DefaultImagePath = @"..\..\resources\Images\default_image.jpg";

        public string Text { get; set; }
        public string Description { get; set; }
        public string ImagePath { get; set; }
        public string Category { get; set; }

        #endregion

    }
}
