using Lessons1_4.Services;

class Program
{
    public static void Main(string[] args)
    {
        SiteParser spLocal = new SiteParser("https://ria.ru/world/");
        spLocal.GetNewsAsync().Wait();
        spLocal.PrintListNewsStruct();
        FileSaver fsLocal = new FileSaver();
        fsLocal.FolderName = "News";
        fsLocal.SaveFiles(spLocal.ReadNewsFromSite());
    }
}
