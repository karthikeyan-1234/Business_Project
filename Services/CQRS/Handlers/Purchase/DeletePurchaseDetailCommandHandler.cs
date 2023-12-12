using CommonLibrary.Models;

using MediatR;
using CommonLibrary.Repositories;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonLibrary.Contexts;
using Services.CQRS.Commands.Purchase_Commands;

namespace Services.CQRS.Handlers.Purchase_Handlers
{
    public class DeletePurchaseDetailCommandHandler : IRequestHandler<DeletePurchaseDetailCommand, PurchaseDetail>
    {
        IGenericRepository<PurchaseDetail, PurchaseDBContext> repo;

        public DeletePurchaseDetailCommandHandler(IGenericRepository<PurchaseDetail, PurchaseDBContext> repo)
        {
            this.repo = repo;
        }

        public async Task<PurchaseDetail> Handle(DeletePurchaseDetailCommand request, CancellationToken cancellationToken)
        {
            repo.Delete(request.deletePurchaseDetail);
            await repo.SaveChangesAsync();
            return request.deletePurchaseDetail;
        }
    }
}
