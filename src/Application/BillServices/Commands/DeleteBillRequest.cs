using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Data;
using Application.Common.Exceptions;
using MediatR;

namespace Application.BillServices.Commands
{
    public class DeleteBillRequest : IRequest<Unit>
    {
        public Guid Id { get; set; }
        
        public class DeleteBillRequestHandler : IRequestHandler<DeleteBillRequest, Unit>
        {
            private readonly IBillTrackerDbContext _billTrackerDbContext;

            public DeleteBillRequestHandler(IBillTrackerDbContext billTrackerDbContext)
            {
                _billTrackerDbContext = billTrackerDbContext;
            }

            public async Task<Unit> Handle(DeleteBillRequest request, CancellationToken cancellationToken)
            {
                var bill = await _billTrackerDbContext.Bills.FindAsync(request.Id);
                if (bill == null)
                    throw new BillNotFoundException();

                _billTrackerDbContext.Bills.Remove(bill);
                await _billTrackerDbContext.SaveChangesAsync(cancellationToken);

                return Unit.Value;
            }
        }
    }
}