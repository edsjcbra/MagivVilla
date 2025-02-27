using System.Net;

namespace MagicVilla_VillaApi.Models;

public class ApiResponse
{
    public HttpStatusCode StatusCode { get; set; }
    public bool IsSucess { get; set; } = true;
    public List<string> ErrorMessages { get; set; }
    public object ApiContent { get; set; }
}