using CommonLibrary.Models;

using MediatR;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.CQRS.Commands
{
    public class AddMaterialCommand:IRequest<Material>
    {
        public Material newMaterial { get; set; }

        public AddMaterialCommand(Material newMaterial)
        {
            this.newMaterial = newMaterial;
        }
    }
}
