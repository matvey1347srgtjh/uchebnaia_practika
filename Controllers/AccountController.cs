using CinemaApp.Models;
using CinemaApp.Repositories;
using CinemaApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;

namespace CinemaApp.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ITicketRepository _ticketRepository;
    private readonly IWebHostEnvironment _environment;

    private static readonly string[] _permittedExtensions = [".jpg", ".jpeg", ".png", ".gif", ".webp"];
    private const long MaxAvatarSizeBytes = 5 * 1024 * 1024; // 5 MB
    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ITicketRepository ticketRepository,
        IWebHostEnvironment environment)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _ticketRepository = ticketRepository;
        _environment = environment;
    }

    [HttpGet]
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
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName
            };

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

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (ModelState.IsValid)
        {
            var result = await _signInManager.PasswordSignInAsync(
                model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return RedirectToLocal(returnUrl);
            }

            ModelState.AddModelError(string.Empty, "Неверный email или пароль");
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

 [Authorize]
public async Task<IActionResult> Profile()
{
    var user = await _userManager.GetUserAsync(User);
    if (user == null)
    {
        return NotFound();
    }

    
    var allTickets = await _ticketRepository.GetTicketsByUserIdAsync(user.Id);

    
    var viewModel = new ProfileViewModel
    {
        User = user,
        
        
        Tickets = allTickets
            .Where(t => t.Status == TicketStatus.Sold)
            .OrderByDescending(t => t.PurchaseDate)
            .ToList(),
            
        
        ReservedTickets = allTickets
            .Where(t => t.Status == TicketStatus.Reserved && 
                        t.ReservedUntil.HasValue && 
                        t.ReservedUntil.Value > DateTime.Now) 
            .OrderBy(t => t.ReservedUntil) 
            .ToList()
    };

    return View(viewModel);
}

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateAvatar(IFormFile? avatar)
    {
        if (avatar == null || avatar.Length == 0)
        {
            TempData["AvatarError"] = "Выберите изображение для загрузки.";
            return RedirectToAction(nameof(Profile));
        }

        if (avatar.Length > MaxAvatarSizeBytes)
        {
            TempData["AvatarError"] = "Файл слишком большой. Максимальный размер — 5 МБ.";
            return RedirectToAction(nameof(Profile));
        }

        var extension = Path.GetExtension(avatar.FileName).ToLowerInvariant();
        if (!_permittedExtensions.Contains(extension))
        {
            TempData["AvatarError"] = "Допустимые форматы: JPG, PNG, GIF, WEBP.";
            return RedirectToAction(nameof(Profile));
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            TempData["AvatarError"] = "Не удалось определить пользователя.";
            return RedirectToAction(nameof(Profile));
        }

        var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "avatars");
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        var safeFileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(uploadsFolder, safeFileName);

        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await avatar.CopyToAsync(stream);
        }

        if (!string.IsNullOrWhiteSpace(user.AvatarPath))
        {
            var existingPath = Path.Combine(_environment.WebRootPath, user.AvatarPath.TrimStart(Path.DirectorySeparatorChar, '/').Replace('/', Path.DirectorySeparatorChar));
            if (System.IO.File.Exists(existingPath))
            {
                try
                {
                    System.IO.File.Delete(existingPath);
                }
                catch
                {
                    // Игнорируем ошибки удаления
                }
            }
        }

        user.AvatarPath = $"/uploads/avatars/{safeFileName}";
        await _userManager.UpdateAsync(user);

        TempData["AvatarSuccess"] = "Аватар успешно обновлён.";
        return RedirectToAction(nameof(Profile));
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateProfile(ProfileUpdateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["ProfileError"] = "Не удалось обновить данные. Проверьте введённую информацию.";
            return RedirectToAction(nameof(Profile));
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound();
        }

        var hasChanges = false;

        if (!string.Equals(user.FullName, model.FullName, StringComparison.Ordinal))
        {
            user.FullName = model.FullName;
            hasChanges = true;
        }

        if (!string.Equals(user.PhoneNumber, model.PhoneNumber, StringComparison.Ordinal))
        {
            user.PhoneNumber = model.PhoneNumber;
            hasChanges = true;
        }

        if (!string.Equals(user.UserName, model.UserName, StringComparison.Ordinal))
        {
            user.UserName = model.UserName;
            hasChanges = true;
        }

        if (!string.Equals(user.Email, model.Email, StringComparison.OrdinalIgnoreCase))
        {
            user.Email = model.Email;
            user.NormalizedEmail = _userManager.NormalizeEmail(model.Email);
            hasChanges = true;
        }

        if (!hasChanges)
        {
            TempData["ProfileNotice"] = "Изменений не обнаружено.";
            return RedirectToAction(nameof(Profile));
        }

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            TempData["ProfileError"] = string.Join(" ", result.Errors.Select(e => e.Description));
            return RedirectToAction(nameof(Profile));
        }

        TempData["ProfileSuccess"] = "Данные профиля обновлены.";
        return RedirectToAction(nameof(Profile));
    }

    [Authorize]
    [HttpGet]
    public IActionResult ChangePassword()
    {
        return View();
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound();
        }

        var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
        if (result.Succeeded)
        {
            await _signInManager.SignInAsync(user, isPersistent: false);
            ViewBag.SuccessMessage = "Пароль успешно изменен";
            return View();
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(model);
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

    private IActionResult RedirectToLocal(string? returnUrl)
    {
        if (Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }
        return RedirectToAction("Index", "Home");
    }
}

