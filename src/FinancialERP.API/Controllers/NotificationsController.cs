using FinancialERP.Business.Services.Abstract;
using FinancialERP.DataAccess.Repositories.Abstract;
using FinancialERP.Entity.Models;
using Microsoft.AspNetCore.Mvc;

namespace FinancialERP.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly IFinancialAnalysisService _analysisService;
        private readonly IGenericRepository<Notification> _notificationRepo;

        public NotificationsController(
            IFinancialAnalysisService analysisService,
            IGenericRepository<Notification> notificationRepo)
        {
            _analysisService = analysisService;
            _notificationRepo = notificationRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetNotifications()
        {
            var notifications = await _notificationRepo.FindAsync(n => !n.IsRead);
            return Ok(notifications.OrderByDescending(n => n.CreatedAt));
        }

        [HttpGet("smart")]
        public async Task<IActionResult> GetSmartNotifications()
        {
            var recommendations = await _analysisService.GenerateSmartNotificationsAsync();
            return Ok(recommendations);
        }

        [HttpPost("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var notification = await _notificationRepo.GetByIdAsync(id);
            if (notification == null) return NotFound();

            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
            _notificationRepo.Update(notification);
            await _notificationRepo.SaveChangesAsync();
            return Ok();
        }
    }
}
