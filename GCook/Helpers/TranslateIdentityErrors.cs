namespace GCook.Helpers;

public static class TranslateIdentityErrors
{
    public static string TranslateErrorMessage(string codeError)
    {
        string message = codeError switch
        {
            "DefaultError" => "Ocorreu um erro desconhecido.",
            "ConcurrencyFailure" => "Falha de concorrência otimista, o objeto foi modificado.",
            "InvalidToken" => "Token inválido.",
            "LoginAlreadyAssociated" => "Já existe um usuário com este login",
            "InvalidUserName" => $"Este login é inválido, um login deve conter apenas letras ou digitos.",
            "InvalidEmail" => "E-mail inválido.",
            "DuplicateUserName" => "Este login já está sendo utilizado",
            "DuplicateEmail" => $"Este E-mail já está sendo utilizado",
            "InvalidRoleName" => "Está permissão é inválida",
            "DuplicateRoleName" => "Esta permissão já está sendo utulizada",
            "UserAlreadyInRole" => "Usuário já possui está permissão",
            "UserNotInRole" => "Usuário não tem está permissão",
            "UserLockoutNotEnabled" => "Lockout não está habilitado para este usuário",
            "UserAlreadyHasPassword" => "Usuário já possui uma senha definida.",
            "PassawordMismatch" => "Senha incorreta.",
            "PasswordTooShort" => "Senha muito curta.",
            "PasswordRequiresNonAlphanumeric" => "Senhas devem conter ao mesnos um caracter não alfanumérico.",
            "PassawordRequiresDigit" => "Senhas devem conter ao menos um digito ('0'-'9').",
            "PassawordRequiresLower" => "Senhas devem conter ao menos um caracter em caixa baixa ('a'-'z').",
            "PasswordRequiresUpper" => "Senhas devem conter ao menos um caracter em caixa alta ('A'-'Z').",
            _ => "Ocorreu um erro desconhecido."
        };
        return message;
    }
}