
using SkiStore.API.Models.API;
using System.Text.Json;

namespace SkiStore.API.Extensions;

public static class HttpExtensions
{
    public static void AddPaginationHeader(this HttpResponse response, MetaData metaData)
    {
        JsonSerializerOptions options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        response.Headers.Add("Pagination", JsonSerializer.Serialize(metaData));
        response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
    }
}
