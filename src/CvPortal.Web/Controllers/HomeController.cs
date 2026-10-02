using System.Diagnostics;
using CvPortal.Application.DTOs;
using CvPortal.Application.Services.Interfaces;
using CvPortal.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace CvPortal.Web.Controllers;

/// <summary>
/// Herkese açık CV sayfası.
/// </summary>
public class HomeController(ICvService cvService, IContactMessageService messageService) : Controller
{
    public async Task<IActionResult> Index(CancellationToken ct)
        => View(new HomeViewModel { Cv = await cvService.GetCvAsync(ct) });

    [HttpPost]
    [ValidateAntiForgeryToken]
    [EnableRateLimiting("contact")]
    public async Task<IActionResult> Contact([Bind(Prefix = nameof(HomeViewModel.Contact))] CreateContactMessageDto contact, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            ViewData["ScrollTo"] = "contact";
            return View(nameof(Index), new HomeViewModel { Cv = await cvService.GetCvAsync(ct), Contact = contact });
        }

        await messageService.CreateAsync(contact, ct);
        TempData["ContactSuccess"] = "Mesajınız iletildi. En kısa sürede dönüş yapacağım, teşekkürler!";
        return Redirect(Url.Action(nameof(Index)) + "#contact");
    }

    [Route("Home/StatusCode")]
    public IActionResult StatusCodePage(int code)
    {
        Response.StatusCode = code;
        return View("StatusCode", code);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
        => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}
