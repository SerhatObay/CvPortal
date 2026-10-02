using CvPortal.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CvPortal.Web.Areas.Admin.Controllers;

public class MessagesController(IContactMessageService messageService) : AdminControllerBase
{
    public async Task<IActionResult> Index(bool unread = false, CancellationToken ct = default)
    {
        ViewData["OnlyUnread"] = unread;
        return View(await messageService.GetAllAsync(unread, ct));
    }

    public async Task<IActionResult> Details(int id, CancellationToken ct)
    {
        var message = await messageService.GetByIdAsync(id, ct);
        if (message is null) return NotFound();

        if (!message.IsRead)
        {
            await messageService.SetReadStatusAsync(id, true, ct);
            message.IsRead = true;
        }

        return View(message);
    }

    [HttpPost]
    public async Task<IActionResult> MarkUnread(int id, CancellationToken ct)
    {
        if (await messageService.SetReadStatusAsync(id, false, ct)) Success("Mesaj okunmadı olarak işaretlendi.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        if (await messageService.DeleteAsync(id, ct)) Success("Mesaj silindi.");
        else Error("Mesaj bulunamadı.");

        return RedirectToAction(nameof(Index));
    }
}
