using System;
using System.Threading;
using System.Threading.Tasks;
using Application.BillServices.Commands;
using Application.Common.Exceptions;
using Xunit;

namespace IntegrationTests
{
    public class ApplicationIntegrationTest : IntegrationTestBase
    {
        [Fact]
        public async Task DeleteBillRequest_Should_Throw_BillNotFoundException()
        {
            var request = new DeleteBillRequest {Id = Guid.NewGuid()};
            var requestHandler = new DeleteBillRequest.DeleteBillRequestHandler(_billTrackerDbContext);
            await Assert.ThrowsAsync<BillNotFoundException>(
                () => requestHandler.Handle(request, CancellationToken.None));
        }

        [Fact]
        public async Task UpdateBillRequest_Should_Throw_BillNotFoundException()
        {
            var request = new UpdateBillRequest() {Id = Guid.NewGuid()};
            var requestHandler = new UpdateBillRequest.UpdateBillRequestHandler(_billTrackerDbContext);
            await Assert.ThrowsAsync<BillNotFoundException>(
                () => requestHandler.Handle(request, CancellationToken.None));
        }
    }
}