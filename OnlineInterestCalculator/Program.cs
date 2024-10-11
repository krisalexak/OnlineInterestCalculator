using HtmlAgilityPack;
using OnlineInterestCalculator.Services;
using OnlineInterestCalculator.Dto;
using Spectre.Console;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Reflection.Metadata;

const string url = "https://www.bankofgreece.gr/statistika/xrhmatopistwtikes-agores/ekswtrapezika-epitokia";
const string cultureInfoStr = "el-GR";
Console.OutputEncoding = System.Text.Encoding.UTF8;

var consoleService = new ConsoleService(cultureInfoStr);
var interestCalculatorService = new InterestCalculatorService(cultureInfoStr);
var scrapingService = new ScrapingService(cultureInfoStr);
consoleService.DisableConsoleResize(); // disable console resize because library Spectre.Console looks broken

Console.WriteLine("Scraping data from the web, please wait...");

var ratesPerPeriod = await scrapingService.GetInterestRatesPerPeriod(url);

Console.Clear();

(DateTime validFrom, DateTime validTo, decimal amount) = consoleService.GetUserInput();

var result = interestCalculatorService.CalculateInterests(ratesPerPeriod, validFrom, validTo, amount);

consoleService.DisplayResults(result, validFrom, validTo);

Console.ReadLine();



