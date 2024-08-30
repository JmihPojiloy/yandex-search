using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Diagnostics;
using YandexSearchApp.Models;

namespace YandexSearchApp.Services
{
    public class SearchService
    {
        private readonly IConfiguration _configuration;

        public SearchService(IConfiguration configuration) =>
            _configuration = configuration;

        public async Task<List<SearchResult>> PerformSearch(SearchRequest request)
        {
            var results = new List<SearchResult>();
            var options = new ChromeOptions();
            options.AddArgument("--headless");


            using (var driver = new ChromeDriver(options))
            {

                await driver.Navigate().GoToUrlAsync($"{_configuration["YandexSearchUrl"]}?text={Uri.EscapeDataString(request.SearchKeyword!)}");

                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

                int currentIndex = request.StartIndex;
                int remainingResult = request.PageSize;

                while (remainingResult > 0)
                {
                    wait.Until(d => d.FindElement(By.TagName("body")));

                    var listItems = driver.FindElements(By.TagName("li"));

                    if (listItems == null)
                    {
                        results.Add(new SearchResult
                        {
                            RequestedUrl = "no url",
                            PageTitle = "Capcha found!",
                            TimeTaken = 0,
                            SearchPatternCount = 0
                        });
                        break;
                    };

                    for (int i = currentIndex; i < listItems.Count; i++)
                    {
                        try
                        {
                            var item = listItems[i];
                            var linkElement = item.FindElement(
                                By.TagName("a"));
                            var url = linkElement.GetAttribute("href");

                            var stopwatch = Stopwatch.StartNew();
                            driver.Navigate().GoToUrl(url);

                            var title = driver.Title;
                            var pageSource = driver.PageSource;
                            int patternCount = CountOccurances(pageSource, request.SearchPattern!);
                            stopwatch.Stop();

                            results.Add(new SearchResult
                            {
                                RequestedUrl = url,
                                PageTitle = title,
                                TimeTaken = stopwatch.Elapsed.TotalSeconds,
                                SearchPatternCount = patternCount
                            });
                        }
                        catch (Exception)
                        {
                            driver.Navigate().Back();
                            continue;
                        }

                        driver.Navigate().Back();
                    }
                    if (remainingResult > 0)
                    {
                        try
                        {
                            var nextPageButton = wait.Until(d =>
                            {
                                var elements = d.FindElements(By.XPath("//div[contains(@class, 'Pager-ListItem_type_next')]//a[contains(@class, 'Pager-Item_type_next')]"));

                                return elements.Count > 0 ? elements[0] : null;
                            });

                            if (nextPageButton != null)
                            {
                                var nextPageUrl = nextPageButton.GetAttribute("href");

                                if (!string.IsNullOrEmpty(nextPageUrl))
                                {
                                    driver.Navigate().GoToUrl(nextPageUrl);
                                    remainingResult--;
                                    currentIndex = 0;
                                }
                                else
                                {
                                    break; // Нет кнопки следующей страницы - выход
                                }
                            }
                            else
                            {
                                break; // Кнопка следующей страницы не найдена - выход
                            }
                        }
                        catch (Exception)
                        {
                            break; // Время ожидания истекло - выход
                        }
                    }
                }
            }
            return results;
        }

        private int CountOccurances(string text, string pattern)
        {
            if (string.IsNullOrEmpty(pattern)) return 0;
            return (text.Length - text.Replace(pattern, "").Length) / pattern.Length;
        }
    }
}
