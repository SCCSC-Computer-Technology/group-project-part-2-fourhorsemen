using fourHorsemen_Online_Video_Game_Database.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using fourHorsemen_Online_Video_Game_Database.Models.ViewModels;
using fourHorsemen_Online_Video_Game_Database.Services;
using fourHorsemen_Online_Video_Game_Database.Data;
using Microsoft.EntityFrameworkCore;
using fourHorsemen_Online_Video_Game_Database.ViewModels;

namespace fourHorsemen_Online_Video_Game_Database.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<SiteUser> _signInManager;
        private readonly UserManager<SiteUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly GameDBContext _context;

        public AccountController(SignInManager<SiteUser> signInManager, UserManager<SiteUser> userManager, IEmailSender emailSender, GameDBContext context)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _emailSender = emailSender;
            _context = context;

        }

        public IActionResult Login()
        {
            ViewBag.FunFact = FunFactsService.GetRandomFact();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    return RedirectToLocal(returnUrl);
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt");

            }
            return View(model);

        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if(Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {

                var user = new SiteUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }




        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null) return View("Error");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return View("Error");

            var result = await _userManager.ConfirmEmailAsync(user, token);
            return result.Succeeded ? View("ConfirmEmail") : View("Error");
        }

        //forget password get
        [HttpGet]
        public IActionResult ForgotPassword() => View();

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                return View("ForgotPasswordConfirmation");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action("ResetPassword", "Account", new { token, email = user.Email }, Request.Scheme);

            await _emailSender.SendEmailAsync(user.Email, "Reset Password",
                $"Click <a href='{callbackUrl}'>here</a> to reset your password.");

            return View("ForgotPasswordConfirmation");
        }

        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            if (token == null || email == null) return View("Error");
            return View(new ResetPasswordViewModel { Token = token, Email = email });
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return RedirectToAction("ResetPasswordConfirmation");

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
            return result.Succeeded ? View("ResetPasswordConfirmation") : View("Error");
        }

        public async Task<IActionResult> Favorites()
        {
            var userId = _userManager.GetUserId(User);
            var favorites = await _context.UserGames
                .Where(ug => ug.UserId == userId && ug.Category == GameCategory.Favorite)
                .Include(ug => ug.Game)
                .ToListAsync();

            ViewBag.Category = "Favorites";
            return View("UserGames", favorites);
        }

        public async Task<IActionResult> OwnedGames()
        {
            var userId = _userManager.GetUserId(User);
            var owned = await _context.UserGames
                .Where(ug => ug.UserId == userId && ug.Category == GameCategory.Owned)
                .Include(ug => ug.Game)
                .ToListAsync();

            ViewBag.Category = "Owned";
            return View("UserGames", owned);
        }

        public async Task<IActionResult> Wishlist()
        {
            var userId = _userManager.GetUserId(User);
            var wishlist = await _context.UserGames
                .Where(ug => ug.UserId == userId && ug.Category == GameCategory.Wishlist)
                .Include(ug => ug.Game)
                .ToListAsync();

            ViewBag.Category = "Wishlist";
            return View("UserGames", wishlist);
        }

        public async Task<IActionResult> DefeatedGames()
        {
            var userId = _userManager.GetUserId(User);
            var defeated = await _context.UserGames
                .Where(ug => ug.UserId == userId && ug.Category == GameCategory.Defeated)
                .Include(ug => ug.Game)
                .ToListAsync();

            ViewBag.Category = "Defeated";
            return View("UserGames", defeated);
        }

        public async Task<IActionResult> MyInfo()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var viewModel = new MyInfoViewModel
            {
                Username = user.UserName,
                Email = user.Email,
                JoinDate = user.JoinDate
            };

            return View(viewModel);
        }


    }
}
