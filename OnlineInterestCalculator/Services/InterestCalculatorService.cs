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
    internal class InterestCalculatorService
    {
        private readonly string _cultureInfoStr;
        public InterestCalculatorService(string cultureInfoStr) 
        {
            _cultureInfoStr = cultureInfoStr;
        }
        internal ResultDto CalculateInterests(List<InterestRatePerPeriodDto> ratesPerPeriod, DateTime validFrom, DateTime validTo, decimal amount)
        {
            DateTime? unionFrom; DateTime? unionTo;
            var resultLines = new List<ResultLineDto>();
            var daysPerPeriod = 0;
            var calendarDays = 0;
            decimal legalInterestPerPeriod = 0;
            decimal defaultInterestPerPeriod = 0;
            decimal legalInterestSum = 0;
            decimal defaultInterestSum = 0;

            foreach (var ratePerPeriod in ratesPerPeriod)
            {
                (unionFrom, unionTo) = FindUnion(ratePerPeriod.ValidFrom, ratePerPeriod.ValidTo, validFrom, validTo);
                if (unionFrom is not null && unionTo is not null)
                {
                    daysPerPeriod = (unionTo - unionFrom).Value.Days;
                    // get the days of the calendar year, assuming the interest rate is defined based on the year it started i.e. ValidFrom 
                    calendarDays = DateTime.IsLeapYear(ratePerPeriod.ValidFrom.Year) ? 366 : 365; 

                    legalInterestPerPeriod = CalculateInterest(ratePerPeriod.LegalRate, daysPerPeriod, calendarDays, amount);
                    defaultInterestPerPeriod = CalculateInterest(ratePerPeriod.DefaultRate, daysPerPeriod, calendarDays, amount);

                    resultLines.Add(
                        new ResultLineDto(
                            unionFrom.Value,
                            unionTo.Value,
                            daysPerPeriod,
                            LegalRate: new ResultRateDto(ratePerPeriod.LegalRate, legalInterestPerPeriod),
                            DefaultRate: new ResultRateDto(ratePerPeriod.DefaultRate, defaultInterestPerPeriod))
                        );
                }
            }
            legalInterestSum = resultLines.Sum(x => x.LegalRate.Interest);
            defaultInterestSum = resultLines.Sum(x => x.DefaultRate.Interest);

            return new ResultDto(
                    ResultLines: resultLines,
                    InitialCapital: amount,
                    LegalInterestSum: legalInterestSum,
                    DefaultInterestSum: defaultInterestSum,
                    LegalInterestTotal: amount + legalInterestSum,
                    DefaultInterestTotal: amount + defaultInterestSum
                    );
        }

        internal static(DateTime? unionFrom, DateTime? unionTo) FindUnion(DateTime fromDate1, DateTime toDate1, DateTime fromDate2, DateTime toDate2)
        {
            if (fromDate1 > toDate1 || fromDate2 > toDate2)
            {
                return (null, null);
            }

            if (toDate1 < fromDate2 || toDate2 < fromDate1)
            {
                return (null, null);
            }

            DateTime unionFrom = new DateTime(Math.Max(fromDate1.Ticks, fromDate2.Ticks));
            DateTime unionTo = new DateTime(Math.Min(toDate1.Ticks, toDate2.Ticks));

            return (unionFrom, unionTo);
        }

        internal decimal CalculateInterest(decimal interestRate, int days, int calendarDays, decimal amount)
        {
            return Decimal.Round(interestRate * days * amount / (calendarDays * 100), 2);
        }
    }
}
