using BLL.Model.RequestModel.HelperModel;
using BLL.Model.ServiceResponse;
using BLL.Service.Interface;
using DAL.Repository;
using DAL.Repository.Interface;
using Domain.Model;

namespace BLL.Service;

public class MarketplaceUserService : IGenericService<MarketplaceUser>
{
    private readonly IGenericRepository<MarketplaceUser> _repository;

    public MarketplaceUserService(IGenericRepository<MarketplaceUser> repository)
    {
        _repository = repository;
    }

    public async Task<ServiceResponse<MarketplaceUser>> GetAsync(int id)
    {
        var response = new ServiceResponse<MarketplaceUser>();

        try
        {
            var entity = await _repository.GetById(id);
            if (entity == null)
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

    public async Task<ServiceResponse<MarketplaceUser>> CreateAsync(MarketplaceUser entity)
    {
        var response = new ServiceResponse<MarketplaceUser>();

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

    public async Task<ServiceResponse<MarketplaceUser>> UpdateAsync(MarketplaceUser entity)
    {
        var response = new ServiceResponse<MarketplaceUser>();

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

    public async Task<ServiceResponse<MarketplaceUser>> DeleteAsync(int id)
    {
        var response = new ServiceResponse<MarketplaceUser>();

        try
        {
            var entity = await _repository.GetById(id);
            if (entity == null)
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