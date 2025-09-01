using AutoMapper;
using BLL.Model.DTO;
using BLL.Model.RequestModel.HelperModel;
using Domain.Model;

namespace BLL.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        //Address
        CreateMap<Address, AddressDTO>();
        CreateMap<AddressDTO, Address>();
        
        //MarketplaceShop
        CreateMap<MarketplaceShop, MarketplaceShopDTO>().ForMember(x => x.Email,
             options =>
                 options.MapFrom(x => x.ApplicationUser.Email));
        CreateMap<MarketplaceShopDTO, MarketplaceShop>();
        
        //MarketplaceUser
        CreateMap<MarketplaceUser, UserShopView>();
        CreateMap<UserShopView, MarketplaceUser>();

        //MarketplaceAdmin

        // CreateMap<List<MarketplaceShop>, List<MarketplaceShopDTO>>();
        // CreateMap<List<MarketplaceShopDTO>, List<MarketplaceShop>>();
    }
}