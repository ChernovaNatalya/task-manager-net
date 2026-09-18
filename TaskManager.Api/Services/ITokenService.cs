using TaskManager.Api.Models;

namespace TaskManager.Api.Services;

public interface ITokenService
{
    (string Token, DateTime ExpiresAt) GenerateToken(User user);
}