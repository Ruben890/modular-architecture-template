namespace Shared.Messages.Queries
{
    public record GetUserByEmailOrUserNameHandler(string Email, string UserName);
}
