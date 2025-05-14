using BulletinBoardApi.Models;
using System.Data;
using System.Data.SqlClient;

namespace BulletinBoardApi.Data
{
    public class AnnouncementRepository
    {
        private readonly string _connectionString;

        public AnnouncementRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task<IEnumerable<Announcement>> GetAllAsync()
        {
            var announcements = new List<Announcement>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("GetAllAnnouncements", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    await conn.OpenAsync();

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
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
                    }
                }
            }

            return announcements;
        }

        public async Task AddAsync(Announcement announcement)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("AddAnnouncement", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Title", announcement.Title);
                    cmd.Parameters.AddWithValue("@Description", announcement.Description);
                    cmd.Parameters.AddWithValue("@Status", announcement.Status);
                    cmd.Parameters.AddWithValue("@Category", announcement.Category);
                    cmd.Parameters.AddWithValue("@SubCategory", announcement.SubCategory);

                    await conn.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task UpdateAsync(Announcement announcement)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("UpdateAnnouncement", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Id", announcement.Id);
                    cmd.Parameters.AddWithValue("@Title", announcement.Title);
                    cmd.Parameters.AddWithValue("@Description", announcement.Description);
                    cmd.Parameters.AddWithValue("@Status", announcement.Status);
                    cmd.Parameters.AddWithValue("@Category", announcement.Category);
                    cmd.Parameters.AddWithValue("@SubCategory", announcement.SubCategory);

                    await conn.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task DeleteAsync(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("DeleteAnnouncement", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);

                    await conn.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
