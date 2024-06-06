using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ksiegarnia.Models.DTO;
using ksiegarnia.Repositories.Abstract;
using ksiegarnia.Models.Domain;

namespace ksiegarnia.Controllers
{
    public class UserAuthenticationController : Controller
    {
        private IUserAuthenticationService authService;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UserAuthenticationController(IUserAuthenticationService authService, SignInManager<ApplicationUser> signInManager)
        {
            this.authService = authService;
            _signInManager = signInManager;
        }

        /* Metoda Register() została zakomentowana, ponieważ nie jest potrzebna 
           w kontekście logowania za pomocą Google. Możesz ją pominąć. */

        public async Task<IActionResult> Register()
        {
            var model = new RegistrationModel
            {
                Email = "lolkowskid@gmail.com",
                Username = "Maharadza",
                Name = "Maharadza",
                Password = "Problem@123",
                PasswordConfirm = "Problem@123",
                Role = "User"
            };
            var result = await authService.RegisterAsync(model);
            return Ok(result.Message);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await authService.LoginAsync(model);
            if (result.StatusCode == 1)
                return RedirectToAction("Index", "Home");
            else
            {
                TempData["msg"] = "Could not logged in..";
                return RedirectToAction(nameof(Login));
            }
        }

        public async Task<IActionResult> Logout()
        {
            await authService.LogoutAsync();
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            // Zwróć wyzwanie uwierzytelniające na zewnątrz do dostawcy
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "UserAuthentication", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                TempData["ErrorMessage"] = $"Error from external provider: {remoteError}";
                return RedirectToAction(nameof(Login));
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                TempData["ErrorMessage"] = "Error loading external login information.";
                return RedirectToAction(nameof(Login));
            }

            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            System.Diagnostics.Debug.WriteLine(result);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["ErrorMessage"] = "External login failed.";
                return RedirectToAction(nameof(Login));
            }
        }
    }
}
