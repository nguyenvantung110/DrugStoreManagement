using drug_store_api.entities.PrescriptionTemplates;

namespace drug_store_api.repositories.IF
{
    public interface IPrescriptionRepository
    {
        Task<IEnumerable<PrescriptionTemplate?>> GetPrescriptionTemplate(Guid? categoryId);
    }
}
