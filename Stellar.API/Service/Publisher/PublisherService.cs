using ApplicationCore.Interfaces;
using ApplicationCore.Entities;
using Stellar.API.Dtos.Supplier;
namespace Stellar.API.Service.Publisher
{
    public class PublisherService
    {
        private readonly IRepository<ApplicationCore.Entities.Publisher> _publisherRepository;

        public PublisherService(IRepository<ApplicationCore.Entities.Publisher> publisherRepository)
        {
            _publisherRepository = publisherRepository;
        }

        public async Task<List<PublishersDto>> GetPublishers()
        {
            var publishers = (await _publisherRepository.ListAsync()).Select(p =>
            new PublishersDto
            {
                PublisherId = p.PublisherId,
                PublisherName = p.PublisherName,
                ConcatName = p.ContactName,
                Country = p.Country,
                Address = p.Address,
                Phone = p.Phone,
                Status = p.Status == 1 ? "活動中" : "停權中"
            }).ToList();

            return publishers;
        }

        public async Task UpdatePublisher(PublishersDto publisher) {

            if (publisher.PublisherId == 0)
            {
                var newPublisherData = new ApplicationCore.Entities.Publisher()
                {
                    PublisherName = publisher.PublisherName,
                    ContactName = publisher.ConcatName,
                    Country=publisher.Country,
                    Address = publisher.Address,
                    Phone = publisher.Phone,
                    Status= (publisher.Status == "活動中") ? (byte)1 : (byte)0,
            };
                await _publisherRepository.AddAsync(newPublisherData);
            }
            else { 
            var publisherInDB= await _publisherRepository.GetByIdAsync(publisher.PublisherId);

            if (publisherInDB != null) {
                publisherInDB.PublisherName = publisher.PublisherName;
                publisherInDB.ContactName=publisher.ConcatName;
                publisherInDB.Country = publisher.Country;
                publisherInDB.Address = publisher.Address;
                publisherInDB.Phone = publisher.Phone;
                publisherInDB.Status = (publisher.Status == "活動中") ? (byte)1 : (byte)0;


                await _publisherRepository.UpdateAsync(publisherInDB);
            }

            }

        }
    }
}
