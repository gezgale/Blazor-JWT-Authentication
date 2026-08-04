using Asp.Versioning;
using AuthApi.Services;
using Microsoft.AspNetCore.Mvc;
namespace AuthApi.Controllers.v1;

[ApiVersion("1")]
public class UserManagementController : BaseController
{
    private readonly UserManagementService _service;
    public UserManagementController(UserManagementService service) { _service = service; }
    [HttpGet("GetUsers")]
    public IActionResult GetUsers() => Ok(_service.GetUsers());

    [HttpPut("UpdateUser")]
    public async Task<IActionResult> UpdateUser([FromBody] AuthApi.DTOs.UserManagementDto dto)
    {
        return Ok(dto);
    }
}