using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.BillServices.Models;
using Application.Common.Data;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.BillServices.Queries
{
    public class GetBillByIdRequest : IRequest<BillDto>
    {
        public Guid Id { get; set; }

        public class GetBillByIdRequestHandler : IRequestHandler<GetBillByIdRequest, BillDto>
        {
            private readonly IBillTrackerDbContext _billTrackerDbContext;
            private readonly IMapper _mapper;

            public GetBillByIdRequestHandler(IBillTrackerDbContext billTrackerDbContext, IMapper mapper)
            {
                _billTrackerDbContext = billTrackerDbContext;
                _mapper = mapper;
            }

            public async Task<BillDto> Handle(GetBillByIdRequest request, CancellationToken cancellationToken)
            {
                var bill =  await _billTrackerDbContext
                    .Bills
                    .FindAsync(request.Id);

                return _mapper.Map<BillDto>(bill);
            }
        }
    }
}