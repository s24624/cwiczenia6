using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WarehouseInfoController : ControllerBase
{
    private IWarehouseInfoService _warehouseInfoService;

    public WarehouseInfoController(IWarehouseInfoService warehouseInfoService)
    {
        _warehouseInfoService = warehouseInfoService;
    }

    [HttpPost]
    public IActionResult AddProductToWarehouse(WarehouseInfo warehouseInfo)
    {
        var key = _warehouseInfoService.AddProductToWarehouse(warehouseInfo);
        return Ok(new { key });
    }
}