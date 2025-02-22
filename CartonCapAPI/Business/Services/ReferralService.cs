using CartonCapAPI.Business.Services.Interfaces;
using CartonCapAPI.Data.Services.Interfaces;
using CartonCapAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CartonCapAPI.Business.Services
{
    public class ReferralService : IReferralService
    {
        private readonly IReferralDataService _referralDataService;
        private static readonly Random rnd = new Random();
        private static readonly string allowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        public ReferralService(IReferralDataService referralDataService)
        {
            _referralDataService = referralDataService;
        }

        public IEnumerable<ReferralModel> Get(int userId)
        {
            var result = _referralDataService.Get(userId);
            return result;
        }

        public int Create(ReferralModel model)
        {
            ValidateModel(model);

            var existingReferrals = Get(model.OriginatingUserId);

            // Fairly simple way to limit abuse or mass generation of referrals. Would need some additional logic around the emails and phone numbers to really 
            // make this a way to prevent abuse. Could also look at limiting based on time range where only X amount per hour can be generated.
            if (existingReferrals.Count(x => x.Status < Models.Enums.ReferralStatusesEnum.Created) > 10)
            {
                throw new ValidationException("You can only have 10 active refferals at once, please cancel or wait for them to be redeemed.");
            }

            // Generate random 6 character string for code, would need some logic eventually to prevent duplicates and would probably want a larger string
            // depending on the amount of users.
            model.Code = GenerateRandomString();

            var result = _referralDataService.Create(model);
            return result;
        }

        public int Update(ReferralModel model)
        {
            ValidateModel(model, true);
            var result = _referralDataService.Update(model);

            if (result == default)
            {
                throw new ValidationException("Unable to update the referral.");
            }

            return result;
        }

        public bool Delete(int referralId)
        {
            var result = _referralDataService.Delete(referralId);

            if (!result)
            {
                throw new ValidationException("Unable to delete the referral.");
            }

            return result;
        }

        public bool Resend(int referralId)
        {
            var existingModel = _referralDataService.GetById(referralId);
            if (existingModel.Status > Models.Enums.ReferralStatusesEnum.Sent)
            {
                throw new ValidationException("Can not resend the referral after its been used or cancelled.");
            }

            if (existingModel.LastSent >= DateTime.UtcNow.AddHours(-1))
            {
                throw new ValidationException("Must wait an hour before resending the referral.");
            }

            // TODO: Call some link generation service with the code from the referral, assume it returns true.

            // Update model with new last sent time, make sure status is still in sent.
            existingModel.LastSent = DateTime.UtcNow;
            existingModel.Status = Models.Enums.ReferralStatusesEnum.Sent;

            Update(existingModel);

            return true;
        }

        public bool CheckCode(string referralCode)
        {
            var result = _referralDataService.CheckCode(referralCode);
            if (result.ReferralId == default)
            {
                return false;
            }

            // Treating this as a redeem code route since most of the logic would be in the other parts of the app just updating status when the code is present.
            // Set random user id that would come from the new user onboarding flow.
            result.Status = Models.Enums.ReferralStatusesEnum.Created;
            var rnd = new Random();
            result.RefereeUserId = rnd.Next(1, 10000);
            Update(result);

            return true;
        }

        private void ValidateModel(ReferralModel model, bool isUpdate = false)
        {
            // Validate the model for creates and updates and return validation messages to allow for easier fixes.
            var validationMessage = string.Empty;

            if (model.OriginatingUserId == default)
            {
                validationMessage += "Originating user id is required.";
            }

            if (!isUpdate && !string.IsNullOrWhiteSpace(model.Code))
            {
                validationMessage += "Code should be empty on create.";
            }

            if (string.IsNullOrWhiteSpace(model.PhoneNumber) && string.IsNullOrWhiteSpace(model.EmailAddress))
            {
                validationMessage += "Phone number or email address must be provided.";
            }

            if (isUpdate)
            {
                if (model.Status > Models.Enums.ReferralStatusesEnum.Sent)
                {
                    validationMessage += "Referral can not be updated after the its been used.";
                }

                if (model.ReferralId == default)
                {
                    validationMessage += "Referral ID is required on updates.";
                }
            }

            if (!string.IsNullOrWhiteSpace(validationMessage))
            {
                throw new ValidationException(validationMessage);
            }
        }

        private static string GenerateRandomString()
        {
            var code = string.Empty;
            for (int i = 0; i < 6; i++)
            {
                code += allowedChars[rnd.Next(6)];
            }
            return code;
        }
    }
}
