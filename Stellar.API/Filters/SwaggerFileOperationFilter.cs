using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace Stellar.API.Filters
{
#nullable disable
    public class SwaggerFileOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // 查找使用了 [FromForm] 特性的參數
            var formFileParameters = context.MethodInfo.GetParameters()
                .Where(p => p.GetCustomAttribute<FromFormAttribute>() != null)
                .Select(p => p)
                .ToList();

            if (!formFileParameters.Any())
            {
                return;
            }

            // 移除已有的參數
            operation.Parameters.Clear();

            // 修改請求類型轉為 multipart/form-data
            operation.RequestBody = new OpenApiRequestBody
            {
                Content =
                {
                    ["multipart/form-data"] = new OpenApiMediaType
                    {
                        Schema = GenerateSchema(context, formFileParameters)
                    }
                }
            };
        }

        private OpenApiSchema GenerateSchema(OperationFilterContext context, List<ParameterInfo> formFileParameters)
        {
            var properties = new Dictionary<string, OpenApiSchema>();

            foreach (var parameter in formFileParameters)
            {
                var parameterType = parameter.ParameterType;

                if (parameterType == typeof(IFormFile))
                {
                    properties.Add(parameter.Name, new OpenApiSchema
                    {
                        Type = "string",
                        Format = "binary"
                    });
                }
                else if (typeof(IEnumerable<IFormFile>).IsAssignableFrom(parameterType))
                {
                    properties.Add(parameter.Name, new OpenApiSchema
                    {
                        Type = "array",
                        Items = new OpenApiSchema
                        {
                            Type = "string",
                            Format = "binary"
                        }
                    });
                }
                else
                {
                    // 如果參數是複雜類型，則遞增生成屬性
                    var schema = context.SchemaGenerator.GenerateSchema(parameterType, context.SchemaRepository);
                    properties.Add(parameter.Name, schema);
                }
            }

            return new OpenApiSchema
            {
                Type = "object",
                Properties = properties,
                Required = new HashSet<string>(formFileParameters.Select(p => p.Name))
            };
        }
    }
}
