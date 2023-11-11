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
    public class AddPurchaseCommandHandler : IRequestHandler<AddPurchaseCommand, Purchase>
    {
        IGenericRepository<Purchase,PurchaseDBContext> _purchaseRepository;

        public AddPurchaseCommandHandler(IGenericRepository<Purchase, PurchaseDBContext> _purchaseRepository)
        {
            this._purchaseRepository = _purchaseRepository;
        }

        public async Task<Purchase> Handle(AddPurchaseCommand request, CancellationToken cancellationToken)
        {
            var purchase = await _purchaseRepository.AddAsync(request.newPurchase);
            await _purchaseRepository.SaveChangesAsync();
            return purchase;
        }
    }
}
