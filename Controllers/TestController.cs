using Microsoft.AspNetCore.Mvc;
using ATM_Prediction_System.ML;
using ATM_Prediction_System.Models;

namespace ATM_Prediction_System.Controllers
{
    public class TestController : Controller
    {
        // STEP 1: Test dataset loading
        public IActionResult LoadData()
        {
            var trainer = new ModelTrainer();
            trainer.TestLoadData();

            return Content("Data loaded. Check terminal.");
        }

        // STEP 2: Train model
        public IActionResult Train()
        {
            var trainer = new ModelTrainer();
            trainer.TrainModel();

            return Content("Training started. Check terminal.");
        }

        // STEP 3: SHOW UI PAGE (THIS WAS MISSING)
        [HttpGet]
        public IActionResult PredictTest()
        {
            return View();
        }

        // STEP 4: HANDLE PREDICTION
        [HttpPost]
        public IActionResult Predict(ATMRequest request)
        {
            var trainer = new ModelTrainer();
            var result = trainer.Predict(request);

            return View("Result", result);
        }
    }
}