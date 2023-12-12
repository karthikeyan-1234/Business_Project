using AutoMapper;

using CommonLibrary.Contexts;
using CommonLibrary.Models;
using CommonLibrary.Repositories;

using MediatR;

using Services.CQRS.Commands.Purchase_Commands;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.CQRS.Handlers.Purchase_Handlers
{
    public class UpdatePurchaseDetailCommandHandler : IRequestHandler<UpdatePurchaseDetailCommand, PurchaseDetail>
    {
        IGenericRepository<PurchaseDetail, PurchaseDBContext> repo;
        IMapper mapper;

        public UpdatePurchaseDetailCommandHandler(IGenericRepository<PurchaseDetail, PurchaseDBContext> repo, IMapper mapper)
        {
            this.repo = repo;
            this.mapper = mapper;
        }

        public async Task<PurchaseDetail> Handle(UpdatePurchaseDetailCommand request, CancellationToken cancellationToken)
        {
            var obj = mapper.Map<PurchaseDetail>(request.updatedPurchaseDetail);
            var result = repo.Update(obj);
            await repo.SaveChangesAsync();
            return result;
        }
    }
}
