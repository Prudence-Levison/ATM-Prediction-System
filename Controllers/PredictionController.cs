using Microsoft.AspNetCore.Mvc;
using ATM_Prediction_System.ML;
using ATM_Prediction_System.Models;

namespace ATM_Prediction_System.Controllers
{
    public class PredictController : Controller
    {
        private readonly ATMDataService _service;

        public PredictController()
        {
            _service = new ATMDataService();
        }
        public IActionResult Index()
        {
            ViewBag.Banks = _service.GetBanks();
            return View();
        }
        public JsonResult GetStates(string bank)
        {
            return Json(_service.GetStates(bank));
        }
        public JsonResult GetCities(string bank, string state)
        {
            return Json(_service.GetCities(bank, state));
        }
        public JsonResult GetAddresses(string bank, string state, string city)
        {
            return Json(_service.GetAddresses(bank, state, city));
        }

        [HttpPost]
public IActionResult Result(ATMRequest request)
{
    if (string.IsNullOrWhiteSpace(request.Bank) ||
        string.IsNullOrWhiteSpace(request.State) ||
        string.IsNullOrWhiteSpace(request.City) ||
        string.IsNullOrWhiteSpace(request.Address))
    {
        ViewBag.ErrorMessage = "Please complete all fields.";
        ViewBag.Banks = _service.GetBanks();
        return View("Index");
    }

    var atm = _service.GetATM(
        request.Bank,
        request.State,
        request.City,
        request.Address
    );

    if (atm != null)
    {
        request.ATMName = atm.ATMName;
    }

    var trainer = new ModelTrainer();
    var result = trainer.Predict(request);

    return View(result);
}
    }
}