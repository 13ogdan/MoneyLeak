namespace BankReport
{
    public interface IPayment
    {
        double Amount { get; }
        System.DateTime Date { get; }
        string Details { get; }
    }
}