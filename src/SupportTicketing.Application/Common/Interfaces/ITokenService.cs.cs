namespace SupportTicketing.Application.Common.Interfaces
{
    public interface ITokenService
    {
        public string GenerateToken(int userId, string email,string userName, string role);


    }
}
