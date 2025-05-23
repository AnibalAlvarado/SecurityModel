namespace SecurityGateway.Services.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(string userId, string username, string role);
    }
}
