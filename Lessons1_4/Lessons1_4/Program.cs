using Lessons1_4.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

class Program
{
    public static async Task Main(string[] args)
    {
        var news = new SiteParser("https://ria.ru/world/");
        //news.GetNewsAsync().Wait();
        //news.PrintListNewsModel();
        var file = new FileSaver();
        file.FolderName = "News";
        file.SaveFiles(await news.GetNewsAsync());
    }
}
