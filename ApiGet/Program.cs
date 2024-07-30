
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace TronTransactionRisk
{
    class GetJsonFile 
    {
        public async Task<JObject> Get(string transactionHash)
        {
            string url = $"https://apilist.tronscan.org/api/transaction-info?hash={transactionHash}";

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    JObject transactionData = JObject.Parse(responseBody);
                    return transactionData;
                }
                else
                {
                    return new JObject();
                }
            }

        }
    }
    class TransactionRisk
    {
        readonly string transactionHash;
        public TransactionRisk(string transactionHash)
        {
            this.transactionHash = transactionHash;
        }
        private string GetTransactionRisk(JObject item)
        {
            JToken? normalAddressInfo = item["normalAddressInfo"];
            StringBuilder riskInfo = new StringBuilder();
            if (normalAddressInfo != null)
            {
                foreach (var address in normalAddressInfo.Children<JProperty>())
                {
                    var risk = address.Value["risk"];
                    riskInfo.AppendLine($"{address.Name}: risk = {risk}");
                }
            }
            return riskInfo.ToString();
        }
        public async Task ShowRisk()
        {
            GetJsonFile getJson = new GetJsonFile();
            JObject jsonFile = await getJson.Get(transactionHash);
            string riskInfo = GetTransactionRisk(jsonFile);
            Console.WriteLine(riskInfo);
        }
    }
    class Program
    {
        static async Task Main(string[] args)
        {
            string hash = "853793d552635f533aa982b92b35b00e63a1c1add062c099da2450a15119bcb2";
            TransactionRisk transactionRisk = new TransactionRisk(hash);
            await transactionRisk.ShowRisk();
        }
    }
}
