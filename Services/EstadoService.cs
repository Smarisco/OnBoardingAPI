using OnboardingAPI.Models;
using OnboardingAPI.ServiceInterface;
using System.Data.SqlClient;

namespace OnboardingAPI.Services
{
    public class EstadoService : IEstadoService
    {
        private readonly string _connectionString;

        public EstadoService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public IEnumerable<Estado> GetAll()
        {
            var estados = new List<Estado>();
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            var command = new SqlCommand("SELECT * FROM Estado", connection);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                estados.Add(new Estado
                {
                    Id = (int)reader["Id"],
                    Descricao = reader["Descricao"].ToString()
                });
            }
            return estados;
        }

        public Estado GetById(int id)
        {
            Estado estado = null;
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            var command = new SqlCommand("SELECT * FROM Estado WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                estado = new Estado
                {
                    Id = (int)reader["Id"],
                    Descricao = reader["Descricao"].ToString()
                };
            }
            return estado;
        }

        public void Create(Estado estado)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            var command = new SqlCommand("INSERT INTO Estado (Descricao) VALUES (@Descricao)", connection);
            command.Parameters.AddWithValue("@Descricao", estado.Descricao);
            command.ExecuteNonQuery();
        }

        public void Update(int id, Estado estado)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            var command = new SqlCommand("UPDATE Estado SET Descricao = @Descricao WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Descricao", estado.Descricao);
            command.Parameters.AddWithValue("@Id", id);
            command.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            var command = new SqlCommand("DELETE FROM Estado WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);
            command.ExecuteNonQuery();
        }


    }
}
