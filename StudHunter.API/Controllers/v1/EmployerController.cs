﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudHunter.API.ModelsDto.Employer;
using StudHunter.API.Services;

namespace StudHunter.API.Controllers.v1;

[Route("api/v1/[controller]")]
[ApiController]
public class EmployerController(EmployerService employerService) : ControllerBase
{
    private readonly EmployerService _employerService = employerService;

    [HttpGet]
    public async Task<IActionResult> GetEmployers()
    {
        var employers = await _employerService.GetAllEmployersAsync();
        return Ok(employers);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetEmployer(Guid id)
    {
        var employer = await _employerService.GetEmployerAsync(id);
        if (employer == null)
            return NotFound();
        return Ok(employer);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateEmployer([FromBody] CreateEmployerDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var (employer, error) = await _employerService.CreateEmployerAsync(dto);
        if (error != null)
            return BadRequest(new { error });
        return CreatedAtAction(nameof(GetEmployer), new { id = employer!.Id }, employer);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateEmployer(Guid id, [FromBody] UpdateEmployerDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var (success, error) = await _employerService.UpdateEmployerAsync(id, dto);
        if (!success)
            return error == null ? NotFound() : BadRequest(new { error });
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> SoftDeleteEmployer(Guid id)
    {
        var (success, error) = await _employerService.DeleteEmployerAsync(id);
        if (!success)
            return error == null ? NotFound() : BadRequest(new { error });
        return NoContent();
    }
}
