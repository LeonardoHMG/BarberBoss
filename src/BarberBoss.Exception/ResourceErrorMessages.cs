namespace BarberBoss.Exception;
public static class ResourceErrorMessages
{
    public const string SERVICE_DATE_REQUIRED = "A data e a hora do serviço são obrigatórias.";
    public const string DATE_FUTURE = "A data e hora do serviço não podem ser futuras.";
    public const string DATE_TOO_OLD = "Não é permitido registros com mais de um ano.";
    public const string SERVICE_HOUR_INVALID = "O horário do serviço deve estar entre 06h e 23h.";

    public const string BARBER_NAME_REQUIRED = "O nome do barbeiro é obrigatório.";
    public const string BARBER_NAME_LENGTH = "O nome do barbeiro deve ter entre 2 e 80 caracteres.";

    public const string CLIENT_NAME_REQUIRED = "O nome do cliente é obrigatório.";
    public const string CLIENT_NAME_LENGTH = "O nome do cliente deve ter entre 2 e 120 caracteres.";

    public const string SERVICE_NAME_REQUIRED = "O nome do serviço é obrigatório.";
    public const string SERVICE_NAME_LENGTH = "O nome do serviço deve ter entre 2 e 120 caracteres.";

    public const string AMOUNT_NEGATIVE = "O valor do serviço prestado não pode ser negativo.";
    public const string AMOUNT_MUST_BE_ZERO = "Para cancelados, o valor do serviço deve ser 0.00.";
    public const string AMOUNT_REQUIRED = "O valor do serviço é obrigatório.";

    public const string PAYMENT_METHOD_INVALID = "Escolha um método de pagamento válido (Cartão, Dinheiro, Pix ou Outro).";
    public const string STATUS_INVALID = "O status deve ser Pago ou Cancelado.";
    public const string NOTES_LENGTH = "Observações não podem exceder 500 caracteres.";

    public const string BILLING_NOT_FOUND = "Faturamento não encontrado.";
    public const string BILLING_ALREADY_EXISTS = "Já existe um faturamento para este cliente neste serviço nesta data.";

    public const string PAGE_NUMBER_INVALID = "O número da página deve ser maior ou igual a 1.";
    public const string PAGE_SIZE_INVALID = "O tamanho da página deve estar entre 1 e 100.";
    public const string START_DATE_AFTER_END_DATE = "A data de início não pode ser posterior à data de término.";
    public const string MIN_AMOUNT_GREATER_THAN_MAX_AMOUNT = "O valor mínimo não pode ser maior que o valor máximo.";
    public const string MIN_AMOUNT_NEGATIVE = "O valor mínimo não pode ser negativo.";
    public const string MAX_AMOUNT_NEGATIVE = "O valor máximo não pode ser negativo.";
    public const string CLIENT_NAME_SEARCH_LENGTH = "O nome do cliente deve ter pelo menos 3 caracteres para a busca.";

    public const string NAME_EMPTY = "O nome não pode estar vazio.";
    public const string EMAIL_EMPTY = "O e-mail não pode estar vazio.";
    public const string EMAIL_INVALID = "O e-mail é inválido.";
    public const string INVALID_PASSWORD = "Sua senha deve ter no mínimo 8 caracteres, contendo pelo menos uma letra maiúscula, uma letra minúscula, um número e um caractere especial (por exemplo, !, ?, *, .).";
    public const string EMAIL_ALREADY_REGISTERED = "Esse e-mail já está sendo usado.";
    public const string EMAIL_OR_PASSWORD_INVALID = "E-mail e/ou senha inválidos.";

    public const string ADMIN_CANNOT_REGISTER_BILLING = "Administradores não podem registrar faturamento.";

    public const string UNAUTHORIZED = "Token inválido ou não informado. Faça login novamente.";
    public const string FORBIDDEN = "Você não tem permissão para acessar este recurso.";

    public const string PASSWORD_DIFFERENT_CURRENT_PASSWORD = "A senha inserida é diferente da senha atual.";

    public const string UNKNOWN_ERROR = "Ocorreu um erro desconhecido. Por favor, tente novamente mais tarde.";
}