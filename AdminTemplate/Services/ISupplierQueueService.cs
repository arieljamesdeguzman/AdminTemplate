using AdminTemplate.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdminTemplate.Services
{
    public interface ISupplierQueueService
    {
        Task<bool> QueueSupplierAsync(SupplierDto supplier);
        Task<bool> QueueSuppliersAsync(List<SupplierDto> suppliers);
        QueueStatus GetQueueStatus();
    }

    public class QueueStatus
    {
        public int QueuedCount { get; set; }
        public int ProcessedCount { get; set; }
        public int FailedCount { get; set; }
        public int TotalQueued { get; set; }
        public List<string> RecentErrors { get; set; }
        public int MaxConcurrentWorkers { get; set; }
        public int MaxQueueCapacity { get; set; }
        public double SuccessRate => TotalQueued > 0 ? (ProcessedCount * 100.0 / TotalQueued) : 0;
    }
}