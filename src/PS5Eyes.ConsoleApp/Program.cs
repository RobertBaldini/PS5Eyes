using System;
using System.Threading.Tasks;
using PS5Eyes.VendorBrowser;
using PS5Eyes.VendorBrowser.Amazon;
using PS5Eyes.VendorBrowser.BestBuy;
using PS5Eyes.VendorBrowser.GameStop;
using PS5Eyes.VendorBrowser.Target;
using PS5Eyes.VendorBrowser.Walmart;

namespace PS5Eyes.ConsoleApp {

    class Program {

        static async Task Main(string[] args) {
            using var browser = await BrowserService.LaunchBrowser();
            while (true) {
                try {
                    Console.WriteLine($"Start: {DateTime.Now}");
                    await CheckEachVendor(browser);
                    Console.WriteLine($"End: {DateTime.Now}");
                }
                catch (Exception ex) {
                    Console.WriteLine(ex.ToString());
                }
                finally {
                    await Task.Delay(TimeSpan.FromMinutes(2));
                }
            }
        }

        static async Task CheckEachVendor(BrowserService browser) {
            await new BestBuyService(browser).CheckEachProduct();
            await new GameStopService(browser).IsConsoleInStock();
            await new AmazonService(browser).IsConsoleInStock();
            //await new WalmartService(browser).IsConsoleInStock();
            await new TargetService(browser).IsConsoleInStock();
        }
    }
}
