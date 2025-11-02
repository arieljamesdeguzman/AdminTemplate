using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace AdminTemplate.DTOs
{
    public class InventoryCategoryDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string? ImageUrl { get; set; }

        public IFormFile ImageFile { get; set; }
    }
}