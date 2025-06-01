using BackendHtml.Models;
using Microsoft.AspNetCore.Mvc;

namespace BackendHtml.Controllers;

public class BaseController : Controller
{
    SiteProvider? provider;
    public SiteProvider Provider => provider ??= HttpContext.RequestServices.GetRequiredService<SiteProvider>(); //new SiteProvider(HttpContext.RequestServices.GetService<IConfiguration>());   
}



