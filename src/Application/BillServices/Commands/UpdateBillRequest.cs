using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Data;
using Application.Common.Exceptions;
using Domain;
using MediatR;

namespace Application.BillServices.Commands
{
    public class UpdateBillRequest : IRequest<Unit>
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public BillStatus Status { get; set; }
        public DateTime BillDate { get; set; }
        public DateTime? PaidDate { get; set; }
        
        public class UpdateBillRequestHandler : IRequestHandler<UpdateBillRequest, Unit>
        {
            private readonly IBillTrackerDbContext _billTrackerDbContext;

            public UpdateBillRequestHandler(IBillTrackerDbContext billTrackerDbContext)
            {
                _billTrackerDbContext = billTrackerDbContext;
            }

            public async Task<Unit> Handle(UpdateBillRequest request, CancellationToken cancellationToken)
            {
                var bill = await _billTrackerDbContext.Bills.FindAsync(request.Id);
                if (bill == null)
                    throw new BillNotFoundException();

                bill.Amount = request.Amount;
                bill.Description = request.Description;
                bill.Status = request.Status;
                bill.BillDate = request.BillDate;
                bill.PaidDate = request.PaidDate;

                await _billTrackerDbContext.SaveChangesAsync(cancellationToken);
                return  Unit.Value;
            }
        }
    }
}