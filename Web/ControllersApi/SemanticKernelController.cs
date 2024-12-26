using Infrastructure.Services.Linebot.SemanticKernel;
using Infrastructure.Services.Linebot.SemanticKernel.Dtos;
using Infrastructure.Services.Linebot.SemanticProductSearch;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Web.ControllersApi
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SemanticKernelController : ControllerBase
    {
        private readonly ProductDetailGenerateService _productDetailGenerateService;
        private readonly StellarChatService _productChatService;
#pragma warning disable SKEXP0020 // 類型僅供評估之用，可能會在未來更新中變更或移除。
        private readonly SemanticProductSearchService _semanticProductSearchService;


        public SemanticKernelController(ProductDetailGenerateService productDetailGenerateService,
            StellarChatService productChatService, SemanticProductSearchService semanticProductSearchService)
        {
            _productDetailGenerateService = productDetailGenerateService;
            _productChatService = productChatService;
            _semanticProductSearchService = semanticProductSearchService;
        }


        [HttpPost]
        public async Task<IActionResult> SetUpProductSearchVectorDb()
        {
            await _semanticProductSearchService.FetchAndSaveProductDocumentsAsync();
            return Ok();
        }


    }
}
