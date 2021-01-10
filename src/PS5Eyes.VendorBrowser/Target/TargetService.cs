using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using PS5Eyes.Notification;

namespace PS5Eyes.VendorBrowser.Target {
    
    public class TargetService {

        private readonly BrowserService _browser;

        public TargetService(BrowserService browser) {
            _browser = browser;
        }

        public async Task<bool> IsConsoleInStock() {
            Console.WriteLine("Checking Target PS5 Disc Console");
            var pageUrl = "https://www.target.com/p/playstation-5-console/-/A-81114595";
            await _browser.GoToPage(pageUrl);
            var html = await _browser.GetHtml();
            var outOfStockMessage = "Out of stock in stores near you";
            var hasMessage = html.Contains(outOfStockMessage);
            if (hasMessage) {
                Console.WriteLine(outOfStockMessage);
                return false;
            }
            var buttonElements = new HtmlParser().ParseDocument(html).QuerySelectorAll<IHtmlButtonElement>("button").ToList();
            var pickupButton = buttonElements.Where(e => e.TextContent.Trim() == "Pick it up" && e.GetAttribute("data-test") == "orderPickupButton");
            var isInStock = (!hasMessage && pickupButton.Count() == 1);
            if (isInStock)
                await new NotificationService().SendPageUrl(pageUrl);
            return isInStock;
        }
    }
}
