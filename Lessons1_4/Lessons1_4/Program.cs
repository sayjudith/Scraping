using System;
using System.Threading.Tasks;
using Microsoft.Playwright;
using static System.Net.Mime.MediaTypeNames;

class Program
{
    private const string Selector = ".list .list-item";

    public static async Task Main(string[] args)
    {
        // Запуск браузера
        Console.WriteLine("браузер открывается\n");
        IPlaywright playwright = await Playwright.CreateAsync();
        IBrowser browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
        IPage page = await browser.NewPageAsync();

        // Переход на сайт
        //Console.WriteLine("переход на сайт!");
        await page.GotoAsync("https://ria.ru/world/");

        //Create locator for elements
        var newsLocator = page.Locator(".list .list-item");

        //const string selector = ".list .list-item";
        // extract text from title
        var newsCount = await newsLocator.CountAsync();
        for (int i = 0; i < newsCount; i++)
        {
            var headerText = await newsLocator.Nth(i).Locator(".list-item__content").TextContentAsync();
            var newsItemInfos = newsLocator.Nth(i).Locator(".list-item__info-item");
            var newsPreviewDate = await newsItemInfos.Nth(0).TextContentAsync();
            var newsPreviewCount = await newsItemInfos.Nth(1).TextContentAsync();
            var newsTags = await newsLocator.Nth(i).Locator(".list-tag__text").AllInnerTextsAsync();
            var tagsString = newsTags.Any() ? string.Join(", ", newsTags) : "Нет тегов";
            Console.WriteLine($"NewsTitle_{i}: {headerText}, \npreviews: {newsPreviewCount}, date: {newsPreviewDate}, \ntag: [{tagsString}]");
        }


        //Console.WriteLine($":{}");

        // Закрытие браузера
        await browser.CloseAsync();
        Console.WriteLine("браузер закрыт\n");
    }
}