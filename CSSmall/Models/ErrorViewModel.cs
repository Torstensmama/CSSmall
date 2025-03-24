namespace CSSmall.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        // L�gg till en ErrorMessage-egenskap
        public string ErrorMessage { get; set; }
    }
}