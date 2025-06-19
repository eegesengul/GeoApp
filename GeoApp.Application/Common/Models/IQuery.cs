using MediatR;

namespace GeoApp.Application.Common.Models
{
    public interface IQuery<TResponse> : IRequest<Result<TResponse>>
    {
    }
}