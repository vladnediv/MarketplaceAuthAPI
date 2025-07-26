using AutoMapper;
using BLL.Model.RequestModel.HelperModel;
using Domain.Model;

namespace BLL.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Address, AddressDTO>();
        CreateMap<AddressDTO, Address>();
    }
}