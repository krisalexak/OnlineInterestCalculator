using OnlineInterestCalculator.Dto;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace OnlineInterestCalculator.Services
{
    internal class ConsoleService
    {
        private readonly string _cultureInfoStr;
        public ConsoleService(string cultureInfoStr) 
        {
            _cultureInfoStr = cultureInfoStr;
        }
        internal (DateTime, DateTime, decimal) GetUserInput()
        {
            DateTime validFromMin = new DateTime(1946, 8, 21);
            DateTime validFrom = DateTime.Now.Date;
            DateTime validTo = DateTime.Now.Date;
            decimal amount = 0;

            try
            {
                while (true)
                {
                    Console.Write("Enter the 'from' date (dd/MM/yyyy): ");
                    string? fromInput = Console.ReadLine();

                    if (DateTime.TryParseExact(fromInput, "dd/MM/yyyy", new CultureInfo(_cultureInfoStr), DateTimeStyles.None, out validFrom))
                    {
                        if (validFrom.Date >= validFromMin.Date)
                        {
                            break;
                        }
                        else
                        {
                            Console.WriteLine($"'From' date must be after {validFromMin.ToString("dd/MM/yyyy")}. Please try again.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid date format. Please try again.");
                    }
                }

                Console.Clear();

                while (true)
                {
                    Console.Write("Enter the 'to' date (dd/MM/yyyy): ");
                    string? toInput = Console.ReadLine();

                    if (DateTime.TryParseExact(toInput, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out validTo))
                    {
                        if (validTo >= validFrom)
                        {
                            break;
                        }
                        else
                        {
                            Console.WriteLine("'To' date must be on or after 'From' date. Please try again.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid date format. Please try again.");
                    }
                }

                Console.Clear();

                while (true)
                {
                    Console.Write("Enter amount of money: ");
                    string? amountInput = Console.ReadLine();

                    if (decimal.TryParse(amountInput, out amount))
                    {
                        if (amount >= 0)
                        {
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Invalid amount. Please enter a positive number.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid amount. Please enter a valid number.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return new(validFrom, validTo, amount);
        }
        internal void DisplayResults(ResultDto result, DateTime validFrom, DateTime validTo)
        {
            Console.Clear();
            Console.WriteLine($"For your input of € {result.InitialCapital}:");
            Console.WriteLine($"Interests for the specified range from {validFrom.ToString("dd/MM/yyyy")} to {validTo.ToString("dd/MM/yyyy")} (Calendar year)");

            var linesTable = new Table();
            linesTable.AddColumn("Date(from)");
            linesTable.AddColumn("Date(to)");
            linesTable.AddColumn("Days");
            linesTable.AddColumn("Legal Rate(%)");
            linesTable.AddColumn("Legal Interest(€)");
            linesTable.AddColumn("Default Rate(%)");
            linesTable.AddColumn("Default Interest(€)");

            foreach (var line in result.ResultLines)
            {


                linesTable.AddRow(
                    line.ValidFrom.ToString("dd/MM/yyyy"),
                    line.ValidTo.ToString("dd/MM/yyyy"),
                    line.Days.ToString(),
                    line.LegalRate.Rate.ToString(),
                    line.LegalRate.Interest.ToString(),
                    line.DefaultRate.Rate.ToString(),
                    line.DefaultRate.Interest.ToString()
                    );


            }

            linesTable.AddRow(
                "------",
                "------",
                "------",
                "------",
                "------",
                "------",
                "------"
                );
            linesTable.AddRow(
                "Initial capital",
                string.Empty,
                string.Empty,
                string.Empty,
                result.InitialCapital.ToString(),
                string.Empty,
                result.InitialCapital.ToString()
                );
            linesTable.AddRow(
                "Rate (€)",
                string.Empty,
                string.Empty,
                string.Empty,
                result.LegalInterestSum.ToString(),
                string.Empty,
                result.DefaultInterestSum.ToString()
                );
            linesTable.AddRow(
                "Total",
                string.Empty,
                string.Empty,
                string.Empty,
                result.LegalInterestTotal.ToString(),
                string.Empty,
                result.DefaultInterestTotal.ToString()
                );
            AnsiConsole.Write(linesTable);
        }

        internal void DisableConsoleResize() 
        {
            IntPtr handle = GetConsoleWindow();
            IntPtr sysMenu = GetSystemMenu(handle, false);
            DeleteMenu(sysMenu, 0xF000, 0x00000000);
        }


        [DllImport("kernel32.dll", ExactSpelling = true)]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll")]
        static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);
    }
}
