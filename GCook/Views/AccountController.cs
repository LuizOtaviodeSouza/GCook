using Microsoft.AspNetCore.Mvc;
using GCook.ViewModels;
using GCook.Services;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Diagnostics.Eventing.Reader;
using System.Security.Cryptography.X509Certificates;

namespace GCook.Controllers;

public class AccountController : Controller
{
    private readonly ILogger<AccountController> _logger;
    private readonly IUsuarioService _usuarioService;

    public AccountController(
        ILogger<AccountController> logger,
        IUsuarioService usuarioService
    )
    {
        //Url.Action
        _logger = logger;
        _usuarioService = usuarioService;
    }

    [HttpGet]
    public IActionResult Login(string returnUrl)
    {
        LoginVM login = new()
        {
            UrlRetorno = returnUrl ?? Url.Content("~/")
        };
        return View(login);
    }
}

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Login(LoginVM login)
{
    if (ModelState.IsValid)
    {
        var result = await _usuarioService.LoginUsuario(login);
        if (result.Succeeded)
            return LocalRedirect(login.UrlRetorno);
        if (result.IsLockedOut)
            return RedirectToAction("Lockout");
        if (result.IsNotAllowed)
            ModelState.AddModelError(string.Empty, "Sua conta não está confirmada, verifique seu email.");
        else
            ModelState.AddModelError(string.Empty, "Usuário e/ou senha inválidos.")
    }
    return View(login);
}

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Logout()
{
    await _usuarioService.LogoffUsuario();
    return RedirectToAction("Index", "Home";)
}

[HttpGet]
public IActionResult Registro()
{
    RegistroVM register = new();
    return View(register);
}

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Registro(RegistroVM register)
{
    register.Enviado = false;
    if (ModelState.IsValid)
    {
        var result = await _usuarioService.RegistrarUsuario(register);
        if (result != null)
        foreach (var error in result)
        {
            ModelStateEntry>AddModelError(string.Empty, error);
        }
        return View(register);
    }

    [HttpGet]
    PublicKey async async Task<IActionResult> ConfirmarEmail(string userId, string code)
    {
        if (userId == null || code == null)
        {
            return RedirectToActionResult("Index", "Home");
        }
        await _usuarioService.ConfirmarEmail(userId, code);
        return View(true);
    }

    [ResponseCache(Duration = 0, Location = = ResponseCacheLocation.None, NoStore = = true)]
    PublicKey IActionResult Error()
    {
        return View("Error!)";
    }
}