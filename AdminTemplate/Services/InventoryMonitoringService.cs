using AdminTemplate.Data;
using AdminTemplate.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace AdminTemplate.Services
{
    public class InventoryMonitoringService : IInventoryMonitoringService
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly ILogger<InventoryMonitoringService> _logger;

        public InventoryMonitoringService(
            ApplicationDbContext context,
            IEmailService emailService,
            ILogger<InventoryMonitoringService> logger)
        {
            _context = context;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task CheckInventoryLevelsAsync()
        {
            _logger.LogInformation("Starting inventory level check at {Time}", DateTime.UtcNow);

            try
            {
                // Get all active users' emails
                var userEmails = await _context.Users
                    .Where(u => u.IsEmailVerified)
                    .Select(u => u.Email)
                    .ToListAsync();

                if (!userEmails.Any())
                {
                    _logger.LogWarning("No verified users found to send notifications");
                    return;
                }

                // Check for items with CurrentQuantity = 0
                var outOfStockItems = await _context.Inventories
                    .Include(i => i.Category)
                    .Include(i => i.Supplier)
                    .Where(i => i.Status == "active" && i.CurrentQuantity == 0)
                    .ToListAsync();

                // Check for items with CurrentQuantity = 5
                var lowStockItems = await _context.Inventories
                    .Include(i => i.Category)
                    .Include(i => i.Supplier)
                    .Where(i => i.Status == "active" && i.CurrentQuantity == 5)
                    .ToListAsync();

                // Send out of stock notifications
                if (outOfStockItems.Any())
                {
                    await SendOutOfStockNotifications(outOfStockItems, userEmails);
                }

                // Send low stock notifications
                if (lowStockItems.Any())
                {
                    await SendLowStockNotifications(lowStockItems, userEmails);
                }

                _logger.LogInformation(
                    "Inventory check completed. Out of stock: {OutOfStock}, Low stock: {LowStock}",
                    outOfStockItems.Count,
                    lowStockItems.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking inventory levels");
                throw;
            }
        }

        private async Task SendOutOfStockNotifications(List<Inventory> items, List<string> recipients)
        {
            var subject = "🚨 URGENT: Inventory Items Out of Stock";
            var body = BuildOutOfStockEmailBody(items);

            await _emailService.SendBulkEmailAsync(recipients, subject, body);

            _logger.LogInformation("Sent out of stock notifications for {Count} items to {Recipients} recipients",
                items.Count, recipients.Count);
        }

        private async Task SendLowStockNotifications(List<Inventory> items, List<string> recipients)
        {
            var subject = "⚠️ WARNING: Inventory Items Running Low";
            var body = BuildLowStockEmailBody(items);

            await _emailService.SendBulkEmailAsync(recipients, subject, body);

            _logger.LogInformation("Sent low stock notifications for {Count} items to {Recipients} recipients",
                items.Count, recipients.Count);
        }

        private string BuildOutOfStockEmailBody(List<Inventory> items)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<html><body style='font-family: Arial, sans-serif;'>");
            sb.AppendLine("<h2 style='color: #dc3545;'>🚨 URGENT: Items Out of Stock</h2>");
            sb.AppendLine("<p>The following inventory items are <strong>completely out of stock</strong> and require immediate restocking:</p>");
            sb.AppendLine("<table style='border-collapse: collapse; width: 100%; margin: 20px 0;'>");
            sb.AppendLine("<thead>");
            sb.AppendLine("<tr style='background-color: #dc3545; color: white;'>");
            sb.AppendLine("<th style='border: 1px solid #ddd; padding: 12px; text-align: left;'>Item Name</th>");
            sb.AppendLine("<th style='border: 1px solid #ddd; padding: 12px; text-align: left;'>Category</th>");
            sb.AppendLine("<th style='border: 1px solid #ddd; padding: 12px; text-align: left;'>Supplier</th>");
            sb.AppendLine("<th style='border: 1px solid #ddd; padding: 12px; text-align: center;'>Location</th>");
            sb.AppendLine("<th style='border: 1px solid #ddd; padding: 12px; text-align: center;'>Reorder Level</th>");
            sb.AppendLine("</tr>");
            sb.AppendLine("</thead>");
            sb.AppendLine("<tbody>");

            foreach (var item in items)
            {
                sb.AppendLine("<tr>");
                sb.AppendLine($"<td style='border: 1px solid #ddd; padding: 10px;'><strong>{item.ItemName}</strong></td>");
                sb.AppendLine($"<td style='border: 1px solid #ddd; padding: 10px;'>{item.Category?.Name ?? "N/A"}</td>");
                sb.AppendLine($"<td style='border: 1px solid #ddd; padding: 10px;'>{item.Supplier?.SupplierName ?? "N/A"}</td>");
                sb.AppendLine($"<td style='border: 1px solid #ddd; padding: 10px; text-align: center;'>{item.Location ?? "N/A"}</td>");
                sb.AppendLine($"<td style='border: 1px solid #ddd; padding: 10px; text-align: center;'>{item.ReorderLevel} {item.Unit}</td>");
                sb.AppendLine("</tr>");
            }

            sb.AppendLine("</tbody>");
            sb.AppendLine("</table>");
            sb.AppendLine("<p style='color: #dc3545; font-weight: bold;'>⚠️ Please take immediate action to restock these items.</p>");
            sb.AppendLine("<hr style='margin: 30px 0;'/>");
            sb.AppendLine($"<p style='color: #666; font-size: 12px;'>This is an automated notification sent on {DateTime.Now:MMMM dd, yyyy 'at' hh:mm tt}</p>");
            sb.AppendLine("</body></html>");

            return sb.ToString();
        }

        private string BuildLowStockEmailBody(List<Inventory> items)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<html><body style='font-family: Arial, sans-serif;'>");
            sb.AppendLine("<h2 style='color: #ffc107;'>⚠️ WARNING: Items Running Low on Stock</h2>");
            sb.AppendLine("<p>The following inventory items have reached a <strong>critically low quantity of 5 units</strong> and need to be restocked soon:</p>");
            sb.AppendLine("<table style='border-collapse: collapse; width: 100%; margin: 20px 0;'>");
            sb.AppendLine("<thead>");
            sb.AppendLine("<tr style='background-color: #ffc107; color: #000;'>");
            sb.AppendLine("<th style='border: 1px solid #ddd; padding: 12px; text-align: left;'>Item Name</th>");
            sb.AppendLine("<th style='border: 1px solid #ddd; padding: 12px; text-align: left;'>Category</th>");
            sb.AppendLine("<th style='border: 1px solid #ddd; padding: 12px; text-align: left;'>Supplier</th>");
            sb.AppendLine("<th style='border: 1px solid #ddd; padding: 12px; text-align: center;'>Current Qty</th>");
            sb.AppendLine("<th style='border: 1px solid #ddd; padding: 12px; text-align: center;'>Location</th>");
            sb.AppendLine("<th style='border: 1px solid #ddd; padding: 12px; text-align: center;'>Reorder Level</th>");
            sb.AppendLine("</tr>");
            sb.AppendLine("</thead>");
            sb.AppendLine("<tbody>");

            foreach (var item in items)
            {
                sb.AppendLine("<tr>");
                sb.AppendLine($"<td style='border: 1px solid #ddd; padding: 10px;'><strong>{item.ItemName}</strong></td>");
                sb.AppendLine($"<td style='border: 1px solid #ddd; padding: 10px;'>{item.Category?.Name ?? "N/A"}</td>");
                sb.AppendLine($"<td style='border: 1px solid #ddd; padding: 10px;'>{item.Supplier?.SupplierName ?? "N/A"}</td>");
                sb.AppendLine($"<td style='border: 1px solid #ddd; padding: 10px; text-align: center; color: #dc3545; font-weight: bold;'>5 {item.Unit}</td>");
                sb.AppendLine($"<td style='border: 1px solid #ddd; padding: 10px; text-align: center;'>{item.Location ?? "N/A"}</td>");
                sb.AppendLine($"<td style='border: 1px solid #ddd; padding: 10px; text-align: center;'>{item.ReorderLevel} {item.Unit}</td>");
                sb.AppendLine("</tr>");
            }

            sb.AppendLine("</tbody>");
            sb.AppendLine("</table>");
            sb.AppendLine("<p style='color: #856404; font-weight: bold;'>📋 Please review and place orders for these items to avoid stock-outs.</p>");
            sb.AppendLine("<hr style='margin: 30px 0;'/>");
            sb.AppendLine($"<p style='color: #666; font-size: 12px;'>This is an automated notification sent on {DateTime.Now:MMMM dd, yyyy 'at' hh:mm tt}</p>");
            sb.AppendLine("</body></html>");

            return sb.ToString();
        }
    }
}