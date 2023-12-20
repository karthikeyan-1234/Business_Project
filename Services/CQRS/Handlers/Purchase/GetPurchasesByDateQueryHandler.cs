using CommonLibrary.Contexts;
using CommonLibrary.Models;
using CommonLibrary.Repositories;

using MediatR;

using Services.CQRS.Queries.Purchases;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.CQRS.Handlers.Purchase_Handlers
{
    public class GetPurchaseDetailsByDateQueryHandler : IRequestHandler<GetPurchaseDetailsByDateQuery, IEnumerable<PurchaseDetail>>
    {
        IGenericRepository<PurchaseDetail, PurchaseDBContext> repo;

        public GetPurchaseDetailsByDateQueryHandler(IGenericRepository<PurchaseDetail, PurchaseDBContext> repo)
        {
            this.repo = repo;
        }

        public Task<IEnumerable<PurchaseDetail>> Handle(GetPurchaseDetailsByDateQuery request, CancellationToken cancellationToken)
        {
            var res = (IEnumerable<PurchaseDetail>) repo.Find(p => p.lastEditat >= request.startDate && p.lastEditat <= request.endDate);
            return Task.FromResult(res);
        }
    }
}
