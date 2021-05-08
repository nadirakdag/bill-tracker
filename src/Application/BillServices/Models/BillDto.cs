using System;
using Application.Common.Mappings;
using AutoMapper;
using Domain;

namespace Application.BillServices.Models
{
    public class BillDto : IMapFrom<Bill>
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public BillStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime BillDate { get; set; }
        public DateTime? PaidDate { get; set; }
        
        public void Mapping(Profile profile)
        {
            profile.CreateMap<Bill, BillDto>();
        }
    }
}