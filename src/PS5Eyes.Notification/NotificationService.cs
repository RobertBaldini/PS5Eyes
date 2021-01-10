using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PS5Eyes.Notification {
    
    public class NotificationService {

        public async Task SendPageUrl(string pageUrl) {
            Console.WriteLine("Sending notification");
            var json = JsonConvert.SerializeObject(new {
                pageUrl = pageUrl
            });
            var notificationEndpoint = "https://prod-48.westus.logic.azure.com:443/workflows/cded2b49626f476fbe70d43f2232b032/triggers/manual/paths/invoke?api-version=2016-06-01&sp=%2Ftriggers%2Fmanual%2Frun&sv=1.0&sig=nzagh1A-RENLmVeSSE2JSgzWBtOwiLEVzVdBUeG7xrY";
            using var httpClient = new HttpClient();
            var response = await httpClient.PostAsync(notificationEndpoint, new StringContent(json, Encoding.UTF8, "application/json"));
            // notification complete, sleep for an hour to stop from spamming
            await Task.Delay(TimeSpan.FromMinutes(60));
        }
    }
}
