using Microsoft.AspNetCore.Identity;

using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Mvc.RazorPages;

using Movie_Booking_App.Models;

using System.ComponentModel.DataAnnotations;

namespace Movie_Booking_App.Pages.Account

{

    public class RegisterModel : PageModel

    {

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly SignInManager<ApplicationUser> _signInManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public RegisterModel(UserManager<ApplicationUser> userManager,

                             SignInManager<ApplicationUser> signInManager,

                             RoleManager<IdentityRole> roleManager)

        {

            _userManager = userManager;

            _signInManager = signInManager;

            _roleManager = roleManager;

        }

        [BindProperty]

        public InputModel Input { get; set; } = new();

        public string? ReturnUrl { get; set; }

        public class InputModel

        {

            [Required]

            [Display(Name = "Full Name")]

            public string FullName { get; set; } = string.Empty;

            [Required]

            [EmailAddress]

            [Display(Name = "Email")]

            public string Email { get; set; } = string.Empty;

            [Required]

            [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters.")]

            [DataType(DataType.Password)]

            [Display(Name = "Password")]

            public string Password { get; set; } = string.Empty;

            [DataType(DataType.Password)]

            [Display(Name = "Confirm Password")]

            [Compare("Password", ErrorMessage = "Passwords do not match.")]

            public string ConfirmPassword { get; set; } = string.Empty;

        }

        public void OnGet(string? returnUrl = null)

        {

        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)

        {

            returnUrl ??= Url.Content("~/");

            if (!ModelState.IsValid)

                return Page();

            // Ensure "User" role exists

            if (!await _roleManager.RoleExistsAsync("User"))

                await _roleManager.CreateAsync(new IdentityRole("User"));

            var user = new ApplicationUser

            {

                UserName = Input.Email,

                Email = Input.Email,

                FullName = Input.FullName

            };

            var result = await _userManager.CreateAsync(user, Input.Password);

            if (result.Succeeded)

            {

                // Assign default "User" role

                await _userManager.AddToRoleAsync(user, "User");

                await _signInManager.SignInAsync(user, isPersistent: false);

                return LocalRedirect(returnUrl);

            }

            foreach (var error in result.Errors)

                ModelState.AddModelError(string.Empty, error.Description);

            return Page();

        }

    }

}

