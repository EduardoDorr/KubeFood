namespace KubeFood.Delivery.Worker.Domain;

public enum OrderStatus
{
    PaymentPending = 5,
    PaymentFailed = 6,
    PaymentConfirmed = 7,

    InPreparation = 8,
    ReadyForDelivery = 9,
    Delivered = 10,
    Completed = 11,

    Expired = 14,
    Failed = 15,
}