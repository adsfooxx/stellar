using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Infrastructure.Services.Linebot.SemanticKernel.Dtos
{
    public class SetUpProductSearchVectorDbRequest
    {
        [JsonPropertyName("start_index")] public int StartIndex { get; set; }
        [JsonPropertyName("count")] public int Count { get; set; }
    }
}
