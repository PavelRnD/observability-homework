using System.Collections.Concurrent;
using Observability.Homework.Exceptions;
using Observability.Homework.Models;

namespace Observability.Homework.Services;

public interface IPizzaBakeryService
{
    Task<Product> DoPizza(Product product, CancellationToken cancellationToken = default);
}

public class PizzaBakeryService : IPizzaBakeryService
{
    private readonly ILogger<PizzaBakeryService> _logger;
    private readonly ConcurrentDictionary<Guid, Product> _bake = new();

    public PizzaBakeryService(ILogger<PizzaBakeryService> logger)
    {
        _logger = logger;
    }

    public async Task<Product> DoPizza(Product product, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("DoPizza id:{productId} type:{productType}",product.Id, product.Type);
        try
        {
            await MakePizza(product, cancellationToken);
            await BakePizza(product, cancellationToken);
            await PackPizza(product, cancellationToken);
            return product;
        }
        catch (OperationCanceledException)
        {
            _logger.LogError("PizzaBakeryService cancel do pizza");
            DropPizza(product);
            throw;
        }
        catch (BurntPizzaException)
        {
            _logger.LogError("PizzaBakeryService burnt pizza");
            return await DoPizza(product, cancellationToken);
        }
    }

    private async Task<Product> BakePizza(Product product, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("BakePizza id:{productId} type:{productType}",product.Id, product.Type);
        PushToBake(product);
        var bakeForSeconds = new Random().Next(3, 9);
        await Task.Delay(TimeSpan.FromSeconds(bakeForSeconds), cancellationToken);
        if (bakeForSeconds > 7)
        {
            DropPizza(product);
            throw new BurntPizzaException("The pizza is burnt");
        }
        return PopFromBake(product);
    }

    private async Task<Product> MakePizza(Product product, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("MakePizza id:{productId} type:{productType}",product.Id, product.Type);
        await Task.Delay(new Random().Next(1, 3) * 1000, cancellationToken);
        return product;
    }
    
    private async Task<Product> PackPizza(Product product, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("PackPizza id:{productId} type:{productType}",product.Id, product.Type);
        await Task.Delay(new Random().Next(1, 2) * 1000, cancellationToken);
        return product;
    }

    private void PushToBake(Product product)
    {
        _logger.LogInformation("PushToBake id:{productId} type:{productType}",product.Id, product.Type);
        _bake[product.Id] = product;
    }

    private Product PopFromBake(Product product)
    {
        _logger.LogInformation("PopFromBake id:{productId} type:{productType}",product.Id, product.Type);
        _bake.Remove(product.Id, out var pizza);
        return pizza!; //пусть у нас всегда есть пицца
    }
    
    private void DropPizza(Product product)
    {
        _logger.LogInformation("DropPizza id:{productId} type:{productType}",product.Id, product.Type);
        _bake.Remove(product.Id, out _);
    }
}