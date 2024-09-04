using System.ComponentModel.DataAnnotations;

namespace GCook.ViewModels;
public class LoginVM 
 {
    [Display(Name = "Email ou nome do Usuário", Prompt = "Email ou nome do Usuario")]
    [Required(ErrorMessage = "Por favor, infome o email ou nome do usuário")]

    public string EMail { get; set; }   

    [DataType(DataType.Password)]
    [Display(Name = "Senha de Acesso", Prompt = "Senha de Acesso")]
    [Required(ErrorMessage = "Por favor, infome a senha de acesso")]

    public string Senha { get; set; }

    [Display(Name = "Manter Conectado?")]

    public bool Lembrar { get; set; } = false;

    public string UrlRetorno { get; set; }   
    
 }