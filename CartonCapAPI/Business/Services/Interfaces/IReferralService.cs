using CartonCapAPI.Models;
using System.Collections.Generic;

namespace CartonCapAPI.Business.Services.Interfaces
{
    public interface IReferralService
    {
        public IEnumerable<ReferralModel> Get(int userId);
        public int Create(ReferralModel model);
        public int Update(ReferralModel model);
        public bool Delete(int referralId);
        public bool Resend(int referralId);
        public bool CheckCode(string referralCode);
    }
}
