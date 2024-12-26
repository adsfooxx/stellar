using Infrastructure.Data.Mongo.Entity;
using Infrastructure.Data.Mongo.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.Mongo
{
    public  class mongoChat
    {

        private readonly MongoRepository<ChatDto> _mongoRepository;

        public mongoChat(MongoRepository<ChatDto> mongoRepository)
        {
            _mongoRepository = mongoRepository;
        }

        public async Task WriteHistoryAddToDb(string user, string chat)
        {
            ChatDto chatDto = new ChatDto()
            {
                User_Id = user,
                Chat = chat,
            };

            await _mongoRepository.CreateAsync(chatDto);
        }
        public async Task<List<ChatDto>> GetChatHistoryByUserIdAsync(string userId)
        {

            return await _mongoRepository.FindAsync("User_Id", userId);
        }
    }
}
