using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using PS5Eyes.Notification;

namespace PS5Eyes.VendorBrowser.Walmart {

    // disabled, recaptcha
    public class WalmartService {

        private readonly BrowserService _browser;

        public WalmartService(BrowserService browser) {
            _browser = browser;
        }

        public async Task<bool> IsConsoleInStock() {
            Console.WriteLine("Checking Walmart PS5 Disc Console");
            var pageUrl = "https://www.walmart.com/ip/PlayStation5-Console/363472942";
            await _browser.GoToPage(pageUrl);
            var html = await _browser.GetHtml();
            var outOfStockMessage = "Oops! This item is unavailable or on backorder.";
            var hasMessage = html.Contains(outOfStockMessage);
            if (hasMessage) {
                Console.WriteLine(outOfStockMessage);
                return false;
            }
            var spanElements = new HtmlParser().ParseDocument(html).QuerySelectorAll<IHtmlSpanElement>("span").ToList();
            var addToCartButton = spanElements.Where(e => e.TextContent.Trim() == "Add to cart" && e.ClassList.Contains("spin-button-children"));
            var isInStock = (!hasMessage && addToCartButton.Count() == 1);
            if (isInStock)
                await new NotificationService().SendPageUrl(pageUrl);
            return isInStock;
        }
    }
}
