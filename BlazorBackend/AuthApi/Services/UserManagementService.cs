using AuthApi.DTOs;
using AuthApi.Models;
using Microsoft.AspNetCore.Identity;

namespace AuthApi.Services;
public class UserManagementService
{
 private readonly UserManager<ApplicationUser> _userManager;
 public UserManagementService(UserManager<ApplicationUser> userManager){_userManager=userManager;}
 public IEnumerable<UserManagementDto> GetUsers()=>_userManager.Users.Select(x=>new UserManagementDto{
  Id=x.Id,FullName=x.FullName,Email=x.Email,UserName=x.UserName,EmailConfirmed=x.EmailConfirmed});
}
