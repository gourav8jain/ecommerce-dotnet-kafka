namespace ECommerce.Events;

public class ProductCreatedEvent : BaseEvent
{
    public Guid ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string Category { get; set; } = string.Empty;

    public ProductCreatedEvent(Guid productId, string name, string description, decimal price, int stockQuantity, string category)
        : base(productId.ToString())
    {
        ProductId = productId;
        Name = name;
        Description = description;
        Price = price;
        StockQuantity = stockQuantity;
        Category = category;
    }
}

public class ProductUpdatedEvent : BaseEvent
{
    public Guid ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string Category { get; set; } = string.Empty;

    public ProductUpdatedEvent(Guid productId, string name, string description, decimal price, int stockQuantity, string category)
        : base(productId.ToString())
    {
        ProductId = productId;
        Name = name;
        Description = description;
        Price = price;
        StockQuantity = stockQuantity;
        Category = category;
    }
}

public class ProductStockUpdatedEvent : BaseEvent
{
    public Guid ProductId { get; set; }
    public int NewStockQuantity { get; set; }
    public int OldStockQuantity { get; set; }

    public ProductStockUpdatedEvent(Guid productId, int newStockQuantity, int oldStockQuantity)
        : base(productId.ToString())
    {
        ProductId = productId;
        NewStockQuantity = newStockQuantity;
        OldStockQuantity = oldStockQuantity;
    }
}

public class ProductDeletedEvent : BaseEvent
{
    public Guid ProductId { get; set; }

    public ProductDeletedEvent(Guid productId)
        : base(productId.ToString())
    {
        ProductId = productId;
    }
} 