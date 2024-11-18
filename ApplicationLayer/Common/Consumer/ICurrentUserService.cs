using Microsoft.AspNetCore.Http;

namespace ApplicationLayer.Common.Consumer
{
    public interface ICurrentUserService
    {
        string? UserId { get; }
        string? IpAddress { get; }
    }
}
