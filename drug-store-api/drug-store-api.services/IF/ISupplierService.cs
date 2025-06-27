using drug_store_api.dtos.Suppliers;

namespace drug_store_api.services.IF
{
    public interface ISupplierService
    {
        /// <summary>
        /// Get list supplier
        /// </summary>
        /// <returns>List supplier</returns>
        Task<IEnumerable<SupplierDto>> GetSuppliersAsync();

        /// <summary>
        /// Get supplier info by Id
        /// </summary>
        /// <param name="id">Id of supplier</param>
        /// <returns>Supplier info</returns>
        Task<SupplierDto?> GetSupplierByIdAsync(int id);

        /// <summary>
        /// Create supplier
        /// </summary>
        /// <param name="supplierDto">Supplier info to create</param>
        /// <returns></returns>
        Task CreateSupplier(SupplierDto supplierDto);

        /// <summary>
        /// Update supplier info
        /// </summary>
        /// <param name="supplierDto">Supplier info to update</param>
        /// <returns></returns>
        Task UpdateSupplierInfo(SupplierDto supplierDto);

        /// <summary>
        /// Delete supplier
        /// </summary>
        /// <param name="id">Id of supplier</param>
        /// <returns></returns>
        Task DeleteSupplierAsync(Guid supplierId);
    }
}
