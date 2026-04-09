using BarberBoss.Domain.Enums;

namespace BarberBoss.Domain.Extensions;

public static class PaymentMethodExtensions
{
    public static string ConvertPaymentMethod(this PaymentMethod payment)
    {
        return payment switch
        {
            PaymentMethod.CreditCard => "Cartão de Crédito",
            PaymentMethod.DebitCard => "Cartão de Débito",
            PaymentMethod.Cash => "Dinheiro",
            PaymentMethod.Pix => "Pix",
            PaymentMethod.Other => "Outro",
            _ => string.Empty
        };
    }
}