namespace OrdersSystem.Data.Process.Validation
{
    public class ValidationResult
    {
        public ValidationResult()
        {
            IsValid = true;
            ErrorMessages = new List<string>();
            ErrorCodes = new List<OrderErrorCode>();
        }

        public bool IsValid { get; set; }
        public List<string> ErrorMessages { get; set; }
        public List<OrderErrorCode> ErrorCodes { get; set; }
    }
}
