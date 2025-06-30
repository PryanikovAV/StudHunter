using Microsoft.AspNetCore.Mvc;
using StudHunter.API.ModelsDto.Speciality;
using StudHunter.API.Services;

namespace StudHunter.API.Controllers.v1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class SpecialityController(SpecialityService specialityService) : ControllerBase
    {
        private readonly SpecialityService _specialityService = specialityService;

        [HttpGet]
        public async Task<IActionResult> GetSpecialities()
        {
            var specialities = await _specialityService.GetSpecialitiesAsync();
            return Ok(specialities);
        }
            
        [HttpGet("{id}")]
        public async Task<ActionResult> GetSpeciality(Guid id)
        {
            var speciality = await _specialityService.GetSpecialityAsync(id);
            if (speciality == null)
                return NotFound();
            return Ok(speciality);
        }
            
        [HttpPost]
        public async Task<IActionResult> CreateSpeciality([FromBody] SpecialityDto dto)
        {
            var (speciality, error) = await _specialityService.CreateSpecialityAsync(dto);
            if (error != null)
                return Conflict(new { error });
            return CreatedAtAction(nameof(GetSpeciality), new { id = speciality!.Id }, speciality);
        }
            
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSpeciality(Guid id, [FromBody] SpecialityDto dto)
        {
            var (success, error) = await _specialityService.UpdateSpecialityAsync(id, dto);
            if (!success)
                return error == null ? NotFound() : Conflict(new { error });
            return NoContent();
        }   
            
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSpeciality(Guid id)
        {
            var (success, error) = await _specialityService.DeleteSpecialityAsync(id); 
            if (!success)
                return error == null ? NotFound() : Conflict(new { error });
            return NoContent();
        }
    }
}
