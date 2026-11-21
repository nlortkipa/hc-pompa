using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.DTOs;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PlatesController : ControllerBase
    {
        private readonly IPlateService _plateService;
        private readonly IPumpOperationService _pumpOperationService;
        private readonly API.Services.ReviewEmailNotifier _reviewEmails;

        public PlatesController(
            IPlateService plateService,
            IPumpOperationService pumpOperationService,
            API.Services.ReviewEmailNotifier reviewEmails)
        {
            _plateService = plateService;
            _pumpOperationService = pumpOperationService;
            _reviewEmails = reviewEmails;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlateDTO>>> GetAll()
        {
            var plates = await _plateService.GetAllAsync();
            return Ok(plates);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<PlateDTO>> Create(PlateDTO dto)
        {
            try
            {
                var plate = await _plateService.CreateAsync(dto);
                return Ok(plate);
            }
            catch (Service.Exceptions.ConflictException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<PlateUpdateResultDTO>> Update(int id, PlateDTO dto)
        {
            PlateUpdateResultDTO result;
            try
            {
                result = await _plateService.UpdateAsync(id, dto);
            }
            catch (Service.Exceptions.ConflictException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            // Checkers newly assigned to the plate get the pending reports' review links.
            foreach (var group in result.AddedReviewRequests.GroupBy(r => r.PumpOperationId))
            {
                var operation = await _pumpOperationService.GetByIdAsync(group.Key, 0, "Admin");
                if (operation != null)
                {
                    await _reviewEmails.SendAsync(operation, group, warnIfNoChecker: false);
                }
            }

            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _plateService.DeleteAsync(id);
            return NoContent();
        }
    }
}
