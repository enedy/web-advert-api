using System.Collections.Generic;
using System.Threading.Tasks;
using WebAdvert.Models;

namespace WebAdvert.Api.Services
{
    public interface IAdvertStorageService
    {
        Task<string> Add(AdvertModel model);
        Task Confirm(ConfirmAdvertModel model);
        Task<bool> CheckHealthAsync();
        Task<List<AdvertModel>> GetAllAsync();
        Task<AdvertModel> GetByIdAsync(string id);
    }
}
