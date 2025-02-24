using Lessons1_4.Models;
using Microsoft.Playwright;


namespace Lessons1_4.Services
{
    internal class SiteParser
    {
        private string _url;

        public List<NewsModel> News;

        public SiteParser(string externalUrl)
        {
            _url = externalUrl;
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
            await page.GotoAsync(_url);
            var newsLocator = page.Locator(".list .list-item");
            var newsCount = await newsLocator.CountAsync();
            if (newsCount == 0)
            {
                Console.WriteLine("No news found");
                return;
            }
            else
            {
                await GetListOfNewsAsync(pageHref, newsLocator, newsCount);
            }
            await browser.CloseAsync();
            Console.WriteLine("браузер закрыт\n");
        }

        private async Task GetListOfNewsAsync(IPage pageHref, ILocator newsLocator, int newsCount) 
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
                    await GetCurrentNewsValuesAsync(i, linkReference, headerText, newsPreviewDate, newsPreviewCount, tagsString, pageHref);
                }
            }
        }

        private async Task GetCurrentNewsValuesAsync(int currId, string linkReference, string headerText, string newsPreviewDate, string newsPreviewCount, string tagsString, IPage pageInnerHref)
        {
            await pageInnerHref.GotoAsync(linkReference);
            await pageInnerHref.WaitForSelectorAsync(".article__header");
            var newsMainTitle = pageInnerHref.Locator(".article__title");
            var newsSubTitle = pageInnerHref.Locator(".article__second-title");
            var newsTextsLocator = pageInnerHref.Locator(".article__block");
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
                    FileName = $"[{currId + 1}] {fileName}.txt",
                    Title = headerText,
                    PublishDate = newsPreviewDate,
                    ViewsCount = newsPreviewCount,
                    TagList = tagsString,
                    FullContent = fullNewsContent
                });
            }
            else
            {
                Console.WriteLine($"file [{currId + 1}] cannot be formed");
            }
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

        public void PrintListNewsModel()
        {
            foreach (NewsModel anotherNews in News)
            {
                Console.WriteLine(anotherNews);
            }
        }
    }
}