using System.Security.Claims;
using Humanizer;
using IdentityModel;
using IdentityService.Models;
using IdentityService.Pages.Ciba;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace IdentityService.Pages.Account.Register
{
    [AllowAnonymous]
    [SecurityHeaders]
    
    public class Index : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public Index(UserManager<ApplicationUser> userManager) {
            _userManager = userManager;
        }

        [BindProperty]
        public RegisterViewModel Input {get; set;}

        [BindProperty]
        public bool RegisterSuccess {get; set;}
        public IActionResult OnGet([FromQuery] string returnUrl)
        {
            Input = new RegisterViewModel{
                ReturnUrl = returnUrl,
            };

            return Page();
        }


        public async Task<IActionResult> OnPost() {
            if (Input.ButtonText != "register") RedirectToPage("~/");

            if (ModelState.IsValid) {
                var user = new ApplicationUser{
                    UserName = Input.Username,
                    Email  = Input.Email,
                    // we use fake emails so normally this would not be like this
                    EmailConfirmed = true,
                };

                // create the user
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded) {
                    // if succeeds, create claims for the user
                    await _userManager.AddClaimsAsync(user, new Claim[] 
                    {
                        // claim for the fullname
                        new(JwtClaimTypes.Name, Input.Fullname)
                    });
                    RegisterSuccess = true;
                }

            }
            return Page();
        }
    }
}
