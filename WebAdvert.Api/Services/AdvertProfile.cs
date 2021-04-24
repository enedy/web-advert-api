using AutoMapper;
using WebAdvert.Models;

namespace WebAdvert.Api.Services
{
    public class AdvertProfile : Profile
    {
        public AdvertProfile()
        {
            CreateMap<AdvertModel, AdvertDbModel>();
        }
    }
}
