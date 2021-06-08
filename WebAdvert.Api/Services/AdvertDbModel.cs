using System;
using WebAdvert.Models;
using Amazon.DynamoDBv2.DataModel;

namespace WebAdvert.Api.Services
{
    [DynamoDBTable("Adverts")]
    public class AdvertDbModel
    {
        [DynamoDBHashKey]
        public string Id { get; set; }
        [DynamoDBProperty]
        public string Title { get; set; }
        [DynamoDBProperty]
        public string Description { get; set; }
        [DynamoDBProperty]
        public double Price { get; set; }
        [DynamoDBProperty]
        public DateTime CreationDateTime { get; set; }
        [DynamoDBProperty]
        public AdvertStatus AdvertStatus { get; set; }
    }
}
