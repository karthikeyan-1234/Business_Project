using AutoMapper;
using CommonLibrary.DTOs;
using CommonLibrary.Models;
using CommonLibrary.Models.Requests;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary.Mapping
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<NewPurchaseRequest, PurchaseDTO>();
            CreateMap<PurchaseDTO, NewPurchaseRequest>();

            CreateMap<PurchaseDTO, Purchase>();
            CreateMap<Purchase,PurchaseDTO>();

            CreateMap<PurchaseResultDTO, Purchase>();
            CreateMap<Purchase, PurchaseResultDTO>().ForMember(x => x.purchaseDate, y => y.MapFrom(z => DateOnly.FromDateTime((DateTime)z.purchaseDate)));

            CreateMap<NewPurchaseDetailRequest, PurchaseDetailDTO>();
            CreateMap<PurchaseDetailDTO, NewPurchaseDetailRequest>();

            CreateMap<UpdatePurchaseDetailRequest, PurchaseDetailDTO>();
            CreateMap<PurchaseDetailDTO, UpdatePurchaseDetailRequest>();

            CreateMap<NewPurchaseRequest, Purchase>();
            CreateMap<Purchase, NewPurchaseRequest>();

            CreateMap<NewPurchaseDetailRequest, PurchaseDetail>();
            CreateMap<PurchaseDetail, NewPurchaseDetailRequest>();

            CreateMap<PurchaseDetail,PurchaseDetailDTO>();
            CreateMap<PurchaseDetailDTO, PurchaseDetail>();

            CreateMap<Material, MaterialDTO>();
            CreateMap<MaterialDTO, Material>();

            CreateMap<NewMaterialRequest, Material>();
            CreateMap<Material, NewMaterialRequest>();

            CreateMap<NewMaterialRequest, MaterialDTO>();
            CreateMap<MaterialDTO, NewMaterialRequest>();


            CreateMap<Material, MaterialDTO>();
            CreateMap<MaterialDTO, Material>();

            CreateMap<NewMaterialRequest, Material>();
            CreateMap<Material, NewMaterialRequest>();

            CreateMap<NewMaterialRequest, MaterialDTO>();
            CreateMap<MaterialDTO, NewMaterialRequest>();

            CreateMap<NewInventoryRequest, Inventory>();
            CreateMap<Inventory, NewInventoryRequest>();

            CreateMap<Inventory, InventoryDTO>();
            CreateMap<InventoryDTO, Inventory>();
        }
    }
}
