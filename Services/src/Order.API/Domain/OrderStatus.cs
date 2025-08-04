namespace KubeFood.Order.API.Domain;

public enum OrderStatus
{
    Pending = 0,
    ProductsValidated = 1,
    ProductsInvalid = 2,

    StockAvailable = 3,
    StockUnavailable = 4,

    PaymentPending = 5,
    PaymentFailed = 6,
    PaymentConfirmed = 7,

    InPreparation = 8,
    ReadyForDelivery = 9,
    Delivered = 10,
    Completed = 11,

    CanceledByCustomer = 12,
    CanceledByRestaurant = 13,
    Expired = 14,
    Failed = 15,
}