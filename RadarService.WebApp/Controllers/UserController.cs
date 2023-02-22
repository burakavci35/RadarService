using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;
using RadarService.Authorization.Dtos;
using RadarService.Authorization.Services;


namespace RadarService.WebApp.Controllers
{
	[AllowAnonymous]
	public class UserController : Controller
	{
		private readonly IUserService _userSevice;

		public UserController(IUserService userSevice)
		{
			_userSevice = userSevice;
		}

		public IActionResult Register()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Register(RegisterDto registerDto)
		{
			if (ModelState.IsValid)
			{
				var loginResult = await _userSevice.Register(registerDto);

				if (loginResult == null) { return View(loginResult); }

				if (loginResult.IsSuccess)
					return RedirectToAction(nameof(Login));

				ModelState.AddModelError("RegistrationError", loginResult.Message);

			}

			return View(registerDto);
		}

		public IActionResult Login(string? returnUrl = null)
		{
			return View(new LoginDto() { ReturnUrl = returnUrl });
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(LoginDto loginDto)
		{
			if (ModelState.IsValid)
			{
				var loginResult = await _userSevice.Login(loginDto);

				if (loginResult == null) { return View(loginResult); }

				if (loginResult.IsSuccess)
					return loginDto.ReturnUrl == null ? RedirectToAction("Index", "Home") : Redirect(loginDto.ReturnUrl);

				ModelState.AddModelError("LoginError", loginResult.Message);

			}

			return View(loginDto);
		}

		public async Task<IActionResult> Logout()
		{
			await _userSevice.Logout();

			return RedirectToAction(nameof(Login));
		}
	}
}
