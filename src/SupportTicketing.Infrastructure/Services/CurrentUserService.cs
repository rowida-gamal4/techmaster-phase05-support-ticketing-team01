using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using SupportTicketing.Application.Common.Interfaces;

namespace SupportTicketing.Infrastructure.Services{
   public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }
        

        public int? UserId
        {
            get
            {
                var userId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
                return int.TryParse(userId, out var id) ? id : null;
            }
        }

        public string? Role
        {
            get
            {
                var role = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Role);
            

                return role;
            }

        }
        public string? Email
        {
            get
            {
                var email = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);
                return email ;
            }
        }
        public bool IsAuthenticated => httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    }
}