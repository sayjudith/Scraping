using Lessons1_4.Solutions;

class Program
{

    public static void Main(string[] args)
    {
        SiteParser spLocal = new SiteParser("https://ria.ru/world/");
        spLocal.GetNews().Wait();
        spLocal.PrintListNewsStruct();
        FileSaver fsLocal = new FileSaver();
        fsLocal.FolderName = "News";
        fsLocal.mainJob(spLocal.ReadStructNewsFromSite());
    }
}
