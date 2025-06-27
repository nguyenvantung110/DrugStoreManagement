using AutoMapper;
using drug_store_api.entities.Suppliers;
using drug_store_api.dtos.Suppliers;

namespace drug_store_api.systemcommon.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Supplier, SupplierDto>();
            CreateMap<SupplierDto, Supplier>();
            // ... other mapping
        }
    }
}
