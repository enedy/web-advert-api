using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAdvert.Api.Services;
using WebAdvert.Models;

namespace WebAdvert.Api.Controllers
{
    [Route("api/v1/advert")]
    [ApiController]
    public class AdvertController : ControllerBase
    {
        private readonly IAdvertStorageService _advertStorageService;

        public AdvertController(IAdvertStorageService advertStorageService)
        {
            _advertStorageService = advertStorageService;
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
            catch(KeyNotFoundException)
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
    }
}
