using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Stellar.API.Dtos.Category_Tag;

namespace Stellar.API.Service.Category_Tag
{
    public class TagService
    {
        private readonly IRepository<Tag> _tagRepository;
        private readonly IRepository<TagConnect> _tagConnectrepository;
        private readonly IMapper _mapper; // 注入 IMapper
     
        public TagService(IRepository<Tag> tagRepository, IMapper mapper, IRepository<TagConnect> tagConnectrepository)
        {
            _tagRepository = tagRepository;
            _mapper = mapper;
        
            _tagConnectrepository = tagConnectrepository;
        }
        public async Task<List<TagDto>> GetTags()
        {
            var tags = await _tagRepository.ListAsync();
            return _mapper.Map<List<TagDto>>(tags);
        }

        public async Task CreateTag(Tag tag)
        {
            await _tagRepository.AddAsync(tag);
        }
        public async Task UpdateTag([FromBody] Tag tag)
        {
            var _tag = await _tagRepository.GetByIdAsync(tag.TagId);
            _tag.TagName= tag.TagName;
            
            await _tagRepository.UpdateAsync(_tag);
        }
        public async Task DeleteTag(int id)
        {
            var tc = await _tagConnectrepository.ListAsync(t => t.TagId == id);
            await _tagConnectrepository.DeleteRangeAsync(tc);

            var tag = await _tagRepository.GetByIdAsync(id);
            await _tagRepository.DeleteAsync(tag);
        }
    }
}
