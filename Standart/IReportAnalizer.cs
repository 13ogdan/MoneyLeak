namespace BankReport
{
    public interface IReportAnalyzer
    {
        IPayment ParseReportLine(string line);
    }
}