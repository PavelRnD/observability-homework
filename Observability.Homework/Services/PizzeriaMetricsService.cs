using System.Diagnostics.Metrics;
using Observability.Homework.Models;

namespace Observability.Homework.Services;

public class PizzeriaMetricsService
{
    public static readonly string MeterName = "Observability.Metrics.Pizzeria";

    private const string ProductTypeMetricName = "pizzeria.product.type";
    private const string ProductBurntMetricName = "pizzeria.product.burnt";
    private const string ProductCancelMetricName = "pizzeria.product.cancel";
    private const string ProductCookingTimeMetricName = "pizzeria.product.cooking.time";
    private const string ProductCookingCountMetricName = "pizzeria.product.cooking.count";
    
    private readonly Counter<int> _productTypeCounter;
    private readonly Counter<int> _productBurntCounter;
    private readonly Counter<int> _productCancelCounter; 
    
    public PizzeriaMetricsService(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create(MeterName);
        _productTypeCounter = meter.CreateCounter<int>(ProductTypeMetricName);
        _productBurntCounter = meter.CreateCounter<int>(ProductBurntMetricName);
        _productCancelCounter = meter.CreateCounter<int>(ProductCancelMetricName);
    }

    public void ProductType(Product product)
    {
        _productTypeCounter.Add(1, new KeyValuePair<string, object?>[]
        {
            new("product.type", product.Type.ToString()),
        });
    }
    
    public void ProductBurnt()
    {
        _productBurntCounter.Add(1);
    }
    
    public void ProductCancel()
    {
        _productCancelCounter.Add(1);
    }
    
    public void RecordProductCooking(Product product, double cookingTime)
    {
        
    }
}