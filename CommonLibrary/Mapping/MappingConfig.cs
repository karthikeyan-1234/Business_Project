using AutoMapper;
using CommonLibrary.DTOs;
using CommonLibrary.Models;
using CommonLibrary.Models.Requests;
using System;
using System.Collections.Generic;
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

            CreateMap<NewPurchaseRequest, Purchase>();
            CreateMap<Purchase, NewPurchaseRequest>();
        }
    }
}
