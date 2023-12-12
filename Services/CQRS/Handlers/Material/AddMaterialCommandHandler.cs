using CommonLibrary.Contexts;
using CommonLibrary.Models;
using CommonLibrary.Repositories;

using Services.CQRS.Commands.Material_Commands;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.CQRS.Handlers.Material_Handlers
{
    public class AddMaterialCommandHandler
    {
        IGenericRepository<Material, MaterialDBContext> _repo;

        public AddMaterialCommandHandler(IGenericRepository<Material, MaterialDBContext> _repo)
        {
            this._repo = _repo;
        }

        public async Task<Material> Handle(AddMaterialCommand request, CancellationToken cancellationToken)
        {
            var material = await _repo.AddAsync(request.newMaterial);
            await _repo.SaveChangesAsync();
            return material;
        }
    }
}
