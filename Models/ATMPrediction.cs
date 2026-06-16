using Microsoft.ML.Data;
public class ATMPrediction
{
   [ColumnName("PredictedLabel")]

   public bool Prediction { get; set; }
   public float Probability { get; set; }
   public float Score { get; set; }
}