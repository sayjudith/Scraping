using Lessons1_4.Models;
using Microsoft.Playwright;


namespace Lessons1_4.Services
{
    internal class SiteParser
    {
        public string url;
        public string URL { get; set; }

        public List<NewsModel> News;

        public SiteParser(string ExternalURL)
        {
            url = ExternalURL;
            News = new List<NewsModel>();
        }

        public List<NewsModel> ReadNewsFromSite()
        {
            return News;
        }

        public async Task GetNewsAsync()
        {
            Console.WriteLine("браузер открывается\n");
            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
            var page = await browser.NewPageAsync();
            var pageHref = await browser.NewPageAsync();
            await page.GotoAsync(url);
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
                        var newsMainTitle = pageHref.Locator(".article__title");
                        var newsSubTitle = pageHref.Locator(".article__second-title");
                        var newsTextsLocator = pageHref.Locator(".article__block");
                        int textsBlocksCount = await newsTextsLocator.CountAsync();

                        if (textsBlocksCount > 0)
                        {
                            string fullNewsContent = "";
                            for (int j = 0; j < textsBlocksCount; j++)
                            {
                                var articleRowId = await newsTextsLocator.Nth(j).GetAttributeAsync("data-type");
                                if (articleRowId == "text")
                                {
                                    fullNewsContent += "\n" + await newsTextsLocator.Nth(j).TextContentAsync();
                                }
                            }

                            var fullNewsForSave = headerText + "\ndate: " + newsPreviewDate + "\npreviews: " + newsPreviewCount + "\n\ntag: " + tagsString + "\n\n" + fullNewsContent;
                            var fileName = FormatFileName(headerText);
                            News.Add(new NewsModel() 
                            {
                                FileName = $"[{i + 1}] {fileName}.txt",
                                Title = headerText,
                                PublishDate = newsPreviewDate,
                                ViewsCount = newsPreviewCount,
                                TagList = tagsString,
                                FullContent = fullNewsContent
                            });
                        }
                        else
                        {
                            Console.WriteLine($"file [{i + 1}] cannot be formed");
                        }
                    }
                }
            }
            await browser.CloseAsync();
            Console.WriteLine("браузер закрыт\n");
        }

        public string FormatFileName(string? fileName)
        {
            if (fileName == null)
            {
                return "";
            }
            else
            {
                return fileName.Replace("\"", "").Replace("\'", "").Replace(":", "").Replace("*", "").Trim();
            }
        }

        public void PrintListNewsStruct()
        {
            foreach (NewsModel anotherNews in News)
            {
                Console.WriteLine(anotherNews);
            }
        }
    }
}