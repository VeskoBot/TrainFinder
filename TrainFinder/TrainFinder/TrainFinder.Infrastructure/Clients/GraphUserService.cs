using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using TrainFinder.Application.Interfaces;

namespace TrainFinder.Infrastructure.Clients;

public class GraphUserService : IGraphUserService
{
    private readonly GraphServiceClient _delegatedClient;
    private readonly GraphServiceClient _appClient;      
    private const string MobileNumberExtension = "extension_11c36be858624357b15e9e154301df1b_MobileNumber";

    public GraphUserService(GraphServiceClient delegatedClient, IConfiguration configuration)
    {
        _delegatedClient = delegatedClient;

        var tenantId = configuration["AzureAd:TenantId"]!;
        var clientId = configuration["AzureAd:ClientId"]!;
        var clientSecret = configuration["AzureAd:ClientSecret"]!;
        var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
        _appClient = new GraphServiceClient(credential, new[] { "https://graph.microsoft.com/.default" });
    }

    public async Task<GraphUserProfile?> GetUserProfileAsync()
    {
        var user = await _delegatedClient.Me.Request()
            .Select($"displayName,givenName,surname,mail,userPrincipalName,mobilePhone,{MobileNumberExtension}")
            .GetAsync();

        var mobileNumber = user.MobilePhone;
        if (user.AdditionalData?.TryGetValue(MobileNumberExtension, out var mobileValue) == true)
        {
            mobileNumber = mobileValue?.ToString();
        }

        return new GraphUserProfile
        {
            DisplayName = user.DisplayName,
            GivenName = user.GivenName,
            Surname = user.Surname,
            Email = user.Mail ?? user.UserPrincipalName,
            MobileNumber = mobileNumber
        };
    }

    public async Task UpdateUserProfileAsync(string objectId, string? displayName, string? givenName, string? surname, string? mobileNumber)
    {
        var updatedUser = new User
        {
            DisplayName = displayName,
            GivenName = givenName,
            Surname = surname,
            MobilePhone = mobileNumber,
            AdditionalData = new Dictionary<string, object>
            {
                { MobileNumberExtension, mobileNumber ?? "" }
            }
        };

        await _appClient.Users[objectId].Request().UpdateAsync(updatedUser);
    }

    public async Task DeleteUserAsync(string externalObjectId)
    {
        await _appClient.Users[externalObjectId].Request().DeleteAsync();
    }
}
