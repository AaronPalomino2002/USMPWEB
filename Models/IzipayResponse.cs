public class IzipayResponse
{
    public string ShopId { get; set; }
    public string OrderId { get; set; }
    public string TransactionId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; }
    public string Status { get; set; }
    public DateTime TransactionDate { get; set; }
}