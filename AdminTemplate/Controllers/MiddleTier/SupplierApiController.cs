using AdminTemplate.DTOs;
using AdminTemplate.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminTemplate.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierApiController : ControllerBase
    {
        private readonly ISupplierService _service;

        public SupplierApiController(ISupplierService service)
        {
            _service = service;
        }

        // GET: api/SupplierApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SupplierDto>>> GetAll()
        {
            try
            {
                var suppliers = await _service.GetAllAsync();
                var dtos = new List<SupplierDto>();

                foreach (var supplier in suppliers)
                {
                    dtos.Add(new SupplierDto
                    {
                        Id = supplier.Id,
                        SupplierName = supplier.SupplierName,
                        ContactNumber = supplier.ContactNumber,
                        Email = supplier.Email,
                        Address = supplier.Address
                    });
                }

                return Ok(dtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving suppliers.", error = ex.Message });
            }
        }

        // GET: api/SupplierApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SupplierDto>> GetById(int id)
        {
            try
            {
                var supplier = await _service.GetByIdAsync(id);
                if (supplier == null)
                {
                    return NotFound(new { message = $"Supplier with ID {id} not found." });
                }

                var dto = new SupplierDto
                {
                    Id = supplier.Id,
                    SupplierName = supplier.SupplierName,
                    ContactNumber = supplier.ContactNumber,
                    Email = supplier.Email,
                    Address = supplier.Address
                };

                return Ok(dto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the supplier.", error = ex.Message });
            }
        }

        // POST: api/SupplierApi
        [HttpPost]
        public async Task<ActionResult<SupplierDto>> Create([FromBody] SupplierDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _service.AddAsync(dto);

                return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the supplier.", error = ex.Message });
            }
        }

        // PUT: api/SupplierApi/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] SupplierDto dto)
        {
            try
            {
                if (id != dto.Id)
                {
                    return BadRequest(new { message = "ID mismatch between route and body." });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existing = await _service.GetByIdAsync(id);
                if (existing == null)
                {
                    return NotFound(new { message = $"Supplier with ID {id} not found." });
                }

                await _service.UpdateAsync(dto);

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the supplier.", error = ex.Message });
            }
        }

        // DELETE: api/SupplierApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var supplier = await _service.GetByIdAsync(id);
                if (supplier == null)
                {
                    return NotFound(new { message = $"Supplier with ID {id} not found." });
                }

                await _service.DeleteAsync(id);

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the supplier.", error = ex.Message });
            }
        }

        // POST: api/SupplierApi/bulk
        [HttpPost("bulk")]
        public async Task<ActionResult<BulkInsertResult>> BulkCreate([FromBody] List<SupplierDto> suppliers)
        {
            try
            {
                if (suppliers == null || suppliers.Count == 0)
                {
                    return BadRequest(new { message = "No suppliers provided." });
                }

                var result = new BulkInsertResult
                {
                    TotalReceived = suppliers.Count,
                    SuccessCount = 0,
                    FailureCount = 0,
                    Errors = new List<string>()
                };

                foreach (var dto in suppliers)
                {
                    try
                    {
                        // Validate each supplier
                        if (string.IsNullOrWhiteSpace(dto.SupplierName))
                        {
                            result.FailureCount++;
                            result.Errors.Add($"Supplier name is required.");
                            continue;
                        }

                        await _service.AddAsync(dto);
                        result.SuccessCount++;
                    }
                    catch (Exception ex)
                    {
                        result.FailureCount++;
                        result.Errors.Add($"Failed to insert '{dto.SupplierName}': {ex.Message}");
                    }
                }

                if (result.SuccessCount == 0)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during bulk insert.", error = ex.Message });
            }
        }
    }

    public class BulkInsertResult
    {
        public int TotalReceived { get; set; }
        public int SuccessCount { get; set; }
        public int FailureCount { get; set; }
        public List<string> Errors { get; set; }
    }
}