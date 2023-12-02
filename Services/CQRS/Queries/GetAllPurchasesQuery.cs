using CommonLibrary.Models;

using MediatR;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.CQRS.Queries
{
    public class GetAllPurchasesQuery: IRequest<IEnumerable<Purchase>>
    {
        public GetAllPurchasesQuery()
        {

        }
    }
}
