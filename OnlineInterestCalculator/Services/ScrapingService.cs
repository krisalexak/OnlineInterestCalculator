using HtmlAgilityPack;
using OnlineInterestCalculator.Dto;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineInterestCalculator.Services
{
    internal class ScrapingService
    {
        private readonly string _cultureInfoStr;
        public ScrapingService(string cultureInfoStr)
        {
            _cultureInfoStr = cultureInfoStr;
        }
        internal async Task<List<InterestRatePerPeriodDto>> GetInterestRatesPerPeriod(string url)
        {
            var details = new List<InterestRatePerPeriodDto>();

            try
            {
                HttpClient client = new HttpClient();
                string html = await client.GetStringAsync(url);

                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(html);

                var table = doc.DocumentNode.SelectSingleNode("//table[@class='insuranceCompany__table']");

                if (table != null)
                {
                    var rows = table.SelectNodes(".//tbody/tr");

                    foreach (var row in rows)
                    {
                        var columns = row.SelectNodes("td");

                        if (columns != null && columns.Count == 6)
                        {

                            string validFrom = columns[0].InnerText.Trim();
                            string validTo = columns[1].InnerText.Trim();
                            validTo = (
                                validTo == "-" ?
                                DateTime.MaxValue.ToString("dd/MM/yyyy") :
                                columns[1].InnerText.Trim());
                            string legalRate = columns[4].InnerText.Replace("%", string.Empty).Trim();
                            string defaultRate = columns[5].InnerText.Replace("%", string.Empty).Trim();

                            var detail = new InterestRatePerPeriodDto(
                                DateTime.Parse(validFrom, new CultureInfo(_cultureInfoStr)),
                                DateTime.Parse(validTo, new CultureInfo(_cultureInfoStr)),
                                Decimal.Parse(legalRate, new CultureInfo(_cultureInfoStr)),
                                Decimal.Parse(defaultRate, new CultureInfo(_cultureInfoStr))
                                );

                            details.Add(detail);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Table not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            return details;
        }
    }
}
