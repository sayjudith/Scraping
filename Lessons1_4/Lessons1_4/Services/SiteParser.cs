using Lessons1_4.Models;
using Microsoft.Playwright;


namespace Lessons1_4.Services
{
    internal class SiteParser
    {
        private string _url;

        private List<NewsModel> _news;

        public SiteParser(string externalUrl)
        {
            _url = externalUrl;
            _news = new List<NewsModel>();
        }

        public async Task<List<NewsModel>> GetNewsAsync()
        {
            Console.WriteLine("браузер открывается\n");
            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions 
            { 
                Headless = true
            });
            var page = await browser.NewPageAsync();
            await page.GotoAsync(_url);
            var newsLocator = page.Locator(".list .list-item");
            var newsCount = await newsLocator.CountAsync();
            if (newsCount == 0)
            {
                Console.WriteLine("No news found");
                return new List<NewsModel>();
            }
            else
            {
                await GetListOfNewsAsync(page, newsLocator, newsCount);
            }
            await browser.CloseAsync();
            Console.WriteLine("браузер закрыт\n");
            return _news;
        }

        private async Task GetListOfNewsAsync(IPage page, ILocator newsLocator, int newsCount) 
        {
            var headerText = "";
            for (int i = 0; i < newsCount; i++)
            {
                headerText = "";
                try
                {
                    headerText = await newsLocator.Nth(i).Locator(".list-item__content").TextContentAsync();
                    var newsItemInfos = newsLocator.Nth(i).Locator(".list-item__info-item");
                    var newsPreviewDate = await newsItemInfos.Nth(0).TextContentAsync();
                    var newsPreviewCount = await newsItemInfos.Nth(1).TextContentAsync();
                    var newsTags = await newsLocator.Nth(i).Locator(".list-tag__text").AllInnerTextsAsync();
                    var tagsString = newsTags.Any() ? string.Join(", ", newsTags) : "Нет тегов";
                    var linkLocation = newsLocator.Nth(i).Locator(".list-item__title");
                    var linkReference = await linkLocation.GetAttributeAsync("href");
                    if (linkReference != null)
                    {
                        var fileName = FormatFileName(headerText);
                        _news.Add(
                            new NewsModel 
                            {
                                FileName = $"[{i + 1}] {fileName}.txt",
                                Title = headerText,
                                PublishDate = newsPreviewDate,
                                ViewsCount = newsPreviewCount,
                                TagList = tagsString,
                                LinkReference = linkReference,
                                FullContent = string.Empty
                            }
                        );
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("error" + ex.Message + i.ToString());
                    _news.Add(
                        new NewsModel 
                        {
                            FileName = $"[{i + 1}] {headerText}.txt",
                            Title = headerText,
                            PublishDate = string.Empty,
                            ViewsCount = string.Empty,
                            TagList = string.Empty,
                            LinkReference = string.Empty,
                            FullContent = string.Empty
                        }
                    );
                }
            }
            await GetCurrentNewsValuesAsync(page);
        }

        private async Task GetCurrentNewsValuesAsync(IPage page)
        {
            if (_news.Count== 0) 
            {
                Console.WriteLine("Wrong list of news");
            }
            else
            {
                for (int i = 0; i < _news.Count; i++)
                {
                    var currentLinkReference = _news[i].LinkReference;
                    if (!string.IsNullOrEmpty(currentLinkReference))
                    {
                        await page.GotoAsync(currentLinkReference);
                        await page.WaitForSelectorAsync(".article__header");
                        var newsMainTitle = page.Locator(".article__title");
                        var newsSubTitle = page.Locator(".article__second-title");
                        var newsTextsLocator = page.Locator(".article__block");
                        int textsBlocksCount = await newsTextsLocator.CountAsync();

                        string fullNewsContent = "";
                        if (textsBlocksCount == 0)
                        {
                            Console.WriteLine($"file [{i + 1}] cannot be formed");
                        }
                        else
                        {
                            fullNewsContent = "";
                            for (int j = 0; j < textsBlocksCount; j++)
                            {
                                var articleRowId = await newsTextsLocator.Nth(j).GetAttributeAsync("data-type");
                                if (articleRowId == "text")
                                {
                                    fullNewsContent += "\n" + await newsTextsLocator.Nth(j).TextContentAsync();
                                }
                            }
                            _news[i].FullContent = fullNewsContent;
                        }
                    }
                    else
                    {
                        _news[i].FullContent = string.Empty;
                    }
                }
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
            foreach (var anotherNews in _news)
            {
                Console.WriteLine(anotherNews);
            }
        }
    }
}