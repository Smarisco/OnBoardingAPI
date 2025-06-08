using OnboardingAPI.Models;
using OnboardingAPI.ServiceInterface;
using System.Data.SqlClient;

namespace OnboardingAPI.Services
{
    public class AcaoService : IAcaoService
    {
        private readonly string _connectionString;

        public AcaoService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public IEnumerable<Acao> GetAll()
        {
            var acoes = new List<Acao>();
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            var command = new SqlCommand("SELECT * FROM Acao", connection);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                acoes.Add(new Acao
                {
                    Id = (int)reader["Id"],
                    Descricao = reader["Descricao"].ToString()
                });
            }
            return acoes;
        }

        public Acao GetById(int id)
        {
            Acao acao = null;
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            var command = new SqlCommand("SELECT * FROM Acao WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                acao = new Acao
                {
                    Id = (int)reader["Id"],
                    Descricao = reader["Descricao"].ToString()
                };
            }
            return acao;
        }

        public void Create(Acao acao)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            var command = new SqlCommand("INSERT INTO Acao (Descricao) VALUES (@Descricao)", connection);
            command.Parameters.AddWithValue("@Descricao", acao.Descricao);
            command.ExecuteNonQuery();
        }

        public void Update(int id, Acao acao)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            var command = new SqlCommand("UPDATE Acao SET Descricao = @Descricao WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Descricao", acao.Descricao);
            command.Parameters.AddWithValue("@Id", id);
            command.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            var command = new SqlCommand("DELETE FROM Acao WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);
            command.ExecuteNonQuery();
        }
       
    }
}
