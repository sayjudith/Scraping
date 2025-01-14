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
        bool wasFolderCreated = CreateFolder(folderName);
        if (wasFolderCreated == true)
        {
           CleanFolder(folderName);
        }
        GetNews(folderName).Wait();
    }

    private static bool CreateFolder(string newFolderName) {
        string currentDirectory = Directory.GetCurrentDirectory();
        string folderPath = Path.Combine(currentDirectory, newFolderName);
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            Console.WriteLine($"Папка создана в текущем каталоге: {folderPath}");
            return false;
        }
        else
        {
            Console.WriteLine($"Папка уже существует: {folderPath}");
            return true;
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
        var playwright = await Playwright.CreateAsync();
        var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
        var page = await browser.NewPageAsync();
        var pageHref = await browser.NewPageAsync();
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
                    await pageHref.WaitForSelectorAsync(".article__header");
                    var newsMainTitle =  pageHref.Locator(".article__title");
                    var newsSubTitle = pageHref.Locator(".article__second-title");
                    var newsTextsLocator = pageHref.Locator(".article__block");
                    int textsBlocksCount = await newsTextsLocator.CountAsync();

                    if (textsBlocksCount > 0)
                    {
                        string fullNewsContent = "";
                        for (int j = 0; j < textsBlocksCount; j++)
                        {
                            var ttmp = await newsTextsLocator.Nth(j).GetAttributeAsync("data-type");
                            if (ttmp == "text")
                            {
                                var tmp = await newsTextsLocator.Nth(j).TextContentAsync();
                                fullNewsContent += "\n" + await newsTextsLocator.Nth(j).TextContentAsync();
                            }
                        }

                        var fullNewsForSave = headerText + "\ndate: " + newsPreviewDate + "\npreviews: " + newsPreviewCount + "\n\ntag: " + tagsString + "\n\n" + fullNewsContent;
                        var nameForFile = FormatFileName(headerText);
                        SaveNewsContent(storageFolder, $"[{i + 1}] {nameForFile}.txt", fullNewsForSave);
                        Console.WriteLine($"finished [{i + 1}] file");
                    }
                    else {
                        Console.WriteLine($"file [{i + 1}] cannot be formed");
                    }
                }
                Console.WriteLine($"NewsTitle_{i + 1 }: {headerText}, \npreviews: {newsPreviewCount}, date: {newsPreviewDate}, \ntag: [{tagsString}], \nlink: [{linkReference}]\n");
            }
        }
        await browser.CloseAsync();
        Console.WriteLine("браузер закрыт\n");
    }

    private static void SaveNewsContent(string folderName, string fileName, string content) 
    {
        string currentDirectory = Directory.GetCurrentDirectory();
        string filePath = Path.Combine(currentDirectory, folderName, fileName);
        File.WriteAllText(filePath, content);
        Console.WriteLine($"Файл {fileName} сохранен в текущей папке");
    }

    private static string FormatFileName(string? fileName)
    {
        if (fileName == null)
        {
            return "";
        }
        else
        {
            return fileName.Replace("\"", "").Replace("\'", "").Replace(":", "").Replace("*", "");
        }
    }
}