using CvPortal.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CvPortal.Web.Areas.Admin.Controllers;

public class DashboardController(IDashboardService dashboardService) : AdminControllerBase
{
    public async Task<IActionResult> Index(CancellationToken ct)
        => View(await dashboardService.GetSummaryAsync(ct));
}
