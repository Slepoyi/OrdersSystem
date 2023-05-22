namespace OrdersSystem.Data.Process.Validation
{
    public class ValidationResult
    {
        public ValidationResult()
        {
            ErrorMessages = new List<string>();
            ErrorCodes = new List<OrderErrorCode>();
        }

        public bool IsValid
        {
            get
            {
                return ErrorCodes.Count == 0;
            }
        }
        public List<string> ErrorMessages { get; set; }
        public List<OrderErrorCode> ErrorCodes { get; set; }
    }
}
