using System;
using System.Threading.Tasks;
using Microsoft.Playwright;
using static System.Net.Mime.MediaTypeNames;
using HtmlAgilityPack;
using System.Xml.Linq;


class Program
{

    public static void Main(string[] args)
    { 
        string folderName = "News";
        var iCheck = CreateFolder(folderName);
        if (iCheck == 1)
        {
           CleanFolder(folderName);
        }
        GetNews(folderName).Wait();
    }

    private static int CreateFolder(string newFolderName) {
        string currentDirectory = Directory.GetCurrentDirectory();
        string folderPath = Path.Combine(currentDirectory, newFolderName);
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            Console.WriteLine($"Папка создана в текущем каталоге: {folderPath}");
            return 0;
        }
        else
        {
            Console.WriteLine($"Папка уже существует: {folderPath}");
            return 1;
        } 
    }

    private static void CleanFolder(string folderName)
    {
        string currentDirectory = Directory.GetCurrentDirectory();
        string folderPath = Path.Combine(currentDirectory, folderName);
        DirectoryInfo di = new DirectoryInfo(folderPath);
        foreach (FileInfo file in di.GetFiles())
        {
            file.Delete();
        }
        Console.WriteLine($"Файлы в папке {folderName} удалены");
    }

    private static async Task GetNews(string storageFolder) 
    {
        Console.WriteLine("браузер открывается\n");
        IPlaywright playwright = await Playwright.CreateAsync();
        IBrowser browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
        IPage page = await browser.NewPageAsync();
        IPage pageHref = await browser.NewPageAsync();
        await page.GotoAsync("https://ria.ru/world/");
        var newsLocator = page.Locator(".list .list-item");
        var newsCount = await newsLocator.CountAsync();
        if (newsCount == 0)
        {
            Console.WriteLine("No news found");
            return;
        }
        else
        {
            for (int i = 0; i < newsCount; i++)
            {
                var headerText = await newsLocator.Nth(i).Locator(".list-item__content").TextContentAsync();
                var newsItemInfos = newsLocator.Nth(i).Locator(".list-item__info-item");
                var newsPreviewDate = await newsItemInfos.Nth(0).TextContentAsync();
                var newsPreviewCount = await newsItemInfos.Nth(1).TextContentAsync();
                var newsTags = await newsLocator.Nth(i).Locator(".list-tag__text").AllInnerTextsAsync();
                var tagsString = newsTags.Any() ? string.Join(", ", newsTags) : "Нет тегов";
                var linkLocation = newsLocator.Nth(i).Locator(".list-item__title");
                var linkReference = await linkLocation.GetAttributeAsync("href");
                if (linkReference != null)
                {
                    await pageHref.GotoAsync(linkReference);
                    var newsTextLocator = pageHref.Locator(".article__text");
                    var blockCount = await newsTextLocator.CountAsync();
                    var fullNewsContent = "";
                    if (blockCount > 0)
                    {
                        for (int j = 0; j < blockCount; j++)
                        {
                            fullNewsContent += await newsTextLocator.Nth(j).TextContentAsync();
                        }
                        var fullNewsForSafe = headerText + newsPreviewDate +  newsPreviewCount + fullNewsContent;
                        var nameForFile = FixedTextForFileName(headerText);
                        SaveNewsContetnt(storageFolder, $"[{i + 1}]{nameForFile}.txt" , fullNewsForSafe);
                        Console.WriteLine($"finished [{i + 1}] file");
                    }
                }
                Console.WriteLine($"NewsTitle_{i}: {headerText}, \npreviews: {newsPreviewCount}, date: {newsPreviewDate}, \ntag: [{tagsString}], \nlink: [{linkReference}]");  
            }
        }
        await browser.CloseAsync();
        Console.WriteLine("браузер закрыт\n");
    }

    private static void SaveNewsContetnt(string folderName, string fileName, string content) 
    {
    string currentDirectory = Directory.GetCurrentDirectory();
    string filePath = Path.Combine(currentDirectory, folderName, fileName);
    File.WriteAllText(filePath, content);
    Console.WriteLine($"Файл {fileName} сохранен в текущей папке");
    }

    private static string FixedTextForFileName(string? fileName)
    {
        if (fileName == null)
        {
            return "";
        }
        else
        {
            return fileName.Replace("\"", "").Replace("\'", "").Replace(":", "");
        }
    }
}