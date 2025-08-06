using BLL.Model.RequestModel.HelperModel;
using BLL.Model.ServiceResponse;
using BLL.Service.Interface;
using DAL.Repository;
using DAL.Repository.Interface;
using Domain.Model;

namespace BLL.Service;

public class MarketplaceShopService : IGenericService<MarketplaceShop>
{
    private readonly IGenericRepository<MarketplaceShop> _repository;

    public MarketplaceShopService(IGenericRepository<MarketplaceShop> repository)
    {
        _repository = repository;
    }

    public async Task<ServiceResponse<MarketplaceShop>> GetAsync(int id)
    {
        var response = new ServiceResponse<MarketplaceShop>();

        try
        {
            var entity = await _repository.GetById(id);
            if (entity is null)
            {
                response.IsSuccess = false;
                response.Message = ServiceResponseMessages.UserNotFoundById(id);
            }
            else
            {
                response.IsSuccess = true;
                response.Entity = entity;
            }
        }
        catch (Exception ex)
        {
            response.IsSuccess = false;
            response.Message = ex.Message;
        }

        return response;
    }

    public async Task<ServiceResponse<MarketplaceShop>> CreateAsync(MarketplaceShop entity)
    {
        var response = new ServiceResponse<MarketplaceShop>();

        try
        {
            var user = await _repository.Create(entity);
            await _repository.SaveChangesAsync();

            response.IsSuccess = true;
            response.Entity = user;
        }
        catch (Exception ex)
        {
            response.IsSuccess = false;
            response.Message = ex.Message;
        }

        return response;
    }

    public async Task<ServiceResponse<MarketplaceShop>> UpdateAsync(MarketplaceShop entity)
    {
        var response = new ServiceResponse<MarketplaceShop>();

        try
        {
            await _repository.Update(entity);
            await _repository.SaveChangesAsync();

            response.IsSuccess = true;
            response.Entity = entity;
        }
        catch (Exception ex)
        {
            response.IsSuccess = false;
            response.Message = ex.Message;
        }

        return response;
    }

    public async Task<ServiceResponse<MarketplaceShop>> DeleteAsync(int id)
    {
        var response = new ServiceResponse<MarketplaceShop>();

        try
        {
            var entity = await _repository.GetById(id);
            if (entity is null)
            {
                response.IsSuccess = false;
            }
            else
            {
                await _repository.Delete(entity);
                await _repository.SaveChangesAsync();

                response.IsSuccess = true;
                response.Entity = entity;
            }
        }
        catch (Exception ex)
        {
            response.IsSuccess = false;
            response.Message = ex.Message;
        }

        return response;
    }
}