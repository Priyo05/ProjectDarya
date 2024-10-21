using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Project.DataAccess.Models;
using Project.Presentation.Web.Models;
using System.Net.Mail;
using System.Security.Claims;

namespace Project.Presentation.Web.Controllers;

[Route("Auth")]
public class AuthController : Controller
{
    private readonly UserManager<Users> _userManager;
    private readonly SignInManager<Users> _signInManager;
    private readonly EmailService _emailService;

    public AuthController(UserManager<Users> userManager, SignInManager<Users> signInManager, EmailService emailService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailService = emailService;
    }

    [HttpGet("Register")]
    public IActionResult Register() => View();

    [HttpPost("Register")]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new Users
            {
                UserName = model.UserName,
                Email = model.Email,
                Fullname = model.FullName, 
                DateOfBirth = model.DateOfBirth 
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                // Generate email confirmation token
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = Url.Action("ConfirmEmail", "Auth", new { userId = user.Id, token }, Request.Scheme);

                // Kirim email konfirmasi
                await _emailService.SendEmailAsync(model.Email, "Email Confirmation",
                    $"Please confirm your account by <a href='{confirmationLink}'>clicking here</a>.");

                return RedirectToAction("RegisterConfirmation");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }
        return View(model);
    }

    [HttpGet("RegisterConfirmation")]
    public IActionResult RegisterConfirmation()
    {
        return View();
    }

    [HttpGet("ConfirmEmail")]
    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
        if (userId == null || token == null)
        {
            return RedirectToAction("Index", "Home");
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound($"Unable to load user with ID '{userId}'.");
        }


        // Konfirmasi email
        var result = await _userManager.ConfirmEmailAsync(user, token);

        if (result.Succeeded)
        {
            // Email berhasil dikonfirmasi
            return View("ConfirmEmail"); // Tampilkan tampilan konfirmasi email berhasil
        }
        else
        {
            // Email gagal dikonfirmasi
            return View("Error"); // Tampilkan tampilan error
        }
    }

    [HttpGet("Login")]
    public async Task<IActionResult> Login()
    {
        return View();
    }


    [HttpPost("Login")]
    public async Task<IActionResult> Login(LoginViewModel loginViewModel)
    {
        if (ModelState.IsValid)
        {
            // Cari user berdasarkan email
            var user = await _userManager.FindByEmailAsync(loginViewModel.Email);

            if (user != null)
            {
                // Cek apakah email sudah dikonfirmasi
                if (!user.EmailConfirmed)
                {
                    ModelState.AddModelError(string.Empty, "Akun belum diaktifkan. Silakan periksa email Anda dan klik tautan aktivasi.");
                    return View(loginViewModel);
                }

                // Melakukan autentikasi menggunakan SignInManager
                var result = await _signInManager.PasswordSignInAsync(user.UserName, loginViewModel.Password, loginViewModel.RememberMe, lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    // Autentikasi berhasil
                    return RedirectToAction("Index", "Home");
                }
                else if (result.IsLockedOut)
                {
                    // Akun terkunci karena terlalu banyak percobaan login yang gagal
                    ModelState.AddModelError(string.Empty, "Akun Anda telah terkunci. Silakan coba lagi nanti.");
                    return View(loginViewModel);
                }
                else
                {
                    // Autentikasi gagal
                    ModelState.AddModelError(string.Empty, "Percobaan login tidak valid.");
                    return View(loginViewModel);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Email atau kata sandi tidak valid.");
            }
        }

        return View(loginViewModel);
    }

    [HttpGet("ExternalLogin")]
    public IActionResult ExternalLogin(string provider, string returnUrl = null)
    {
        var redirectUrl = Url.Action("ExternalLoginCallback", "Auth", new { ReturnUrl = returnUrl });
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return Challenge(properties, provider);
    }

    [HttpGet("ExternalLoginCallback")]
    public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
    {
        returnUrl = returnUrl ?? Url.Content("~/");

        if (remoteError != null)
        {
            ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
            return RedirectToAction(nameof(Login));
        }

        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            return RedirectToAction(nameof(Login));
        }

        // Cek apakah user sudah ada di database berdasarkan external login
        var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

        if (result.Succeeded)
        {
            // User sudah login, langsung redirect ke halaman home
            return LocalRedirect(returnUrl);
        }
        else
        {
            // User baru, arahkan ke registrasi dengan informasi dari Google
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            return RedirectToAction(nameof(ExternalRegister), new { email });
        }
    }

    [HttpGet("ExternalRegister")]
    public IActionResult ExternalRegister(string email)
    {
        var model = new ExternalLoginViewModel
        {
            Email = email
        };
        return View(model);
    }

    [HttpPost("ExternalRegister")]
    public async Task<IActionResult> ExternalRegister(ExternalLoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return View("Error");
            }

            var user = new Users
            {
                UserName = model.Email,
                Email = model.Email,
                Fullname = model.FullName,
                DateOfBirth = model.DateOfBirth
            };

            var result = await _userManager.CreateAsync(user);
            if (result.Succeeded)
            {
                result = await _userManager.AddLoginAsync(user, info);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        return View(model);
    }





}

