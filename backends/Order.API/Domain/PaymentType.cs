namespace KubeFood.Order.API.Domain;

public enum PaymentType
{
    CreditCard = 0,
    DebitCard = 1,
    Pix = 2,
    Voucher = 3,
    Cash = 4,
    Other = 5
}