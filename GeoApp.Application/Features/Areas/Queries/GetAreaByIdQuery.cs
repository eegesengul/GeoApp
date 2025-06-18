using GeoApp.Application.Features.Areas.Queries;
using MediatR;
using System;

namespace GeoApp.Application.Features.Areas.Queries
{
    public class GetAreaByIdQuery : IRequest<AreaDto>
    {
        public Guid Id { get; set; }

        public GetAreaByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
