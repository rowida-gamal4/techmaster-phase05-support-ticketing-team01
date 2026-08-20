namespace SupportTicketing.Application.Common.Models;

public enum ErrorType
{
    Validation,
    NotFound,
    Conflict,
    BadRequest,
    Forbidden,
    InternalServerError ,
    Unauthorized
}