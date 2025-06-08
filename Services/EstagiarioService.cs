using OnboardingAPI.Models;
using OnboardingAPI.ServiceInterface;
using System.Data.SqlClient;

namespace OnboardingAPI.Services
{
    public class EstagiarioService : IEstagiarioService
    {

        private readonly string _connectionString;

        public EstagiarioService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public IEnumerable<Estagiario> GetAll()
        {
            var estagiarios = new List<Estagiario>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT * FROM Estagiario", connection);
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    estagiarios.Add(new Estagiario
                    {
                        Id = (int)reader["Id"],
                        Nome = reader["Nome"].ToString()
                    });
                }
            }

            return estagiarios;
        }

        public Estagiario GetById(int id)
        {
            Estagiario estagiario = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT * FROM Estagiario WHERE Id = @Id", connection);
                command.Parameters.AddWithValue("@Id", id);

                var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    estagiario = new Estagiario
                    {
                        Id = (int)reader["Id"],
                        Nome = reader["Nome"].ToString()
                    };
                }
            }

            return estagiario;
        }

        public void Create(Estagiario estagiario)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("INSERT INTO Estagiario (Nome) VALUES (@Nome)", connection);
                command.Parameters.AddWithValue("@Nome", estagiario.Nome);
                command.ExecuteNonQuery();
            }
        }

        public void Update(int id, Estagiario estagiario)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("UPDATE Estagiario SET Nome = @Nome WHERE Id = @Id", connection);
                command.Parameters.AddWithValue("@Nome", estagiario.Nome);
                command.Parameters.AddWithValue("@Id", id);
                command.ExecuteNonQuery();
            }
        }

        public void Delete(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("DELETE FROM Estagiario WHERE Id = @Id", connection);
                command.Parameters.AddWithValue("@Id", id);
                command.ExecuteNonQuery();
            }
        }
    }
}
