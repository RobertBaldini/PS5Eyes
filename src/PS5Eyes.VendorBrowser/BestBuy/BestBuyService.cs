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

namespace PS5Eyes.VendorBrowser.BestBuy {
    
    public class BestBuyService {

        private readonly BrowserService _browser;

        public BestBuyService(BrowserService browser) {
            _browser = browser;
        }

        public async Task CheckEachProduct() {
            await IsSpidermanBundleInStock();
            await IsConsoleInStock();
        }

        private async Task<bool> IsConsoleInStock() {
            Console.WriteLine("Checking Best Buy PS5 Disc Console");
            var pageUrl = "https://www.bestbuy.com/site/sony-playstation-5-console/6426149.p?skuId=6426149";
            return await CheckPage(pageUrl);
        }

        private async Task<bool> IsSpidermanBundleInStock() {
            Console.WriteLine("Checking Best Buy Spiderman Bundle");
            var pageUrl = "https://www.bestbuy.com/site/combo/ps5-consoles/96be4c49-d98e-47c6-9a68-291c646d0e47";
            return await CheckPage(pageUrl);
        }

        private async Task<bool> CheckPage(string pageUrl) {
            await _browser.GoToPage(pageUrl);
            var html = await _browser.GetHtml();
            var htmlDom = new HtmlParser().ParseDocument(html);
            var buttons = htmlDom.QuerySelectorAll<IHtmlButtonElement>("button").ToList();
            buttons = buttons
                .Where(e => e.ClassList.Contains("add-to-cart-button"))
                .Where(e => e.GetAttribute("data-sku-id") == "6426149") // all these bundles have the same sku id on the button
                .ToList();
            var wasButtonFound = (buttons.Count() == 1);
            if (!wasButtonFound) {
                Console.WriteLine($"ERROR: Button not found");
                return false;
            }
            var button = buttons.Single();
            var buttonText = button.TextContent;
            Console.WriteLine(buttonText);
            var isInStock = !(buttonText.Contains("Sold Out"));
            if (isInStock)
                await new NotificationService().SendPageUrl(pageUrl);
            return isInStock;
        }
    }
}
