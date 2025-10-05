using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebApp.Controllers.Filters
{
    public class FileUploadDocumentFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            // Находим все операции с IFormFile параметрами
            var fileUploadPaths = swaggerDoc.Paths
                .Where(path => path.Value.Operations.Any(op => 
                    op.Value.Parameters?.Any(p => p.Schema?.Type == "string" && p.Schema?.Format == "binary") == true))
                .ToList();

            foreach (var path in fileUploadPaths)
            {
                foreach (var operation in path.Value.Operations)
                {
                    if (operation.Value.Parameters?.Any(p => p.Schema?.Type == "string" && p.Schema?.Format == "binary") == true)
                    {
                        // Заменяем параметры на requestBody
                        operation.Value.Parameters = operation.Value.Parameters
                            .Where(p => p.Schema?.Type != "string" || p.Schema?.Format != "binary")
                            .ToList();

                        operation.Value.RequestBody = new OpenApiRequestBody
                        {
                            Description = "Файл для загрузки",
                            Content = new Dictionary<string, OpenApiMediaType>
                            {
                                ["multipart/form-data"] = new OpenApiMediaType
                                {
                                    Schema = new OpenApiSchema
                                    {
                                        Type = "object",
                                        Properties = new Dictionary<string, OpenApiSchema>
                                        {
                                            ["file"] = new OpenApiSchema
                                            {
                                                Type = "string",
                                                Format = "binary",
                                                Description = "Файл для загрузки"
                                            }
                                        },
                                        Required = new HashSet<string> { "file" }
                                    }
                                }
                            },
                            Required = true
                        };
                    }
                }
            }
        }
    }

    public class FileUploadOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Проверяем, есть ли параметры с типами, содержащими IFormFile
            var uploadFileParams = context.ApiDescription.ParameterDescriptions
                .Where(p => p.Type?.Name.Contains("UploadFileDto") == true ||
                           p.Type == typeof(IFormFile) ||
                           (p.Type?.GetProperties().Any(prop => prop.PropertyType == typeof(IFormFile)) == true))
                .ToList();

            if (uploadFileParams.Any())
            {
                // Удаляем все параметры, так как они будут заменены на requestBody
                operation.Parameters = operation.Parameters?
                    .Where(p => !uploadFileParams.Any(fp => fp.Name == p.Name))
                    .ToList() ?? new List<OpenApiParameter>();

                // Создаем requestBody для multipart/form-data
                operation.RequestBody = new OpenApiRequestBody
                {
                    Description = "Файл для загрузки",
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["multipart/form-data"] = new OpenApiMediaType
                        {
                            Schema = new OpenApiSchema
                            {
                                Type = "object",
                                Properties = new Dictionary<string, OpenApiSchema>
                                {
                                    ["file"] = new OpenApiSchema
                                    {
                                        Type = "string",
                                        Format = "binary",
                                        Description = "Файл для загрузки"
                                    }
                                },
                                Required = new HashSet<string> { "file" }
                            }
                        }
                    },
                    Required = true
                };
            }
        }
    }

    // Фильтр схемы для обработки IFormFile
    public class FormFileSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type == typeof(IFormFile))
            {
                schema.Type = "string";
                schema.Format = "binary";
            }
        }
    }

    // Фильтр параметров, который предотвращает обработку IFormFile
    public class FormFileParameterFilter : IParameterFilter
    {
        public void Apply(OpenApiParameter parameter, ParameterFilterContext context)
        {
            // Если параметр является IFormFile, скрываем его
            if (context.ParameterInfo?.ParameterType == typeof(IFormFile))
            {
                parameter.Schema = new OpenApiSchema
                {
                    Type = "string",
                    Format = "binary"
                };
            }
        }
    }
}
