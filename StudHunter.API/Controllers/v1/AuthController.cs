using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudHunter.DB.Postgres;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using StudHunter.API.Services;
using StudHunter.API.ModelsDto.LoginRequest;
using StudHunter.DB.Postgres.Models;
using StudHunter.API.ModelsDto.Student;

namespace StudHunter.API.Controllers.v1;

[Route("api/v1/[controller]")]
[ApiController]
public class AuthController(StudHunterDbContext context, IConfiguration configuraion, IPasswordHasher passwordHasher) : ControllerBase
{
    private readonly StudHunterDbContext _context = context;
    private readonly IConfiguration _configuration = configuraion;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;

    /// <summary>
    /// Authenticates a user (student, employer, or administrator) and returns a JWT token.
    /// </summary>
    /// <param name="request">The login request containing email and password.</param>
    /// <returns>A JWT token with user role if authentication is successful; otherwise, an error.</returns>
    /// <response code="200">Returns the JWT token and user role.</response>
    /// <response code="401">Unauthorized if credentials are invalid.</response>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        //var student = await _context.Students.FirstOrDefaultAsync(s => s.Email == request.Email);
        //var employer = student == null ? await _context.Employers.FirstOrDefaultAsync(e => e.Email == request.Email) : null;
        //var administrator = student == null && employer == null ? await _context.Administrators.FirstOrDefaultAsync(a => a.Email == request.Email) : null;

        User? user = null;
        string? role = null;

        //if (student != null)
        //{
        //    user = student;
        //    role = "Student";
        //}
        //else if (employer != null)
        //{
        //    user = employer;
        //    role = "Employer";
        //}
        //else if (administrator != null)
        //{
        //    user = administrator;
        //    role = "Administrator";
        //}

        if (user == null || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
            return Unauthorized(new { error = "Invalid email or password" });

        if (role == null)
            throw new InvalidOperationException("Role is not assigned");

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.Email),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Role, role)
        };

        var jwtKey = _configuration["Jwt:Key"];
        if (string.IsNullOrEmpty(jwtKey))
            throw new InvalidOperationException("JWT Key is not configured");

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                SecurityAlgorithms.HmacSha256));

        return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token), role });
    }

    //[HttpPost("register")]
    //[ProducesResponseType(StatusCodes.Status201Created)]
    //[ProducesResponseType(StatusCodes.Status400BadRequest)]
    //public async Task<IActionResult> Register([FromBody] CreateStudentDto dto)
    //{
    //    if (!ModelState.IsValid)
    //        return BadRequest(ModelState);

    //    var (student, error) = await _studentService.CreateStudentAsync(dto);
    //    if (student == null)
    //        return BadRequest(new { error });

    //    return CreatedAtAction(nameof(StudentController.GetStudent), new { id = student.Id }, student);
    //}
}
