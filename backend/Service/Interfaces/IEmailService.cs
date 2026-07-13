using Service.DTOs;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IEmailService
    {
        Task SendPumpOperationReviewRequestAsync(PumpOperationDto operation, string checkerEmail, string reviewUrl);
    }
}
