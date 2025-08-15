namespace drug_store_api.dtos.Products
{
    public class ProductBasicInfoDto
    {
        public Guid ProductId { get; set; }
        public required string ProductName { get; set; }
        public required string Manufacturer { get; set; }
        public required decimal PricePerUnit { get; set; }
        public required string UnitOfMeasure { get; set; }
        public string? Barcode { get; set; }
    }
}