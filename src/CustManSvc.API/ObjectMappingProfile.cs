using System;
using System.Globalization;
using AutoMapper;
using CustManSvc.API.Service.Database;
using CustManSvc.API.DataTransferObject;
using CustManSvc.API.Common;

namespace CustManSvc.API
{
    public class ObjectMappingProfile : Profile
    {
        public ObjectMappingProfile()
        {
            CreateMap<Customer, CustomerDTO>()
                .ForMember(d => d.DateOfBirth, 
                    opt => opt.MapFrom(s => s.DateOfBirth.ToString(Constants.DateFormatRFC3339)));
            CreateMap<CustomerDTO, Customer>()
                .ForMember(d => d.DateOfBirth,
                    // If there are timezone, convert to UTC
                    opt => opt.MapFrom(s => DateTime.ParseExact(s.DateOfBirth, 
                            Constants.DateFormatRFC3339, DateTimeFormatInfo.InvariantInfo).ToUniversalTime()));
        }
    }
}