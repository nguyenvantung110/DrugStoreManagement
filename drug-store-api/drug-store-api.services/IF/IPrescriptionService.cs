using drug_store_api.dtos.PrescriptionTemplates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace drug_store_api.services.IF
{
    public interface IPrescriptionService
    {
        Task<IEnumerable<PrescriptionTemplateDto?>> GetPrescriptionTemplate(Guid? categoryId);
    }
}
