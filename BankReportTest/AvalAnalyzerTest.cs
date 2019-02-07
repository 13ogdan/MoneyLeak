using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BankReport;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BankReportFrameworkTest
{

    [TestClass]
    public class AvalAnalyzerTest
    {
        private const string SimpleLine = @"04.11.2018/05.11.2018;5437 59** **** 8050;Торгівельна точка;205 - Безготівковий платіж.dofiokofekafe KIYEV;28 865,82;-148,00;-148,00;UAH;28 717,82";
        private const string LineWithDollarLikeACurrency = @"07.10.2018/08.10.2018;5437 59** **** 8050;Торгівельна точка;205 - Безготівковий платіж.ITUNES.COM/BILL ITUNES.COM;58 658,97;-28,25;-0,99;USD;58 630,72";
        private const string LineWithEmptyData = "06.10.2018/06.10.2018;;Банк;\"Місячна плата за рахунок. \";;-650,00;-650,00;UAH;";
        private const string IncommingBill = "24.09.2018/24.09.2018;;Банк;Безготівкове зарахування.ельщик: 3272916731 МФО: 380805 ЛС: 26009545616 Наименование: ФОП Спільний Богдан Вікторович.Київська РД Кр.суми до з`ясування_/_23494105_/_№39 від 24/09/18, Поповнення КР № 1188534400. Переказ власних коштів. Податки сплаченно. Пла;10 871,66;64 326,00;64 326,00;UAH;75 197,66";

        [TestMethod]
        public void Should_ParseFullLineOfAvalReport()
        {
            var analyzer = new AvalAnalizer();

            var report = analyzer.ParseReportLine(SimpleLine);

            Assert.AreEqual(new DateTime(2018, 11, 4), report.Date);
            Assert.AreEqual("205 - Безготівковий платіж.dofiokofekafe KIYEV", report.Details);
            Assert.AreEqual(-148.00, report.Amount);
        }

        [TestMethod]
        public void Should_ParseLine_When_CurrencyIsNotGryvna()
        {
            var analyzer = new AvalAnalizer();

            var report = analyzer.ParseReportLine(LineWithDollarLikeACurrency);

            Assert.AreEqual(-28.25, report.Amount);
        }

        [TestMethod]
        public void Should_ParseLine_When_SomeFieldsAbsent()
        {
            var analyzer = new AvalAnalizer();

            analyzer.ParseReportLine(LineWithEmptyData);
        }

        [TestMethod]
        public void Should_ParseLine_When_ItIsIncomming()
        {
            var analyzer = new AvalAnalizer();

            analyzer.ParseReportLine(IncommingBill);
        }

        [TestMethod]
        public void Should_Indicate_When_CanParseDocument()
        {
            var analyzer = new AvalAnalizer();
            string[] report = ReadReportFromResource();

            var canParseReport = AvalAnalizer.CanParse(report[0]);

            Assert.IsTrue(canParseReport);
        }

        [TestMethod]
        public void Should_ParseWholeDocument()
        {
            var analyzer = new AvalAnalizer();
            string[] report = ReadReportFromResource();
            var parsed = new List<IPayment>();

            for (int i = 1; i < report.Length; i++)
            {
                try
                {
                    var item = analyzer.ParseReportLine(report[i]);
                    parsed.Add(item);
                }
                catch (Exception ex)
                {
                    Assert.Fail("Cannot parse line:{0}; Exception: {1}", report[i], ex);
                }
            }

            Assert.AreEqual(report.Length - 1, parsed.Count);
        }

        private static string[] ReadReportFromResource()
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream("BankReportTest.Resources.070818-061118.csv"))
            using (var sr = new StreamReader(stream))
                return sr.ReadToEnd().Split(Environment.NewLine,StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
