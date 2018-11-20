namespace BankReport
{
    public interface IPayment
    {
        double Amout { get; }
        System.DateTime Date { get; }
        string Details { get; }
    }
}