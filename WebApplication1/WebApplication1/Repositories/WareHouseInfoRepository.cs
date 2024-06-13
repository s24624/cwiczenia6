using System.Data.SqlClient;
using WebApplication1.Models;

namespace WebApplication1.Repositories;

public class WareHouseInfoRepository : IWareHouseInfoRepository
{
    private IConfiguration _configuration;

    public WareHouseInfoRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<int> AddProductToWarehouse(WarehouseInfo warehouseInfo)
    {
        if (warehouseInfo.Amount <= 0)
            throw new ArgumentException("Amount must be greater than 0");

        await using var con = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
        await con.OpenAsync();
        await using var cmd = new SqlCommand();
        cmd.Connection = con;
        cmd.CommandText = "Select Count(*) from Product where IdProduct = @IdProduct";
        
        // Check if the product with the given id exists
        cmd.Parameters.AddWithValue("@IdProduct", warehouseInfo.IdProduct);
        if ((int)await cmd.ExecuteScalarAsync() == 0)
        {
            throw new ArgumentException("Product with this id does not exist");
        }

        cmd.CommandText = "SELECT Count(*) from Warehouse where IdWarehouse = @IdWarehouse";
        cmd.Parameters.AddWithValue("@IdWarehouse", warehouseInfo.IdWareHouse);

        // Check if the warehouse with the given id exists
        if ((int)await cmd.ExecuteScalarAsync() == 0)
        {
            throw new ArgumentException("Warehouse with this id does not exist");
        }

        cmd.CommandText = "SELECT Count(*) From [Order] where IdProduct = @IdProduct AND Amount = @Amount AND CreatedAt < @CreatedAt";
        cmd.Parameters.AddWithValue("@Amount", warehouseInfo.Amount);
        cmd.Parameters.AddWithValue("@CreatedAt", warehouseInfo.CreatedAt);
        if ((int)await cmd.ExecuteScalarAsync() == 0)
        {
            throw new ArgumentException("This order does not exist");
        }

        cmd.CommandText =
            "Select IdOrder from [Order] where IdProduct = @IdProduct AND Amount = @Amount AND CreatedAt<@CreatedAt";
        var idOrder = (int)await cmd.ExecuteScalarAsync();
        cmd.CommandText = "SELECT Price FROM Product WHERE IdProduct = @IdProduct";
        var price = await cmd.ExecuteScalarAsync();

        // Check if the order has already been fulfilled
        cmd.CommandText = "SELECT COUNT(*) FROM Product_Warehouse WHERE IdOrder = @IdOrder";
        cmd.Parameters.AddWithValue("@IdOrder", idOrder);
        if ((int)await cmd.ExecuteScalarAsync() != 0)
            throw new ArgumentException("Order has already been fulfilled");
        
        // Update the order
        cmd.CommandText = "UPDATE [Order] SET FulfilledAt = @FulfilledAt WHERE IdOrder = @IdOrder";
        cmd.Parameters.AddWithValue("@FulfilledAt", DateTime.UtcNow);
        await cmd.ExecuteNonQueryAsync();
        
        // Insert into Product_Warehouse
        var totalPrice = (decimal) warehouseInfo.Amount * (decimal)price;
        cmd.CommandText =
            "INSERT INTO Product_Warehouse (IdWarehouse, IdProduct, IdOrder, Amount, Price, CreatedAt)" +
            " VALUES (@IdWarehouse, @IdProduct, @IdOrder, @Amount, @Price, @CreatedAt)";
        cmd.Parameters.AddWithValue("@Price", totalPrice);
        cmd.Parameters["@CreatedAt"].Value = DateTime.UtcNow;
        var createdId = await cmd.ExecuteNonQueryAsync();
        return createdId;
    }
}
