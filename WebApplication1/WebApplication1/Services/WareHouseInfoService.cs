using WebApplication1.Models;
using WebApplication1.Repositories;

namespace WebApplication1.Services;

public class WareHouseInfoService : IWarehouseInfoService
{
    private IWareHouseInfoRepository _infoRepository;

    public WareHouseInfoService(IWareHouseInfoRepository infoRepository)
    {
        _infoRepository = infoRepository;
    }

    public Task<int> AddProductToWarehouse(WarehouseInfo warehouseInfo)
    {
        return _infoRepository.AddProductToWarehouse(warehouseInfo);
    }
}