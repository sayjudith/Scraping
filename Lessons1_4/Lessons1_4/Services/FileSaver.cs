using Lessons1_4.Models;
namespace Lessons1_4.Services
{
    internal class FileSaver
    {
        public string? FolderName { get; set; }

        public void SaveFiles(List<NewsModel> anotherNews)
        {
            InitFolder();
            SaveNewsContent(anotherNews);
        }

        public void InitFolder()
        {
            bool wasFolderCreated = CreateFolder();
            if (wasFolderCreated == false)
            {
                CleanFolder();
            }
        }

        private bool CreateFolder()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var folderPath = Path.Combine(currentDirectory, FolderName);
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
            var currentDirectory = Directory.GetCurrentDirectory();
            var folderPath = Path.Combine(currentDirectory, FolderName);
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

        private void SaveNewsContent(List<NewsModel> AllNews)
        {
            var currentDirectory = Directory.GetCurrentDirectory();
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
                var filePath = Path.Combine(currentDirectory, FolderName, fileName);
                File.WriteAllText(filePath, fullContentToSave);
                Console.WriteLine($"Файл {fileName} сохранен в текущей папке");
            }
        }
    }
}
