using System.ComponentModel.DataAnnotations;

namespace Wst.Trios.Api.Infrastructure.CoreApi;

public sealed class CoreApiOptions
{
    public const string SectionName = "CoreApi";

    [Required]
    [Url]
    public string BaseUrl { get; set; } = string.Empty;
}
