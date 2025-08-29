using AutoMapper;
using drug_store_api.entities.Suppliers;
using drug_store_api.dtos.Suppliers;
using drug_store_api.dtos.Users;
using drug_store_api.entities.Users;
using drug_store_api.dtos.PurchaseOrders;
using drug_store_api.entities.Products;
using drug_store_api.entities.PurchaseOrderItems;
using drug_store_api.entities.PurchaseOrders;
using drug_store_api.dtos.Auth;
using drug_store_api.entities.PurchaseRequests;
using drug_store_api.dtos.PurchaseRequests;
using drug_store_api.dtos.Products;
using drug_store_api.dtos.PrescriptionTemplates;
using drug_store_api.entities.PrescriptionTemplates;
using drug_store_api.entities.PrescriptionTemplateItems;
using drug_store_api.dtos.Categories;
using drug_store_api.entities.Categories;
using drug_store_api.dtos.Customers;
using drug_store_api.entities.Customers;
using drug_store_api.dtos.Inventory;
using drug_store_api.entities.Inventory;
using drug_store_api.dtos.SaleOrders;
using drug_store_api.entities.SaleOrderItems;
using drug_store_api.entities.SalesOrders;

namespace drug_store_api.systemcommon.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Supplier mapping
            CreateMap<Supplier, SupplierDto>();
            CreateMap<SupplierDto, Supplier>();

            // User mapping
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();
            CreateMap<UserUpdateDto, User>()
                .ForMember(dest => dest.Username, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            // PurchaseRequest mapping
            CreateMap<PurchaseRequestByRequestDate, PurchaseRequestDto>()
                .ForMember(dest => dest.RequestDate, opt => opt.MapFrom(src =>
                    TimeZoneInfo.ConvertTimeFromUtc(
                        src.RequestDate.Kind == DateTimeKind.Utc
                            ? src.RequestDate
                            : DateTime.SpecifyKind(src.RequestDate, DateTimeKind.Utc),
                        TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")
                    ).ToString("yyyy-MM-dd HH:mm:ss")
                ));

            // PurchaseOrder mapping
            CreateMap<PurchaseOrderDto, PurchaseOrder>();
            CreateMap<PurchaseOrder, PurchaseOrderDto>();

            CreateMap<PurchaseOrderCreateDto, PurchaseOrder>()
                .ForMember(dest => dest.PurchaseId, opt => opt.Ignore())
                .ForMember(dest => dest.OrderDate, opt => opt.Ignore())
                .ForMember(dest => dest.TotalAmount, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            CreateMap<PurchaseOrderItemDto, PurchaseOrderItem>()
                .ForMember(dest => dest.PurchaseItemId, opt => opt.Ignore())
                .ForMember(dest => dest.PurchaseId, opt => opt.Ignore())
                .ForMember(dest => dest.SubTotal, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.QuantityReceived, opt => opt.Ignore());

            CreateMap<PurchaseOrder, PurchaseOrderDetailDto>();
            CreateMap<PurchaseOrderItem, PurchaseOrderItemDetailDto>();
            CreateMap<Product, ProductDto>();
            CreateMap<Product, ProductForPrescriptionDto>();
            CreateMap<ProductBasicInfo, ProductBasicInfoDto>();

            // Prescription mapping
            CreateMap<PrescriptionTemplateItem, PrescriptionTemplateItemDto>();
            //CreateMap<PrescriptionTemplate, PrescriptionTemplateDto>();
            CreateMap<PrescriptionTemplate, PrescriptionTemplateDto>()
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.User.Username));

            // Category mapping
            CreateMap<Category, CategoryDto>();
            CreateMap<Category, CategoryByTypeDto>();

            // Customer mapping
            CreateMap<CustomerForOrderDto, Customer>()
               .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.CustomerName));

            // Inventory mappings
            CreateMap<StockInDto, ProductInventory>()
                .ForMember(dest => dest.InventoryId, opt => opt.Ignore())
                .ForMember(dest => dest.CurrentStock, opt => opt.Ignore())
                .ForMember(dest => dest.ReorderLevel, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.MaxStockLevel, opt => opt.MapFrom(src => 1000))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.LastUpdated, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<ProductInventory, ProductInventoryDto>()
               .ForMember(dest => dest.IsLowStock, opt => opt.MapFrom(src => src.CurrentStock <= src.ReorderLevel))
               .ForMember(dest => dest.IsExpired, opt => opt.MapFrom(src => src.ExpiryDate.HasValue && src.ExpiryDate.Value <= DateTime.UtcNow))
               .ForMember(dest => dest.IsExpiringSoon, opt => opt.MapFrom(src => src.ExpiryDate.HasValue && src.ExpiryDate.Value <= DateTime.UtcNow.AddDays(30)))
               .ForMember(dest => dest.TotalValue, opt => opt.MapFrom(src => src.UnitCost.HasValue ? src.UnitCost.Value * src.CurrentStock : (decimal?)null))
               .ForMember(dest => dest.DaysUntilExpiry, opt => opt.MapFrom(src => src.ExpiryDate.HasValue ? (int)(src.ExpiryDate.Value - DateTime.UtcNow).TotalDays : (int?)null));

            // SalesOrder entity mappings
            //CreateMap<SalesOrder, SalesOrderDto>()
            //    .ForMember(dest => dest.CustomerName, opt => opt.Ignore()) // Will be populated from join
            //    .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

            CreateMap<SaleOrderItem, SaleOrderItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.Ignore()); // Will be populated from join

            CreateMap<OrderCreateDto, SalesOrder>()
                .ForMember(dest => dest.OrderId, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => SalesOrderStatusEnum.Completed))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<ProductForOrderDto, SaleOrderItem>()
                .ForMember(dest => dest.OrderId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<SalesOrder, SaleOrderResponseDto>()
                .ForMember(dest => dest.Success, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.Message, opt => opt.MapFrom(src => "Order retrieved successfully"))
                .ForMember(dest => dest.InvoiceNumber, opt => opt.Ignore()) // Generate separately
                .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.FinalAmount))
                .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.CustomerName, opt => opt.Ignore()); // Handle from customer data

            // ============ CUSTOMER MAPPINGS ============

            CreateMap<CustomerForOrderDto, Customer>()
                .ForMember(dest => dest.CustomerId, opt => opt.Ignore())
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.CustomerName))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));
        }
    }
}
