using Lessons1_4.Services;

class Program
{
    public static void Main(string[] args)
    {
        var news = new SiteParser("https://ria.ru/world/");
        news.GetNewsAsync().Wait();
        news.PrintListNewsStruct();
        var file = new FileSaver();
        file.FolderName = "News";
        file.SaveFiles(news.ReadNewsFromSite());
    }
}
