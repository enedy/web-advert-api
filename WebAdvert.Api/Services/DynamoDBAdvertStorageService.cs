using System;
using System.Threading.Tasks;
using WebAdvert.Models;
using AutoMapper;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using System.Collections.Generic;
using Amazon;
using System.Linq;

namespace WebAdvert.Api.Services
{
    public class DynamoDBAdvertStorageService : IAdvertStorageService
    {
        private readonly IMapper _mapper;
        private readonly IAmazonDynamoDB _amazonDynamoDB;
        private readonly IDynamoDBContext _dynamoDBContext;
        public DynamoDBAdvertStorageService(IMapper mapper, IAmazonDynamoDB amazonDynamoDB,
            IDynamoDBContext dynamoDBContext)
        {
            _mapper = mapper;
            _amazonDynamoDB = amazonDynamoDB;
            _dynamoDBContext = dynamoDBContext;
        }

        public async Task<string> Add(AdvertModel model)
        {
            var dbModel = _mapper.Map<AdvertDbModel>(model);
            dbModel.Id = Guid.NewGuid().ToString();
            dbModel.CreationDateTime = DateTime.UtcNow;
            dbModel.AdvertStatus = AdvertStatus.Pending;

            await _dynamoDBContext.SaveAsync(dbModel);

            return dbModel.Id;
        }

        public async Task<bool> CheckHealthAsync()
        {
            var tableData = await _amazonDynamoDB.DescribeTableAsync("Adverts");
            return string.Compare(tableData.Table.TableStatus, "active", true) == 0;
        }

        public async Task Confirm(ConfirmAdvertModel model)
        {
            //using (var context = new DynamoDBContext(_amazonDynamoDB))
            //{
            var record = await _dynamoDBContext.LoadAsync<AdvertDbModel>(model.Id);
            if (record == null) throw new KeyNotFoundException($"A record with ID={model.Id} was not found.");

            if (model.AdvertStatus == AdvertStatus.Active)
            {
                record.AdvertStatus = AdvertStatus.Active;
                await _dynamoDBContext.SaveAsync(record);
            }
            else
            {
                await _dynamoDBContext.DeleteAsync(record);
            }
            //}
        }

        public async Task<List<AdvertModel>> GetAllAsync()
        {
            var scanResult =
                await _dynamoDBContext.ScanAsync<AdvertDbModel>(new List<ScanCondition>()).GetNextSetAsync();

            return scanResult.Select(item => _mapper.Map<AdvertModel>(item)).ToList();
        }

        public async Task<AdvertModel> GetByIdAsync(string id)
        {
            var dbModel = await _dynamoDBContext.LoadAsync<AdvertDbModel>(id);
            if (dbModel != null) return _mapper.Map<AdvertModel>(dbModel);

            throw new KeyNotFoundException();
        }
    }
}
