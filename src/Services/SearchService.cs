using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using System.Diagnostics;
using System.Runtime.InteropServices;
using YandexSearchApp.Models;


namespace YandexSearchApp.Services
{
    public class SearchService
    {
        private readonly IConfiguration _configuration;

        public SearchService(IConfiguration configuration) =>
            _configuration = configuration;

        public List<SearchResult> PerformSearch(SearchRequest request)
        {
            var results = new List<SearchResult>();

            var options = new ChromeOptions();

            options.AddArgument("--disable-blink-features=AutomationControlled");
            options.AddArgument("user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/85.0.4183.102 Safari/537.36");
            options.AddArgument("--headless");
            options.AddArgument("--ignore-certificate-errors");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");
            options.AddArgument("--disable-gpu");
            options.AddArgument("--window-size=1920,1080");
            options.AddArgument("--disable-software-rasterizer");
            options.AddArgument("--disable-extensions");
            options.AddExcludedArgument("enable-automation");

            var service = ChromeDriverService.CreateDefaultService(@"C:\chromedriver", "chromedriver.exe");


            var seleniumHubUrl = _configuration["SELENIUM_HUB_URL"]!;

            using (var driver = new RemoteWebDriver(new Uri(seleniumHubUrl), options))
            {
                driver.Manage().Timeouts().PageLoad = TimeSpan.FromMinutes(2);
                driver.Manage().Timeouts().AsynchronousJavaScript = TimeSpan.FromSeconds(60);

                Actions actions = new Actions(driver);
                actions.MoveByOffset(10, 10).Perform();

                driver.Navigate().GoToUrl($"{_configuration["YandexSearchUrl"]}?text={Uri.EscapeDataString(request.SearchKeyword!)}");

                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

                driver.Manage().Cookies.AddCookie(new Cookie("cookie_name", "cookie_value"));

                int currentIndex = request.StartIndex;
                int remainingResult = request.PageSize;

                while (remainingResult > 0)
                {
                    wait.Until(d => d.FindElement(By.TagName("body")));

                    var listItems = driver.FindElements(By.TagName("li"));

                    if (listItems == null || listItems!.Count <= 0)
                    {
                        results.Add(new SearchResult
                        {
                            RequestedUrl = _configuration["YandexSearchUrl"],
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
