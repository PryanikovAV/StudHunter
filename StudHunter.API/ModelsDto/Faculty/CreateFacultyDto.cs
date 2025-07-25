﻿using System.ComponentModel.DataAnnotations;

namespace StudHunter.API.ModelsDto.Faculty;

public class CreateFacultyDto
{
    [Required(ErrorMessage = "{0} is required")]
    [StringLength(255, ErrorMessage = "{0} cannot exceed {1} characters")]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "{0} cannot exceed {1} characters")]
    public string? Description { get; set; }
}
