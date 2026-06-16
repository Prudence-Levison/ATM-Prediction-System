namespace ATM_Prediction_System.Models
{
    public class ATMRequest
    {
        public string? Bank { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ATMName { get; set; }

        public string? Address { get; set; }
        public DateTime PredictionDateTime { get; set; }

    }
}