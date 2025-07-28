namespace KubeFood.Order.API.Domain;

public enum OrderStatus
{
    Pending = 0,
    ProductValidationFailed = 1,
    StockUnavailable = 2,

    PaymentPending = 3,
    PaymentFailed = 4,
    PaymentConfirmed = 5,

    InPreparation = 6,
    ReadyForDelivery = 7,
    Delivered = 8,
    Completed = 9,

    CanceledByCustomer = 10,
    CanceledByRestaurant = 11,
    Expired = 12,
    Failed = 13,
}