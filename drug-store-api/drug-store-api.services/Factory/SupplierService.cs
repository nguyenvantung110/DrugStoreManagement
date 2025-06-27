using AutoMapper;
using drug_store_api.dtos.Suppliers;
using drug_store_api.entities.Suppliers;
using drug_store_api.repositories.IF;
using drug_store_api.services.IF;

namespace drug_store_api.services.Factory
{
    public class SupplierService : ISupplierService
    {
        private readonly ISupplierRepository _iSupplierRepository;
        private readonly IMapper _mapper;

        public SupplierService(ISupplierRepository iSuppierService, IMapper mapper)
        {
            _iSupplierRepository = iSuppierService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SupplierDto>> GetSuppliersAsync()
        {
            var res = await _iSupplierRepository.GetSuppliersAsync();
            List<SupplierDto> supplierResponseList = _mapper.Map<List<SupplierDto>>(res);
            return supplierResponseList;
        }

        public async Task<SupplierDto?> GetSupplierByIdAsync(int id)
        {
            var res = await _iSupplierRepository.GetSupplierByIdAsync(id);
            var suppliersResponse = _mapper.Map<SupplierDto>(res);
            return suppliersResponse;
        }

        public async Task CreateSupplier(SupplierDto supplierDto)
        {
            try
            {
                supplierDto.Created_At = DateTime.UtcNow;
                supplierDto.Updated_At = DateTime.UtcNow;
                var supplierToCreate = this._mapper.Map<Supplier>(supplierDto);
                await _iSupplierRepository.CreateSupplierAsync(supplierToCreate);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task UpdateSupplierInfo(SupplierDto supplierDto)
        {
            supplierDto.Created_At = DateTime.UtcNow;
            supplierDto.Updated_At = DateTime.UtcNow;
            var supplierToUpdate = this._mapper.Map<Supplier>(supplierDto);
            await _iSupplierRepository.UpdateSupplierAsync(supplierToUpdate);
        }

        public async Task DeleteSupplierAsync(Guid supplierId)
        {
            await _iSupplierRepository.DeleteSupplierAsync(supplierId);
        }
    }
}
