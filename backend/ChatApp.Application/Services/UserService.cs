using ChatApp.Application.Interfaces;
using ChatApp.Application.Persistance;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Utils;

namespace ChatApp.Application.Services;

public class UserService : IUserService
{
    private readonly DatabaseContext _databaseContext;

    public UserService(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }

    public async Task<Result<List<PublicUserInfoEntity>>> GetPublicUserInfos(List<string> userIds)
    {
        Result<List<PublicUserInfoEntity>>? result = null;
        
        // TODO: Should test performance of this as it may be quite slow
        var query = _databaseContext.PublicUserInfos.Where(pui => userIds.Contains(pui.UserId));
        
        List<PublicUserInfoEntity> dbResult = query.ToList();
        if (!dbResult.Any())
        {
            List<ResultError> errors = [new ResultError(ResultErrorType.NOT_FOUND, "User info row does not exist.")];
            result = new Result<List<PublicUserInfoEntity>>(errors);   
        }
        return new Result<List<PublicUserInfoEntity>>(dbResult);
    }
    
    public async Task<List<ResultError>> SetUserInfo(PublicUserInfoEntity entity)
    {
        List<ResultError> errors = entity.Validate();
        if (errors.Any())
            return errors;
        
        await _databaseContext.PublicUserInfos.AddAsync(entity);
        await _databaseContext.SaveChangesAsync();
        return [];
    }
}