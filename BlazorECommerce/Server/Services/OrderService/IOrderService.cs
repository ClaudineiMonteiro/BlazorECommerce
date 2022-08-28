namespace BlazorECommerce.Server.Services.OrderService
{
    public interface IOrderService
    {
        Task<ServiceResponse<bool>> PlaceOrder();
        Task<ServiceResponse<List<OrderOverviewResponse>>> getOrders();
        Task<ServiceResponse<OrderDetailsResponse>> GetOrderDetails(int orderId);
    }
}
