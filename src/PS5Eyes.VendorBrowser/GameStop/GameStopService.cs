using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using AngleSharp.Io;
using PS5Eyes.Notification;

namespace PS5Eyes.VendorBrowser.GameStop {
    
    public class GameStopService {

        private readonly BrowserService _browser;

        public GameStopService(BrowserService browser) {
            _browser = browser;
        }

        public async Task<bool> IsConsoleInStock() {
            Console.WriteLine("Checking GameStop PS5 Disc Console");
            var pageUrl = "https://www.gamestop.com/video-games/playstation-5/consoles/products/playstation-5/11108140.html";
            return await CheckPage(pageUrl);
        }

        private async Task<bool> CheckPage(string pageUrl) {
            await _browser.GoToPage(pageUrl);
            var html = await _browser.GetHtml();
            var htmlDom = new HtmlParser().ParseDocument(html);
            var buttons = htmlDom.QuerySelectorAll<IHtmlButtonElement>("button").ToList();
            buttons = buttons
                .Where(e => e.ClassList.Contains("add-to-cart"))
                .Where(e => e.GetAttribute("data-pid") == "11108140")
                .ToList();
            var wasButtonFound = (buttons.Count() == 2);
            if (!wasButtonFound) {
                Console.WriteLine($"ERROR: Button not found");
                return false;
            }
            var button = buttons.First();
            var buttonText = button.TextContent;
            Console.WriteLine(buttonText);
            var isInStock = !(buttonText.ToUpper().Contains("NOT AVAILABLE"));
            if (isInStock)
                await new NotificationService().SendPageUrl(pageUrl);
            return isInStock;
        }
    }
}
