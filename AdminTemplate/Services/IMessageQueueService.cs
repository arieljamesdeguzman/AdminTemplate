using AdminTemplate.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdminTemplate.Services
{
    public interface IMessageQueueService
    {
        Task PublishSupplierAsync(SupplierDto supplier);
        Task PublishSuppliersAsync(List<SupplierDto> suppliers);
    }
}