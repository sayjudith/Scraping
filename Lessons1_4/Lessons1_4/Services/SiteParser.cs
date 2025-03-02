using Lessons1_4.Models;
using Microsoft.Playwright;


namespace Lessons1_4.Services
{
    internal class SiteParser
    {
        private string _url;

        private List<NewsModel> _news;
        private List<NewsTitleModel> _newsTitles;

        public SiteParser(string externalUrl)
        {
            _url = externalUrl;
            _news = new List<NewsModel>();
            _newsTitles = new List<NewsTitleModel>();
        }

        public List<NewsModel> ReadNewsFromSite()
        {
            return _news;
        }

        public async Task GetNewsAsync()
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
                return;
            }
            else
            {
                await GetListOfNewsAsync(page, newsLocator, newsCount);
            }
            await browser.CloseAsync();
            Console.WriteLine("браузер закрыт\n");
        }

        private async Task GetListOfNewsAsync(IPage pageHref, ILocator newsLocator, int newsCount) 
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
                        _newsTitles.Add(
                            new NewsTitleModel() {
                                FileName = $"[{i + 1}] {fileName}.txt",
                                Title = headerText,
                                PublishDate = newsPreviewDate,
                                ViewsCount = newsPreviewCount,
                                TagList = tagsString,
                                LinkReference = linkReference
                            }
                        );
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("error" + ex.Message + i.ToString());
                    _newsTitles.Add(
                        new NewsTitleModel() {
                            FileName = $"[{i + 1}] {headerText}.txt",
                            Title = headerText,
                            PublishDate = string.Empty,
                            ViewsCount = string.Empty,
                            TagList = string.Empty,
                            LinkReference = string.Empty
                        }
                    );
                }
            }
            await GetCurrentNewsValuesAsync(newsCount, _newsTitles, pageHref);
        }

        private async Task GetCurrentNewsValuesAsync(int countOfNews, List<NewsTitleModel> newsTitleList, IPage pageInnerHref)
        {
            if (countOfNews > 0) {
                for (int i = 0; i < countOfNews; i++)
                {
                    var currLinkReference = newsTitleList[i].LinkReference;
                    if (!string.IsNullOrEmpty(currLinkReference))
                    {
                        await pageInnerHref.GotoAsync(currLinkReference);
                        await pageInnerHref.WaitForSelectorAsync(".article__header");
                        var newsMainTitle = pageInnerHref.Locator(".article__title");
                        var newsSubTitle = pageInnerHref.Locator(".article__second-title");
                        var newsTextsLocator = pageInnerHref.Locator(".article__block");
                        int textsBlocksCount = await newsTextsLocator.CountAsync();

                        string fullNewsContent = "";
                        if (textsBlocksCount > 0)
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
                            _news.Add(new NewsModel()
                            {
                                titleModel = _newsTitles[i],
                                FullContent = fullNewsContent
                            });
                        }
                        else
                        {
                            Console.WriteLine($"file [{i + 1}] cannot be formed");
                        }
                    }
                    else
                    {
                        _news.Add(new NewsModel()
                        {
                            titleModel = _newsTitles[i],
                            FullContent = string.Empty
                        });
                    }
                }
            }
            else
            {
                Console.WriteLine("Wrong list of news");
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