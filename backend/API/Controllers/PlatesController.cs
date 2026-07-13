using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.DTOs;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PlatesController : ControllerBase
    {
        private readonly IPlateService _plateService;

        public PlatesController(IPlateService plateService)
        {
            _plateService = plateService;
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
        public async Task<IActionResult> Update(int id, PlateDTO dto)
        {
            try
            {
                await _plateService.UpdateAsync(id, dto);
                return NoContent();
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

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _plateService.DeleteAsync(id);
            return NoContent();
        }
    }
}
