using ChatApp.Domain.Entities;
using ChatApp.Domain.Utils;

namespace ChatApp.Application.Interfaces;

public interface IUserService
{
    Task<Result<List<PublicUserInfoEntity>>> GetPublicUserInfos(List<string> userIds);
    Task<List<ResultError>> SetUserInfo(PublicUserInfoEntity entity);
}