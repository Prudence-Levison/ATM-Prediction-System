using Microsoft.AspNetCore.Mvc;
using ATM_Prediction_System.ML;
using ATM_Prediction_System.Models;

namespace ATM_Prediction_System.Controllers
{
    public class TestController : Controller
    {
        public IActionResult LoadData()
        {
            var trainer = new ModelTrainer();
            trainer.TestLoadData();

            return Content("Data loaded. Check terminal.");
        }
        public IActionResult Train()
        {
            var trainer = new ModelTrainer();
            trainer.TrainModel();

            return Content("Training started. Check terminal.");
        }

        [HttpGet]
        public IActionResult PredictTest()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Predict(ATMRequest request)
        {
            var trainer = new ModelTrainer();
            var result = trainer.Predict(request);

            return View("Result", result);
        }
    }
}