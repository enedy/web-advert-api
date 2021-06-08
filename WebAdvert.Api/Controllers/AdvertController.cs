using Amazon.SimpleNotificationService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAdvert.Api.Services;
using WebAdvert.Models;
using WebAdvert.Models.Messages;

namespace WebAdvert.Api.Controllers
{
    [Route("api/v1/advert")]
    [ApiController]
    public class AdvertController : ControllerBase
    {
        private readonly IAmazonSimpleNotificationService _amazonSimpleNotificationService;
        private readonly IConfiguration _configuration;
        private readonly IAdvertStorageService _advertStorageService;

        public AdvertController(IAdvertStorageService advertStorageService,
            IAmazonSimpleNotificationService amazonSimpleNotificationService, IConfiguration configuration)
        {
            _advertStorageService = advertStorageService;
            _amazonSimpleNotificationService = amazonSimpleNotificationService;
            _configuration = configuration;
        }

        [Route("create"), HttpPost]
        [ProducesResponseType(400)]
        [ProducesResponseType(typeof(CreateAdvertResponse), 201)]
        public async Task<IActionResult> Create(AdvertModel model)
        {
            var recordId = string.Empty;

            try
            {
                recordId = await _advertStorageService.Add(model);
            }
            catch (KeyNotFoundException)
            {
                return new NotFoundResult();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

            return StatusCode(201, new CreateAdvertResponse { Id = recordId });
        }

        [Route("confirm"), HttpPut]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Confirm(ConfirmAdvertModel model)
        {
            var recordId = string.Empty;

            try
            {
                await _advertStorageService.Confirm(model);
                await RaiseAdvertConfirmedMessage(model);
            }
            catch (KeyNotFoundException)
            {
                return new NotFoundResult();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

            return new OkResult();
        }

        private async Task RaiseAdvertConfirmedMessage(ConfirmAdvertModel model)
        {
            var topicArn = _configuration.GetValue<string>("TopicArn");
            var dbModel = await _advertStorageService.GetByIdAsync(model.Id);
            await _amazonSimpleNotificationService.PublishAsync(topicArn,
                JsonConvert.SerializeObject(new AdvertConfirmedMessage
                {
                    Id = model.Id,
                    Title = dbModel.Title
                }));
        }
    }
}
