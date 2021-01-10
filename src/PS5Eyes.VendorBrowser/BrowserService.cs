using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PuppeteerSharp;

namespace PS5Eyes.VendorBrowser {
    
    public class BrowserService : IDisposable {

        private readonly PuppeteerSharp.Browser _browser;
        private readonly PuppeteerSharp.Page _currentPage;

        private BrowserService(Browser browser, Page currentPage) {
            _browser = browser;
            _currentPage = currentPage;
        }

        public static async Task<BrowserService> LaunchBrowser() {
            await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);
            var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = false });
            var initialPage = (await browser.PagesAsync()).First();
            return new BrowserService(browser, initialPage);
        }

        public async Task GoToPage(string url) {
            await _currentPage.GoToAsync(url, WaitUntilNavigation.Networkidle0);
            if ((await GetHtml()).Contains("Verify your identity"))
                Console.WriteLine("ERROR: reCAPTCHA");
        }

        public async Task<string> GetHtml()
            => await _currentPage.GetContentAsync();

        public void Dispose() 
            => _browser?.Dispose();
    }
}
