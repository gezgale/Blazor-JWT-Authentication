using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BlazorFrontend.Models.Entities
{
    public class LoginRequest
    {
        [Display(Name = "آدرس ایمیل")]
        [Required(ErrorMessage = "ورود {0} الزامی است.")]
        [EmailAddress(ErrorMessage = "قالب {0} به درستی وارد نشده است.")]
        public string Email { get; set; } = string.Empty;
        [Display(Name = "کلمه عبور")]
        [Required(ErrorMessage = "ورود {0} الزامی می باشد.")]
        public string Password { get; set; } = string.Empty;
    }
}