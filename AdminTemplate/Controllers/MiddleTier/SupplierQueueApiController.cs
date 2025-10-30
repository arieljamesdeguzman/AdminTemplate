using AdminTemplate.DTOs;
using AdminTemplate.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdminTemplate.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierQueueApiController : ControllerBase
    {
        private readonly IMessageQueueService _queueService; // ✅ Changed interface
        private readonly ILogger<SupplierQueueApiController> _logger;

        public SupplierQueueApiController(
            IMessageQueueService queueService, // ✅ Changed interface
            ILogger<SupplierQueueApiController> logger)
        {
            _queueService = queueService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult> QueueSupplier([FromBody] SupplierDto supplier)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _queueService.PublishSupplierAsync(supplier);

                return Accepted(new
                {
                    message = "Supplier queued successfully",
                    supplierName = supplier.SupplierName,
                    status = "queued"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error queueing supplier");
                return StatusCode(500, new
                {
                    message = "Failed to queue supplier",
                    error = ex.Message
                });
            }
        }

        [HttpPost("bulk")]
        public async Task<ActionResult> QueueBulkSuppliers([FromBody] List<SupplierDto> suppliers)
        {
            try
            {
                if (suppliers == null || suppliers.Count == 0)
                {
                    return BadRequest(new { message = "No suppliers provided" });
                }

                await _queueService.PublishSuppliersAsync(suppliers);

                return Accepted(new
                {
                    message = "Suppliers queued successfully",
                    totalQueued = suppliers.Count,
                    status = "queued"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error queueing bulk suppliers");
                return StatusCode(500, new
                {
                    message = "Failed to queue suppliers",
                    error = ex.Message
                });
            }
        }

        [HttpGet("health")]
        public ActionResult GetHealth()
        {
            return Ok(new
            {
                status = "healthy",
                queueService = "rabbitmq",
                timestamp = DateTime.UtcNow
            });
        }
    }
}