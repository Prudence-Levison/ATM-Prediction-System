using Microsoft.ML.Data;

namespace ATM_Prediction_System.Models
{
    public class ATMData
    {
        [LoadColumn(0)]
        public string? Bank { get; set; }

        [LoadColumn(1)]
        public string? ATMName { get; set; }

        [LoadColumn(2)]
        public string? Address { get; set; }

        [LoadColumn(3)]
        public string? State { get; set; }

        [LoadColumn(4)]
        public string? City { get; set; }

        [LoadColumn(5)]
        public string? CashAvailability { get; set; }

        [LoadColumn(6)]
        public string? LastChecked { get; set; }
    }
}