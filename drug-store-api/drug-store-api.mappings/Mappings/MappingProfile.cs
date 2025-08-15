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
        }
    }
}
