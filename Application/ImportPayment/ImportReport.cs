using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using BankReport;
using Entity;

namespace Application.ImportPayment
{
    public class ImportReportHelper : IImportReportHelper
    {
        private readonly SHA256 _hashAlgorithm;

        public ImportReportHelper()
        {
            _hashAlgorithm = SHA256.Create();
        }

        public IReportImportResult Import(string[] report)
        {
            var factory = new ReportAnalyzerFactory();
            var result = new ReportImportResult();
            var analyzer = factory.CreateAnalyzerByFileHeader(report[0]);
            for (int i = 1; i < report.Length; i++)
            {
                try
                {
                    var bankPayment = analyzer.ParseReportLine(report[i]);
                    var payment = new Payment();
                    if (bankPayment.Amout > 0)
                        payment.Income = true;
                    payment.Amount = new decimal(Math.Abs(bankPayment.Amout));
                    payment.Date = bankPayment.Date;
                    payment.Details = bankPayment.Details;

                    payment.PaymentId = GetLineHash(report[i]);

                    result.ValidPayments.Add(payment);
                }
                catch (Exception)
                {
                    result.InvalidPaymentsLine.Add(report[i]);
                    throw;
                }
            }
            return result;
        }

        private string GetLineHash(string line)
        {
            var hash = _hashAlgorithm.ComputeHash(Encoding.UTF32.GetBytes(line));
            return string.Join("-", hash);
        }

        private class ReportImportResult : IReportImportResult
        {
            public ReportImportResult()
            {
                ValidPayments = new HashSet<Payment>();
                InvalidPaymentsLine = new HashSet<string>();
            }

            public ICollection<Payment> ValidPayments { get; private set; }
            public ICollection<string> InvalidPaymentsLine { get; private set; }
        }
    }
}
