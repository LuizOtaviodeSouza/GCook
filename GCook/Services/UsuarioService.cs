using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using GCook.Data;
using GCook.ViewModels;
using Microsoft.EntityFrameworkCore;
using GCook.Helpers;
using GCook.Models;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Text.Encodings.Web;


namespace GCook.Services;

public class UsuarioService : IUsuarioService
{
    private readonly AppDbContext _context;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserStore<IdentityUser> _userStore;
    private readonly IUserEmailStore<IdentityUser> _emailStore;
    private readonly IWebHostEnvironment _hostEnvironment;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<UsuarioService> _logger;

     
    public UsuarioService(
        AppDbContext context, 
        SignInManager<IdentityUser> signInManager, 
        UserManager<IdentityUser> userManager, 
        IHttpContextAccessor httpContextAccessor,
        IUserStore<IdentityUser> userStore,
        IWebHostEnvironment hostEnvironment,
        IEmailSender emailSender,
        ILogger<UsuarioService> logger
    )
    {
        _context = context;
        _signInManager = signInManager;
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
        _userStore = userStore;
        _emailStore = (IUserEmailStore<IdentityUser>)_userStore;
        _hostEnvironment = hostEnvironment;
        _emailSender = emailSender;
        _logger = logger;
    }

    public async Task<bool> ConfirmarEmail(string userId, string code)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return false;
        }
        code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        var result = await _userManager.ConfirmEmailAsync(user, code);
        return result.Succeeded;
    }

    public async Task<UsuarioVM> GetUsuarioLogado()
    {
       var usedId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
       if (userId == null)
       {
        return null;
       }
       var userAccount = await _userManager.FindByIdAsync(userId);
       var usuario = await _context.Usuarios.Where(u => u.UsuarioId == userId).SingleOrDefaultAsync();
       var perfis = string.Join(",", await _userManager.GetRolesAsync(userAccount));
       var admin = await _userManager.IsInRoleAsync(userAccount, "Adminstrador");
       UsuarioVM usuarioVM = new()
       {
        UsuarioId = userId,
        Nome = usuario.Nome,
        DataNascimento = usuario.DataNascimento,
        Foto = usuario.Foto,
        Email = userAccount.Email,
        UserName = userAccount.UserName,
        Perfil = perfis,
        IsAdmin = admin
       };
       return usuarioVM;
    }

    public async Task<SingInResult> LoginUsuario(LoginVM login)
    {
        string userName = login.EMail;
        if (HelperResult.IsValidEmail(login.Email))
        {
            var user = await _userManager.FindByEmailAsync(login.EMail);
            if (user != null)
            userName = user.UserName;
        }

    var result = await _singInManager.PasswordSingInAsync(
        userName, login.Senha, login.Lembrar, lockoutOnFailure: true
    );

    if (result.Succeeded)
        _logger.LogInformation($"Usuário {login.Email} acessou o sistema");
    if (result.IsLockedOut)
        _logger.LogWarning($"Usuário {login.Email} está bloqueado");

        return result;
    }

    public async Task LogoffUsuario()
    {
        _logger.LogInformation($"Usuário {ClaimTypes.Email} fez logoff");
        await _signInManager.SignOutAsync();
    }

    public async Task<List<string>> RegistrarUsuario(RegistroVm registro)
    {
        var user = Activator.CreateInstance<IdentityUser>();   

        await _userStore.SetUserNameAsync(user, registro.Email, CancellationToken.None);
        await _emailStore.SetEmailAsync(user, registro.Email, CancellationToken.None);
        var result = await _userManager.CreateAsync(user, registro.Senha);

        if (result.Secceeded)
        {
            _logger.LogInformation($"Novo usuário registrado com o email {user.Email}.");

            var userId = await _userManager.GetUserIdAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var url = $"http://localhost:5143/Account/ConfirmarEmail?userId={userId}&code={code}";

            await _userManager.AddToRoleAsync(user, "Usuário;");

            await _emailSender.SendEmailAsync(registro.Email, "GCokk - Creiação de Conta", GetConfirmEmailHtml(HtmlEncoder.Default.Encode(url)));

            // Cria a conta pessoal do usuário
            Usuario usuario = new()
            {
                UsuarioId = userId,
                DataNascimento = registro.DataNascimento ?? DateTime.Now,
                Nome = registro.Nome
            };
            if (registro.Foto != null)
            {
                string fileName = userId + Path.GetExtension(registro.Foto.FileName);
                string uploads = Path.Combine(_hostEnvironment.WebRootPath, @"img\usuarios");
                string newFIle = Path.Combine(uploads, fileName);
                using (var stream = new FileStream(newFile, FileMode.Create))
                {
                    registro.Foto.CopyTo(stream);
                }
                usuario.Foto = @"\img\usuarios\" + fileName;
            }
            _contexto.Add(usuario);
            await _contexto.SaveChangesAsync();

            return null;
        }

        List<string> errors = new();
        foreach (var error in result.Errors)
        {
            errors.Add(TranslateIdentityErrors.TranslateErrorMessage(error. Code));
        }
        return errors;
    }


    private string GetConfirmEmailHtml(string url)
    {
        
    }
}
