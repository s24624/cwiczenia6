using WebApplication1.Models;

namespace WebApplication1.Repositories;

public interface IWareHouseInfoRepository
{
    public Task<int> AddProductToWarehouse(WarehouseInfo warehouseInfo);
}