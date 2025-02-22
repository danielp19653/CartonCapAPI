using CartonCapAPI.Models;
using System.Collections.Generic;

namespace CartonCapAPI.Data.Services.Interfaces
{
    public interface IReferralDataService
    {
        public IEnumerable<ReferralModel> Get(int userId);
        public ReferralModel GetById(int referralId);
        public int Create(ReferralModel model);
        public int Update(ReferralModel model);
        public bool Delete(int referralId);
        public ReferralModel CheckCode(string referralCode);
    }
}
