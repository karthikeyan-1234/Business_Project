using CommonLibrary.Contexts;
using CommonLibrary.Models;
using CommonLibrary.Repositories;
using MediatR;
using Services.CQRS.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.CQRS.Handlers
{
    public class AddPurchaseDetailCommandHandler : IRequestHandler<AddPurchaseDetailCommand, PurchaseDetail>
    {
        IGenericRepository<PurchaseDetail,PurchaseDBContext>  _purchaseRepository;

        public AddPurchaseDetailCommandHandler(IGenericRepository<PurchaseDetail, PurchaseDBContext> _purchaseRepository)
        {
            this._purchaseRepository = _purchaseRepository;
        }

        public async Task<PurchaseDetail> Handle(AddPurchaseDetailCommand request, CancellationToken cancellationToken)
        {
            var result = await _purchaseRepository.AddAsync(request.newPurchaseDetail);
            await _purchaseRepository.SaveChangesAsync();
            return result;
        }
    }
}
