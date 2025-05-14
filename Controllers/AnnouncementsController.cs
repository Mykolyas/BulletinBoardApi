using BulletinBoardApi.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace BulletinBoardApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnnouncementsController : Controller
    {
        private readonly IConfiguration _configuration;

        public AnnouncementsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? category = null, [FromQuery] string? subcategory = null)
        {
            var announcements = new List<Announcement>();

            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("GetAllAnnouncements", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            // Додаємо параметри до запиту
            cmd.Parameters.AddWithValue("@Category", (object?)category ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@SubCategory", (object?)subcategory ?? DBNull.Value);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                announcements.Add(new Announcement
                {
                    Id = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    Description = reader.GetString(2),
                    CreatedDate = reader.GetDateTime(3),
                    Status = reader.GetBoolean(4),
                    Category = reader.GetString(5),
                    SubCategory = reader.GetString(6)
                });
            }

            return Ok(announcements);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            using var cmd = new SqlCommand("GetAnnouncementById", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Id", id);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                var announcement = new Announcement
                {
                    Id = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    Description = reader.GetString(2),
                    CreatedDate = reader.GetDateTime(3),
                    Status = reader.GetBoolean(4),
                    Category = reader.GetString(5),
                    SubCategory = reader.GetString(6)
                };

                return Ok(announcement);
            }

            return NotFound();
        }

        [HttpGet("subcategories")]
        public IActionResult GetSubcategories([FromQuery] string category)
        {
            var subcategoriesByCategory = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
        { "Побутова техніка", new List<string> { "Холодильники", "Пральні машини", "Бойлери", "Печі", "Витяжки", "Мікрохвильові печі" } },
        { "Комп'ютерна техніка", new List<string> { "ПК", "Ноутбуки", "Монітори", "Принтери", "Сканери" } },
        { "Смартфони", new List<string> { "Android смартфони", "iOS/Apple смартфони" } },
        { "Інше", new List<string> { "Одяг", "Взуття", "Аксесуари", "Спортивне обладнання", "Іграшки" } }
            };

            if (string.IsNullOrWhiteSpace(category) || !subcategoriesByCategory.ContainsKey(category))
            {
                return Json(new List<string>());
            }

            return Json(subcategoriesByCategory[category]);
        }



        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Announcement announcement)
        {
            using SqlConnection conn = new(_configuration.GetConnectionString("DefaultConnection"));
            using SqlCommand cmd = new("AddAnnouncement", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Title", announcement.Title);
            cmd.Parameters.AddWithValue("@Description", announcement.Description);
            cmd.Parameters.AddWithValue("@Status", announcement.Status);
            cmd.Parameters.AddWithValue("@Category", announcement.Category);
            cmd.Parameters.AddWithValue("@SubCategory", announcement.SubCategory);

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();

            return Ok("Added successfully");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAnnouncement(int id, [FromBody] Announcement announcement)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var parameters = new
                {
                    Id = id,
                    Title = announcement.Title,
                    Description = announcement.Description,
                    Status = announcement.Status,
                    Category = announcement.Category,
                    SubCategory = announcement.SubCategory
                };

                var result = await connection.ExecuteAsync("UpdateAnnouncement", parameters, commandType: CommandType.StoredProcedure);
                return result > 0 ? Ok("Updated successfully") : NotFound("Announcement not found");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAnnouncement(int id)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var result = await connection.ExecuteAsync("DeleteAnnouncement", new { Id = id }, commandType: CommandType.StoredProcedure);
                return result > 0 ? Ok("Deleted successfully") : NotFound("Announcement not found");
            }
        }
    }
}
