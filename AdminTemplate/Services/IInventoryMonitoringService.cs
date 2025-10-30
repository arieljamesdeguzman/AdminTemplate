using System.Threading.Tasks;

namespace AdminTemplate.Services
{
    public interface IInventoryMonitoringService
    {
        Task CheckInventoryLevelsAsync();
    }
}