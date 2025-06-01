namespace BackendHtml.Controllers;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Auth.OAuth2.Web;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[Route("oauth2")]
public class OAuthController : Controller
{
    private readonly IConfiguration _configuration;

    public OAuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet("authorize")]
    public IActionResult Authorize()
    {
        var clientId = _configuration["GooglePhotos:ClientId"];
        var clientSecret = _configuration["GooglePhotos:ClientSecret"];
        var redirectUri = Url.Action("Callback", "OAuth", null, Request.Scheme);

        var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
        {
            ClientSecrets = new ClientSecrets
            {
                ClientId = clientId,
                ClientSecret = clientSecret
            },
            Scopes = new[] { "https://www.googleapis.com/auth/photoslibrary.appendonly" },
        });

        var authUrl = flow.CreateAuthorizationCodeRequest(redirectUri).Build().AbsoluteUri;
        return Redirect(authUrl);
    }

    [HttpGet("callback")]
    public async Task<IActionResult> Callback(string code)
    {
        if (string.IsNullOrEmpty(code))
            return BadRequest("Authorization code is missing.");

        var clientId = _configuration["GooglePhotos:ClientId"];
        var clientSecret = _configuration["GooglePhotos:ClientSecret"];
        var redirectUri = Url.Action("Callback", "OAuth", null, Request.Scheme);

        var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
        {
            ClientSecrets = new ClientSecrets
            {
                ClientId = clientId,
                ClientSecret = clientSecret
            },
            Scopes = new[] { "https://www.googleapis.com/auth/photoslibrary.appendonly" },
        });

        try
        {
            var token = await flow.ExchangeCodeForTokenAsync(null, code, redirectUri, CancellationToken.None);
            // Lưu token vào session hoặc database để sử dụng sau
            HttpContext.Session.SetString("GooglePhotosToken", token.AccessToken);
            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error during OAuth callback: {ex.Message}");
        }
    }
}