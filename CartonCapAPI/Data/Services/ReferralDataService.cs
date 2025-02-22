using CartonCapAPI.Data.Services.Interfaces;
using CartonCapAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CartonCapAPI.Data.Services
{
    public class ReferralDataService : IReferralDataService
    {
        public List<ReferralModel> _mockDataSet = MockDataSet.DataSet.ToList();

        public ReferralDataService()
        {

        }

        public IEnumerable<ReferralModel> Get(int userId)
        {
            var results = _mockDataSet.Where(x => x.OriginatingUserId == userId);
            return results;
        }

        public ReferralModel GetById(int referralId)
        {
            var result = _mockDataSet.SingleOrDefault(x => x.ReferralId == referralId);
            return result;
        }

        public int Create(ReferralModel model)
        {
            var rnd = new Random();
            
            // When connected to a DB use the auto increment or guid generation for the key instead.
            model.ReferralId = rnd.Next(1, 100000);
            _mockDataSet.Add(model);

            return model.ReferralId;
        }

        public int Update(ReferralModel model)
        {
            //Replace with a real update using EF or whatever other tools for the corresponding db.
            var existingModel = _mockDataSet.SingleOrDefault(x => x.ReferralId == model.ReferralId);
            if (existingModel == null)
            {
                // If it doesn't exist in the DB return 0 and let business layer handle error message.
                return 0;
            }

            _mockDataSet.Remove(existingModel);
            _mockDataSet.Add(model);

            return (model.ReferralId);
        }

        public bool Delete(int referralId)
        {
            //Replace with a status update if cancelled status is used instead of hard delete.
            var existingModel = _mockDataSet.SingleOrDefault(x => x.ReferralId == referralId);
            if (existingModel == null)
            {
                // If it doesn't exist in the DB return false and let business layer handle error message.
                return false;
            }

            if (existingModel.Status > Models.Enums.ReferralStatusesEnum.Sent)
            {
                throw new ValidationException("Can not delete a referral after its been used.");
            }

            _mockDataSet.Remove(existingModel);
            return true;
        }

        public ReferralModel CheckCode(string referralCode)
        {
            var result = _mockDataSet.SingleOrDefault(x => x.Code == referralCode);
            if (result == null)
            {
                return new ReferralModel();
            }

            return result;
        }
    }
}
