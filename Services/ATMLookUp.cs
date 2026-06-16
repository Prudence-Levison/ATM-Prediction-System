using ATM_Prediction_System.Models;
using Microsoft.ML;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ATM_Prediction_System.ML
{
    public class ATMDataService
    {
        private List<ATMData> _data;

        public ATMDataService()
        {
            LoadData();
        }

        private void LoadData()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Data", "nigeriaatmdataset.csv");

            var mlContext = new MLContext();

            var data = mlContext.Data.LoadFromTextFile<ATMData>(
                path: path,
                hasHeader: true,
                separatorChar: ',',
                allowQuoting: true
            );

            _data = mlContext.Data.CreateEnumerable<ATMData>(data, false).ToList();
        }

        public List<string> GetBanks()
            => _data.Select(x => x.Bank).Distinct().ToList();

        public List<string> GetStates(string bank)
            => _data.Where(x => x.Bank == bank)
                    .Select(x => x.State)
                    .Distinct()
                    .ToList();

        public List<string> GetCities(string bank, string state)
{
    return _data
        .Where(x => x.Bank.Trim().ToLower() == bank.Trim().ToLower()
                 && x.State.Trim().ToLower() == state.Trim().ToLower())
        .Select(x => x.City)
        .Distinct()
        .ToList();
}
        public List<string> GetAddresses(string bank, string state, string city)
            => _data.Where(x => x.Bank.Trim().ToLower() == bank.Trim().ToLower() &&
                                x.State.Trim().ToLower() == state.Trim().ToLower() &&
                                x.City.Trim().ToLower() == city.Trim().ToLower())
                    .Select(x => x.Address)
                    .Distinct()
                    .ToList();

        public ATMData GetATM(string bank, string state, string city, string address)
            => _data.FirstOrDefault(x =>
                x.Bank.Trim().ToLower() == bank.Trim().ToLower() &&
                x.State.Trim().ToLower() == state.Trim().ToLower() &&
                x.City.Trim().ToLower() == city.Trim().ToLower() &&
                x.Address.Trim().ToLower() == address.Trim().ToLower()
            );
    }
}