namespace USMPWEB.Models;

public class ErrorViewModel
{
    public string? RequestId { get; set; }
    public string? Message { get; set; }
    public string? DetailedMessage { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
