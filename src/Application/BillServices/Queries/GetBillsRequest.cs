using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.BillServices.Models;
using Application.Common.Data;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.BillServices.Queries
{
    public class GetBillsRequest : IRequest<List<BillDto>>
    {

        public class GetBillsRequestHandler : IRequestHandler<GetBillsRequest, List<BillDto>>
        {
            private readonly IBillTrackerDbContext _billTrackerDbContext;
            private readonly IMapper _mapper;

            public GetBillsRequestHandler(IBillTrackerDbContext billTrackerDbContext, IMapper mapper)
            {
                _billTrackerDbContext = billTrackerDbContext;
                _mapper = mapper;
            }

            public async Task<List<BillDto>> Handle(GetBillsRequest request, CancellationToken cancellationToken)
            {
                return  await _billTrackerDbContext
                    .Bills
                    .AsNoTracking()
                    .ProjectTo<BillDto>(_mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);
            }
        }
    }
}