using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.IO;
using System.Collections.Generic;
using ATM_Prediction_System.Models;

namespace ATM_Prediction_System.ML
{
    public class ModelTrainer
    {
        private readonly MLContext _mlContext;
        private ITransformer _model;

        private readonly string ModelPath =
            Path.Combine(AppContext.BaseDirectory, "ML", "atm_model.zip");

        public ModelTrainer()
        {
            _mlContext = new MLContext(seed: 0);
            LoadModel();
        }

        public void TestLoadData()
        {
            var dataPath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "nigeriaatmdataset.csv");

            var data = _mlContext.Data.LoadFromTextFile<ATMData>(
                path: dataPath,
                hasHeader: true,
                separatorChar: ',',
                allowQuoting: true
            );

            var rows = _mlContext.Data.CreateEnumerable<ATMData>(data, reuseRowObject: false);

            Console.WriteLine("Dataset Preview:");

            int count = 0;
            foreach (var row in rows)
            {
                Console.WriteLine($"{row.Bank} | {row.ATMName} | {row.City} | {row.CashAvailability}");

                count++;
                if (count == 5)
                    break;
            }

            Console.WriteLine("Dataset loaded successfully");
        }

        private bool ConvertLabel(string cashAvailability)
        {
            return cashAvailability?.Trim().ToLower() == "available";
        }

        public IEnumerable<ATMTrainData> PrepareData()
{
    var dataPath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "nigeriaatmdataset.csv");

    var data = _mlContext.Data.LoadFromTextFile<ATMData>(
        path: dataPath,
        hasHeader: true,
        separatorChar: ',',
        allowQuoting: true
    );

    var rows = _mlContext.Data.CreateEnumerable<ATMData>(data, reuseRowObject: false);

    int valid = 0;
    int skipped = 0;

    foreach (var row in rows)
    {
        if (string.IsNullOrWhiteSpace(row.LastChecked))
        {
            skipped++;
            continue;
        }

        
        if (!DateTime.TryParse(
                row.LastChecked,
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out var dt))
        {
            skipped++;
            continue;
        }

        if (string.IsNullOrWhiteSpace(row.Bank) ||
            string.IsNullOrWhiteSpace(row.ATMName) ||
            string.IsNullOrWhiteSpace(row.City) ||
            string.IsNullOrWhiteSpace(row.State))
        {
            skipped++;
            continue;
        }

        valid++;

        yield return new ATMTrainData
        {
            Bank = row.Bank.Trim().ToLower(),
            ATMName = row.ATMName.Trim().ToLower(),
            City = row.City.Trim().ToLower(),
            State = row.State.Trim().ToLower(),

            HourOfDay = dt.Hour,
            DayOfWeek = (float)dt.DayOfWeek,

            Label = row.CashAvailability?.Trim().ToLower() == "available"
        };
    }

    Console.WriteLine($"VALID ROWS: {valid}");
    Console.WriteLine($"SKIPPED ROWS: {skipped}");
}
         public void TrainModel()
{
    Console.WriteLine(" Training started...");

    var dataList = PrepareData().ToList();

    Console.WriteLine($"Rows after cleaning: {dataList.Count}");

    if (dataList.Count == 0)
        throw new Exception("No valid training data found.");

    var data = _mlContext.Data.LoadFromEnumerable(dataList);

    var split = _mlContext.Data.TrainTestSplit(data, testFraction: 0.2);

    var pipeline =
        _mlContext.Transforms.Categorical.OneHotEncoding("BankEncoded", nameof(ATMTrainData.Bank))
        .Append(_mlContext.Transforms.Categorical.OneHotEncoding("ATMEncoded", nameof(ATMTrainData.ATMName)))
        .Append(_mlContext.Transforms.Categorical.OneHotEncoding("CityEncoded", nameof(ATMTrainData.City)))
        .Append(_mlContext.Transforms.Categorical.OneHotEncoding("StateEncoded", nameof(ATMTrainData.State)))

        .Append(_mlContext.Transforms.Concatenate("Features",
            "BankEncoded",
            "ATMEncoded",
            "CityEncoded",
            "StateEncoded",
            nameof(ATMTrainData.HourOfDay),
            nameof(ATMTrainData.DayOfWeek)
        ))

        .Append(_mlContext.Transforms.NormalizeMinMax("Features"))
        .Append(_mlContext.BinaryClassification.Trainers.FastTree(
            labelColumnName: nameof(ATMTrainData.Label),
            featureColumnName: "Features"
        ));

    Console.WriteLine("Fitting model...");

    _model = pipeline.Fit(split.TrainSet);

    Console.WriteLine(" Training completed");

    var folder = Path.Combine(AppContext.BaseDirectory, "ML");
    Directory.CreateDirectory(folder);

    _mlContext.Model.Save(_model, split.TrainSet.Schema, ModelPath);

    Console.WriteLine($" Model saved at: {ModelPath}");

    var predictions = _model.Transform(split.TestSet);
    var metrics = _mlContext.BinaryClassification.Evaluate(predictions);

    Console.WriteLine($"Accuracy: {metrics.Accuracy:P2}");
}
               public void LoadModel()
        {
            try
            {
                if (File.Exists(ModelPath))
                {
                    DataViewSchema schema;
                    _model = _mlContext.Model.Load(ModelPath, out schema);
                    Console.WriteLine(" Model loaded successfully");
                }
                else
                {
                    Console.WriteLine(" No trained model found. Run TrainModel first.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(" Model load error: " + ex.Message);
            }
        }

                public ATMResult Predict(ATMRequest request)
        {
            if (_model == null)
                LoadModel();

            if (_model == null)
                throw new Exception("Model not found. TrainModel first.");

            var engine = _mlContext.Model.CreatePredictionEngine<ATMTrainData, ATMPrediction>(_model);

            var dt = request.PredictionDateTime;

            var input = new ATMTrainData
            {
                Bank = request.Bank?.Trim().ToLower(),
                ATMName = request.ATMName?.Trim().ToLower(),
                City = request.City?.Trim().ToLower(),
                State = request.State?.Trim().ToLower(),

                HourOfDay = dt.Hour,
                DayOfWeek = (int)dt.DayOfWeek
            };

            var result = engine.Predict(input);

            return new ATMResult
            {
                Prediction = result.Prediction ? "Available" : "Out of Cash",
                Probability = result.Probability
            };
        }
    }
}