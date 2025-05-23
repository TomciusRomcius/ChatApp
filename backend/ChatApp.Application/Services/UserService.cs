using ChatApp.Application.Interfaces;
using ChatApp.Application.Persistence;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Utils;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Application.Services;

public class UserService : IUserService
{
    private readonly DatabaseContext _databaseContext;

    public UserService(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }

    public async Task<bool> IsPublicInfoSetup(string userId)
    {
        IQueryable<int>? query = _databaseContext.PublicUserInfos.Where(pui => pui.UserId == userId).Select(_ => 1);

        int count = await query.CountAsync();
        return count == 1;
    }

    public async Task<Result<List<PublicUserInfoEntity>>> GetPublicUserInfos(List<string> userIds)
    {
        Result<List<PublicUserInfoEntity>>? result = null;

        // TODO: Should test performance of this as it may be quite slow
        IQueryable<PublicUserInfoEntity> query =
            _databaseContext.PublicUserInfos.Where(pui => userIds.Contains(pui.UserId));

        List<PublicUserInfoEntity> dbResult = query.ToList();
        if (!dbResult.Any())
        {
            List<ResultError> errors = [new(ResultErrorType.NOT_FOUND, "User info row does not exist.")];
            result = new Result<List<PublicUserInfoEntity>>(errors);
        }
        else
        {
            result = new Result<List<PublicUserInfoEntity>>(dbResult);
        }

        return result;
    }

    public async Task<List<ResultError>> SetUserInfo(PublicUserInfoEntity entity)
    {
        List<ResultError> errors = entity.Validate();
        if (errors.Any())
            return errors;

        try
        {
            await _databaseContext.PublicUserInfos.AddAsync(entity);
            await _databaseContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            if (ex.InnerException is SqlException sqlException)
            {
                if (sqlException.Number == 2627 || sqlException.Number == 2601)
                    errors.Add(new ResultError(ResultErrorType.VALIDATION_ERROR, "The username is already taken"));
            }

            else
            {
                // TODO: log
                errors.Add(new ResultError(ResultErrorType.UNKNOWN_ERROR, "Unknown error occured"));
            }
        }

        return errors;
    }
}