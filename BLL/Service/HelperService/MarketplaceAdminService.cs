using BLL.Model.RequestModel.HelperModel;
using BLL.Model.ServiceResponse;
using BLL.Service.Interface;
using DAL.Context;
using DAL.Repository;
using DAL.Repository.Interface;
using Domain.Model;

namespace BLL.Service;

public class MarketplaceAdminService : IGenericService<MarketplaceAdmin>
{
    private readonly IGenericRepository<MarketplaceAdmin> _repository;

    public MarketplaceAdminService(IGenericRepository<MarketplaceAdmin> repository)
    {
        _repository = repository;
    }

    public async Task<ServiceResponse<MarketplaceAdmin>> GetAsync(int id)
    {
        ServiceResponse<MarketplaceAdmin> response = new ServiceResponse<MarketplaceAdmin>();
        try
        {
            MarketplaceAdmin? entity = await _repository.GetById(id);
            if (entity == null)
            {
                response.IsSuccess = false;
                response.Message = ServiceResponseMessages.UserNotFoundById(id);
            }
            else
            {
                response.Entity = entity;
                response.IsSuccess = true;
            }
        }
        catch (Exception ex)
        {
            response.Message = ex.Message;
            response.IsSuccess = false;
        }
        return response;
}

    public async Task<ServiceResponse<MarketplaceAdmin>> CreateAsync(MarketplaceAdmin entity)
    {
        ServiceResponse<MarketplaceAdmin> response = new ServiceResponse<MarketplaceAdmin>();

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

    public async Task<ServiceResponse<MarketplaceAdmin>> UpdateAsync(MarketplaceAdmin entity)
    {
        ServiceResponse<MarketplaceAdmin> response = new ServiceResponse<MarketplaceAdmin>();

        try
        {
            await _repository.Update(entity);
            await _repository.SaveChangesAsync();

            response.IsSuccess = true;
        }
        catch (Exception ex)
        {
            response.IsSuccess = false;
            response.Message = ex.Message;
        }
        return response;
    }

    public async Task<ServiceResponse<MarketplaceAdmin>> DeleteAsync(int id)
    {
        ServiceResponse<MarketplaceAdmin> response = new ServiceResponse<MarketplaceAdmin>();

        try
        {
            MarketplaceAdmin? entity = await _repository.GetById(id);
            if (entity == null)
            {
                response.IsSuccess = false;
            }
            else
            {
                await _repository.Delete(entity);
                await _repository.SaveChangesAsync();
                response.IsSuccess = true;
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