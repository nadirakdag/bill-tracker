using System;
using System.Threading;
using System.Threading.Tasks;
using Application.BillServices.Models;
using Application.Common.Data;
using AutoMapper;
using Domain;
using MediatR;

namespace Application.BillServices.Commands
{
    public class CreateBillRequest : IRequest<BillDto>
    {
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public BillStatus Status { get; set; }
        public DateTime BillDate { get; set; }

        public class CreateBillRequestHandler : IRequestHandler<CreateBillRequest, BillDto>
        {
            private readonly IBillTrackerDbContext _billTrackerDbContext;
            private readonly IMapper _mapper;

            public CreateBillRequestHandler(IBillTrackerDbContext billTrackerDbContext, IMapper mapper)
            {
                _billTrackerDbContext = billTrackerDbContext;
                _mapper = mapper;
            }

            public async Task<BillDto> Handle(CreateBillRequest request, CancellationToken cancellationToken)
            {
                var bill = new Bill
                {
                    Amount = request.Amount,
                    Description =  request.Description,
                    Status = request.Status,
                    BillDate = request.BillDate,
                    CreatedDate = DateTime.Now
                };
                
                await _billTrackerDbContext.Bills.AddAsync(bill, cancellationToken);
                await _billTrackerDbContext.SaveChangesAsync(cancellationToken);
                return _mapper.Map<BillDto>(bill);
            }
        }
    }
}