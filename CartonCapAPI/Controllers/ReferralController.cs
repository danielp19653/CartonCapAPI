using CartonCapAPI.Business.Services.Interfaces;
using CartonCapAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CartonCapAPI.Controllers
{
    [ApiController]
    [Route("referrals")]
    public class ReferralController : ControllerBase
    {
        private readonly ILogger<ReferralController> _logger;
        private readonly IReferralService _referralService;

        public ReferralController(ILogger<ReferralController> logger, IReferralService referralService)
        {
            _logger = logger;
            _referralService = referralService;
        }

        [HttpGet]
        public IActionResult Get([FromQuery] int userId)
        {
            try
            {
                var result = _referralService.Get(userId);
                return Ok(result);
            }
            catch(ValidationException ex)
            {
                var response = Content(ex.ValidationMessage);
                response.StatusCode = 422;
                return response;
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] ReferralModel model)
        {
            try
            {
                var result = _referralService.Create(model);
                return Ok(result);
            }
            catch(ValidationException ex)
            {
                var response = Content(ex.ValidationMessage);
                response.StatusCode = 422;
                return response;
            }
        }

        [HttpPut]
        public IActionResult Update([FromBody] ReferralModel model)
        {
            try
            {
                var result = _referralService.Update(model);
                return Ok(result);
            }
            catch (ValidationException ex)
            {
                var response = Content(ex.ValidationMessage);
                response.StatusCode = 422;
                return response;
            }
        }

        [HttpDelete]
        public IActionResult Delete([FromQuery] int referralId)
        {
            try
            {
                var result = _referralService.Delete(referralId);
                return Ok(result);
            }
            catch (ValidationException ex)
            {
                var response = Content(ex.ValidationMessage);
                response.StatusCode = 422;
                return response;
            }
        }

        [Route("resend")]
        [HttpPost]
        public IActionResult Resend([FromQuery]int referralId)
        {
            try
            {
                var result = _referralService.Resend(referralId);
                return Ok(result);
            }
            catch (ValidationException ex)
            {
                var response = Content(ex.ValidationMessage);
                response.StatusCode = 422;
                return response;
            }
        }

        [Route("check-code")]
        [HttpGet]
        public IActionResult CheckCode([FromQuery]string referralCode)
        {
            try
            {
                var result = _referralService.CheckCode(referralCode);
                return Ok(result);
            }
            catch (ValidationException ex)
            {
                var response = Content(ex.ValidationMessage);
                response.StatusCode = 422;
                return response;
            }
        }
    }
}
