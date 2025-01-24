
namespace Lessons1_4.Solutions
{
    internal class FileSaver
    {
        private string? _folderName;

        public string? FolderName { get { return _folderName; } set { _folderName = value; } }

        public void mainJob(List<NewsStruct> aNewsStruct)
        {
            FolderJob();
            SaveNewsContent(aNewsStruct);
        }

        public void FolderJob()
        {
            bool wasFolderCreated = CreateFolder();
            if (wasFolderCreated == false)
            {
                CleanFolder();
            }

        }

        private bool CreateFolder()
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string folderPath = Path.Combine(currentDirectory, this.FolderName);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
                Console.WriteLine($"Папка создана в текущем каталоге: {folderPath}");
                return true;
            }
            else
            {
                Console.WriteLine($"Папка уже существует: {folderPath}");
                return false;
            }
        }

        private void CleanFolder()
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string folderPath = Path.Combine(currentDirectory, this.FolderName);
            var i = 0;
            DirectoryInfo di = new DirectoryInfo(folderPath);
            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
                i++;
            }
            if (i > 0)
            {
                Console.WriteLine($"{i} файлов в папке {FolderName} удалено");
            }
        }

        private void SaveNewsContent(List<NewsStruct> AllNews)
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            var filesCount = AllNews?.Count;
            Console.WriteLine(filesCount);
            for (int i = 0; i < filesCount; i++)
            {
                var fileName = AllNews[i].FileName;
                var fullContentToSave = AllNews[i].Title + 
                    "\ndate: " + AllNews[i].PublishDate + 
                    "\npreviews: " + AllNews[i].ViewsCount + 
                    "\n\ntags: " + AllNews[i].TagList + "\n\n" +
                    AllNews[i].FullContent;
                string filePath = Path.Combine(currentDirectory, FolderName, fileName);
                File.WriteAllText(filePath, fullContentToSave);
                Console.WriteLine($"Файл {fileName} сохранен в текущей папке");
            }
        }
    }
}
