using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;

namespace KT4.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly IConfiguration _configuration;
        private const string LogFilePath = "logs.txt";
        public UserController(IConfiguration configuration) { _configuration = configuration; }

        [HttpPut]
        public IActionResult UpdateUser(int id, string name, int age, string email)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "UPDATE Users SET Name = @Name, Age = @Age, Email = @Email WHERE Id = @Id";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        command.Parameters.AddWithValue("@Name", name);
                        command.Parameters.AddWithValue("@Age", age);
                        command.Parameters.AddWithValue("@Email", email);
                        connection.Open();
                        int result = command.ExecuteNonQuery();
                        if (result < 1)
                        {
                            return BadRequest("Update failed.");
                        }
                    }
                    return Ok("User data was successfully updated.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut]
        public IActionResult UpdateUserEmail(string name, string email)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "UPDATE Users SET Email = @Email WHERE Name = @Name";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Name", name);
                        connection.Open();
                        int result = command.ExecuteNonQuery();
                        if (result < 1)
                        {
                            return BadRequest("Update failed.");
                        }
                        return Ok("User data was successfully updated.");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> NameAgeTransaction(int id, string name, int age)
        {
            var startTime = DateTime.Now;
            using var db = new SqlConnection("Data Source=path.db");

            db.Open();

            using (var transaction = db.BeginTransaction())
            {
                string logmsg = $"{startTime:dd-MM-yyyy HH:mm:ss} - Request for user id={id}, name={name}, age={age}";
                try
                {
                    using (var command = db.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = "UPDATE Users SET Name = @Name WHERE Id = @Id";
                        command.Parameters.AddWithValue("@Name", name);
                        command.Parameters.AddWithValue("@Id", id);

                        await command.ExecuteNonQueryAsync();
                    }

                    using (var command = db.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = "UPDATE Users SET Age = @Age WHERE Id = @Id";
                        command.Parameters.AddWithValue("@Age", age);
                        command.Parameters.AddWithValue("@Id", id);

                        await command.ExecuteNonQueryAsync();
                    }

                    transaction.Commit();

                    logmsg += $" result: Success!";
                    LogToFile(logmsg);

                    return Ok("Name and Age was updated.");
                }
                catch (SqliteException dbEx)
                {
                    transaction.Rollback();

                    logmsg += $" result: Fail! error: {dbEx}";
                    LogToFile(logmsg);

                    return StatusCode(500, "Database Error!");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    logmsg += $" result: Fail! error: {ex}";
                    LogToFile(logmsg);

                    return StatusCode(500, ex.Message);
                }
            }
        }

        private void LogToFile(string msg)
        {
            using (var writer = System.IO.File.AppendText(LogFilePath)) 
            {
                writer.WriteLine(msg);
            }
        }
    }
}
