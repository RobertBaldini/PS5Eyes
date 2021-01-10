using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using PS5Eyes.Notification;

namespace PS5Eyes.VendorBrowser.Amazon {
    
    public class AmazonService {

        private readonly BrowserService _browser;

        public AmazonService(BrowserService browser) {
            _browser = browser;
        }

        public async Task<bool> IsConsoleInStock() {
            Console.WriteLine("Checking Amazon PS5 Disc Console");
            var pageUrl = "https://www.amazon.com/gp/product/B08FC5L3RG";
            await _browser.GoToPage(pageUrl);
            var html = await _browser.GetHtml();
            var outOfStockMessage = "We don't know when or if this item will be back in stock.";
            var hasMessage = html.Contains(outOfStockMessage);
            if (hasMessage) {
                Console.WriteLine(outOfStockMessage);
                return false;
            }
            var inputElements = new HtmlParser().ParseDocument(html).QuerySelectorAll<IHtmlInputElement>("input").ToList();
            var addToCartButton = inputElements.Where(e => e.Id == "add-to-cart-button" && e.Title == "Add to Shopping Cart");
            var isInStock = (!hasMessage && addToCartButton.Count() == 1);
            if (isInStock)
                await new NotificationService().SendPageUrl(pageUrl);
            return isInStock;
        }
    }
}
