using CartonCapAPI.Business.Services;
using CartonCapAPI.Data.Services.Interfaces;
using CartonCapAPI.Models;
using CartonCapAPI.Models.Enums;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace CartonApi.UnitTests.Business
{
    public class ReferralServiceTests
    {
        private readonly Mock<IReferralDataService> _mockReferralDataService;
        private readonly ReferralService _referralService;

        public ReferralServiceTests()
        {
            _mockReferralDataService = new Mock<IReferralDataService>();
            _referralService = new ReferralService(_mockReferralDataService.Object);
        }

        [Fact]
        public void Get_ShouldReturnReferrals()
        {
            // Arrange
            var userId = 1;
            var referrals = new List<ReferralModel> { new ReferralModel() };
            _mockReferralDataService.Setup(x => x.Get(userId)).Returns(referrals);

            // Act
            var result = _referralService.Get(userId);

            // Assert
            Assert.Equal(referrals, result);
        }

        [Fact]
        public void Create_ShouldReturnReferralId()
        {
            // Arrange
            var model = new ReferralModel 
            {
                OriginatingUserId = 1,
                PhoneNumber = "555-444-1234",
                Status = ReferralStatusesEnum.Created
            };
            _mockReferralDataService.Setup(x => x.Get(It.IsAny<int>())).Returns(new List<ReferralModel>());
            _mockReferralDataService.Setup(x => x.Create(model)).Returns(1);

            // Act
            var result = _referralService.Create(model);

            // Assert
            Assert.Equal(1, result);
        }

        [Fact]
        public void Create_ShouldThrowValidationException_WhenMoreThan10ActiveReferrals()
        {
            // Arrange
            var model = new ReferralModel { OriginatingUserId = 1 }; 
            var referrals = Enumerable.Repeat(new ReferralModel { Status = ReferralStatusesEnum.Created }, 11); // simulate 11 active referrals
            _mockReferralDataService.Setup(x => x.Get(It.IsAny<int>())).Returns(referrals);

            // Act & Assert
            Assert.Throws<ValidationException>(() => _referralService.Create(model));
        }

        [Fact]
        public void Create_ShouldThrowValidationException_WhenPhoneNumberAndEmailMissing()
        {
            // Arrange
            var model = new ReferralModel { OriginatingUserId = 1 }; // missing phone number and email
            _mockReferralDataService.Setup(x => x.Get(It.IsAny<int>())).Returns(new List<ReferralModel>());
            _mockReferralDataService.Setup(x => x.Create(model)).Returns(1);

            // Act & Assert
            Assert.Throws<ValidationException>(() => _referralService.Create(model));
        }

        [Fact]
        public void Create_ShouldThrowValidationException_WhenCodeIsProvided()
        {
            // Arrange
            var model = new ReferralModel
            {
                OriginatingUserId = 1,
                PhoneNumber = "555-444-1234",
                Status = ReferralStatusesEnum.Created,
                Code = "123ABC" // Code should be generated on back end to prevent re use
            };
            _mockReferralDataService.Setup(x => x.Get(It.IsAny<int>())).Returns(new List<ReferralModel>());
            _mockReferralDataService.Setup(x => x.Create(model)).Returns(1);

            // Act & Assert
            Assert.Throws<ValidationException>(() => _referralService.Create(model));
        }

        [Fact]
        public void Update_ShouldReturnReferralId()
        {
            // Arrange
            var referralModel = new ReferralModel
            {
                ReferralId = 1,
                OriginatingUserId = 123,
                Code = "ABC123",
                PhoneNumber = "123-456-7890",
                EmailAddress = "test@example.com",
                Status = ReferralStatusesEnum.Sent,
                LastSent = DateTime.UtcNow
            };

            _mockReferralDataService.Setup(x => x.Update(referralModel)).Returns(referralModel.ReferralId);

            // Act
            var result = _referralService.Update(referralModel);

            // Assert
            Assert.Equal(referralModel.ReferralId, result);
        }

        [Fact]
        public void Update_ShouldThrowValidationException_WhenOriginatingUserIdIsMissing()
        {
            // Arrange
            var referralModel = new ReferralModel
            {
                ReferralId = 1,
                OriginatingUserId = 0, // Invalid OriginatingUserId
                Code = "ABC123",
                PhoneNumber = "123-456-7890",
                EmailAddress = "test@example.com",
                Status = ReferralStatusesEnum.Sent,
                LastSent = DateTime.UtcNow
            };

            // Act & Assert
            Assert.Throws<ValidationException>(() => _referralService.Update(referralModel));
        }

        [Fact]
        public void Update_ShouldThrowValidationException_WhenDoesNotExistInDataSet()
        {
            // Arrange
            var referralModel = new ReferralModel
            {
                ReferralId = 1,
                OriginatingUserId = 123,
                Code = "ABC123",
                PhoneNumber = "123-456-7890",
                EmailAddress = "test@example.com",
                Status = ReferralStatusesEnum.Sent,
                LastSent = DateTime.UtcNow
            };

            _mockReferralDataService.Setup(x => x.Update(referralModel)).Returns(0); // Simulate update failure

            // Act & Assert
            var exception = Assert.Throws<ValidationException>(() => _referralService.Update(referralModel));
        }

        [Fact]
        public void Update_ShouldThrowValidationException_WhenStatusIsGreaterThanSent()
        {
            // Arrange
            var referralModel = new ReferralModel
            {
                ReferralId = 1,
                OriginatingUserId = 123,
                Code = "ABC123",
                PhoneNumber = "123-456-7890",
                EmailAddress = "test@example.com",
                Status = ReferralStatusesEnum.UserCreated,
                LastSent = DateTime.UtcNow
            };

            _mockReferralDataService.Setup(x => x.Update(referralModel)).Returns(0); // Simulate update failure

            // Act & Assert
            Assert.Throws<ValidationException>(() => _referralService.Update(referralModel));
        }

        [Fact]
        public void Delete_ShouldReturnTrue()
        {
            // Arrange
            var referralId = 1;

            _mockReferralDataService.Setup(x => x.Delete(referralId)).Returns(true);

            // Act
            var result = _referralService.Delete(referralId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Delete_ShouldThrowValidationException_WhenReferralIsNotInDataSet()
        {
            // Arrange
            var referralId = 1;

            _mockReferralDataService.Setup(x => x.Delete(referralId)).Returns(false);

            // Act & Assert
            Assert.Throws<ValidationException>(() => _referralService.Delete(referralId));
        }

        [Fact]
        public void Resend_ShouldReturnTrue()
        {
            // Arrange
            var referralId = 1;
            var model = new ReferralModel
            {
                OriginatingUserId = 1,
                ReferralId = referralId,
                PhoneNumber = "555-444-1234",
                Status = ReferralStatusesEnum.Created,
            };
            _mockReferralDataService.Setup(x => x.GetById(It.IsAny<int>())).Returns(model);

            _mockReferralDataService.Setup(x => x.Update(model)).Returns(referralId);

            // Act
            var result = _referralService.Resend(referralId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Resend_ShouldThrowValidationException_WhenStatusIGreaterThanSent()
        {
            // Arrange
            var referralId = 1;
            var model = new ReferralModel
            {
                OriginatingUserId = 1,
                ReferralId = referralId,
                PhoneNumber = "555-444-1234",
                Status = ReferralStatusesEnum.UserCreated, // Invalid status for resend
            };
            _mockReferralDataService.Setup(x => x.GetById(It.IsAny<int>())).Returns(model);

            _mockReferralDataService.Setup(x => x.Update(model)).Returns(referralId);

            // Act & Assert
            Assert.Throws<ValidationException>(() => _referralService.Resend(referralId));
        }

        [Fact]
        public void Resend_ShouldThrowValidationException_WhenSentWithinLastHour()
        {
            // Arrange
            var referralId = 1;
            var model = new ReferralModel
            {
                OriginatingUserId = 1,
                ReferralId = referralId,
                PhoneNumber = "555-444-1234",
                Status = ReferralStatusesEnum.Sent,
                LastSent = DateTime.UtcNow.AddMinutes(-10)
            };
            _mockReferralDataService.Setup(x => x.GetById(It.IsAny<int>())).Returns(model);

            _mockReferralDataService.Setup(x => x.Update(model)).Returns(referralId);

            // Act & Assert
            Assert.Throws<ValidationException>(() => _referralService.Resend(referralId));
        }

        [Fact]
        public void CheckCode_ShouldReturnTrue()
        {
            // Arrange
            var referralId = 1;
            var model = new ReferralModel
            {
                OriginatingUserId = 1,
                ReferralId = referralId,
                Code = "123ABC",
                PhoneNumber = "555-444-1234",
                Status = ReferralStatusesEnum.Created,
            };
            _mockReferralDataService.Setup(x => x.CheckCode(It.IsAny<string>())).Returns(model);

            _mockReferralDataService.Setup(x => x.Update(model)).Returns(referralId);

            // Act
            var result = _referralService.CheckCode(model.Code);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CheckCode_ShouldReturnFalse()
        {
            // Arrange
            var referralId = 1;
            var model = new ReferralModel
            {
                OriginatingUserId = 1,
                ReferralId = referralId,
                Code = "123ABC",
                PhoneNumber = "555-444-1234",
                Status = ReferralStatusesEnum.Created,
            };
            _mockReferralDataService.Setup(x => x.CheckCode(It.IsAny<string>())).Returns(new ReferralModel()); // Unable to find code

            _mockReferralDataService.Setup(x => x.Update(model)).Returns(referralId);

            // Act
            var result = _referralService.CheckCode(model.Code);

            // Assert
            Assert.False(result);
        }
    }
}
