using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CvPortal.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
[AutoValidateAntiforgeryToken]
public abstract class AdminControllerBase : Controller
{
    protected void Success(string message) => TempData["Success"] = message;
    protected void Error(string message) => TempData["Error"] = message;
}
