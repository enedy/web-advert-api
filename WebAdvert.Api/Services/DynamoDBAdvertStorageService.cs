using System;
using System.Threading.Tasks;
using WebAdvert.Models;
using AutoMapper;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;

namespace WebAdvert.Api.Services
{
    public class DynamoDBAdvertStorageService : IAdvertStorageService
    {
        private readonly IMapper _mapper;
        public DynamoDBAdvertStorageService(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task<string> Add(AdvertModel model)
        {
            var dbModel = _mapper.Map<AdvertDbModel>(model);
            dbModel.Id = new Guid().ToString();
            dbModel.CreationDateTime = DateTime.UtcNow;
            dbModel.AdvertStatus = AdvertStatus.Pending;

            using (var client = new AmazonDynamoDBClient())
            {
                using (var context = new DynamoDBContext(client))
                {
                    await context.SaveAsync(dbModel);
                }
            }

            return dbModel.Id;
        }

        public async Task<bool> Confirm(ConfirmAdvertModel model)
        {
            throw new NotImplementedException();
        }
    }
}
