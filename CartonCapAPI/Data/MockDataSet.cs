using CartonCapAPI.Models;
using System;
using System.Collections.Generic;

namespace CartonCapAPI.Data
{
    public static class MockDataSet
    {
        public static IEnumerable<ReferralModel> DataSet = new List<ReferralModel>
        {
            new ReferralModel()
            {
                ReferralId = 1,
                OriginatingUserId = 1,
                Code = "ABC123",
                EmailAddress = "johnDoe@email.com",
                LastSent = DateTime.UtcNow,
                IsSent = true,
                IsUserCreated = false,
                Status = Models.Enums.ReferralStatusesEnum.Sent
            },
            new ReferralModel()
            {
                ReferralId = 2,
                OriginatingUserId = 1,
                RefereeUserId = 2,
                Code = "DEF123",
                PhoneNumber = "555-444-1234",
                LastSent = DateTime.UtcNow.AddDays(-1),
                IsSent = true,
                IsUserCreated = true,
                Status = Models.Enums.ReferralStatusesEnum.UserCreated
            },
            new ReferralModel()
            {
                ReferralId = 3,
                OriginatingUserId = 2,
                Code = "ABC456",
                EmailAddress = "JaneDoe@email.com",
                PhoneNumber = "555-444-1234",
                LastSent = DateTime.UtcNow.AddDays(-1),
                IsSent = true,
                IsUserCreated = false,
                Status = Models.Enums.ReferralStatusesEnum.Sent
            }
        };
    }
}
