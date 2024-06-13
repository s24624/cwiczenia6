using WebApplication1.Models;

namespace WebApplication1.Services;

public interface IWarehouseInfoService
{
    public Task<int> AddProductToWarehouse(WarehouseInfo warehouseInfo);
}