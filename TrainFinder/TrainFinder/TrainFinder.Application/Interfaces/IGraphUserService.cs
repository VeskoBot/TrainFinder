namespace TrainFinder.Application.Interfaces;

public interface IGraphUserService
{
    Task<GraphUserProfile?> GetUserProfileAsync();
    Task UpdateUserProfileAsync(string objectId, string? displayName, string? givenName, string? surname, string? mobileNumber);
    Task DeleteUserAsync(string externalObjectId);
}
