using CommonLibrary.Contexts;
using CommonLibrary.Models;
using CommonLibrary.Repositories;

using MediatR;

using Services.CQRS.Queries;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.CQRS.Handlers
{
    public class GetallPurchasesHandler : IRequestHandler<GetAllPurchasesQuery, IEnumerable<Purchase>>
    {
        IGenericRepository<Purchase, PurchaseDBContext> repo;

        public GetallPurchasesHandler(IGenericRepository<Purchase, PurchaseDBContext> repo)
        {
            this.repo = repo;
        }

        public async Task<IEnumerable<Purchase>> Handle(GetAllPurchasesQuery request, CancellationToken cancellationToken)
        {
           return await repo.GetAllAsync();
        }
    }
}
