using OnboardingAPI.Models;
using OnboardingAPI.ServiceInterface;
using System.Data.SqlClient;

namespace OnboardingAPI.Services
{
    public class InteracaoService :IInteracaoService
    {
        private readonly string _connectionString;

        public InteracaoService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public IEnumerable<Interacao> GetAll()
        {
            var interacoes = new List<Interacao>();
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            var command = new SqlCommand("SELECT * FROM Interacao", connection);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                interacoes.Add(new Interacao
                {
                    Id = (int)reader["Id"],
                    EstagiarioId = (int)reader["EstagiarioId"],
                    EstadoAtualId = (int)reader["EstadoAtualId"],
                    AcaoTomadaId = (int)reader["AcaoTomadaId"],
                    ProximoEstadoId = (int)reader["ProximoEstadoId"],
                    RecompensaRecebida = Convert.ToSingle(reader["RecompensaRecebida"]),
                    DataInteracao = Convert.ToDateTime(reader["DataInteracao"])
                });
            }
            return interacoes;
        }

        public Interacao GetById(int id)
        {
            Interacao interacao = null;
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            var command = new SqlCommand("SELECT * FROM Interacao WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                interacao = new Interacao
                {
                    Id = (int)reader["Id"],
                    EstagiarioId = (int)reader["EstagiarioId"],
                    EstadoAtualId = (int)reader["EstadoAtualId"],
                    AcaoTomadaId = (int)reader["AcaoTomadaId"],
                    ProximoEstadoId = (int)reader["ProximoEstadoId"],
                    RecompensaRecebida = Convert.ToSingle(reader["RecompensaRecebida"]),
                    DataInteracao = Convert.ToDateTime(reader["DataInteracao"])
                };
            }
            return interacao;
        }

        public void Create(Interacao interacao)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            var command = new SqlCommand(
                "INSERT INTO Interacao (EstagiarioId, EstadoAtualId, AcaoTomadaId, ProximoEstadoId, RecompensaRecebida, DataInteracao) " +
                "VALUES (@EstagiarioId, @EstadoAtualId, @AcaoTomadaId, @ProximoEstadoId, @RecompensaRecebida, @DataInteracao)", connection);

            command.Parameters.AddWithValue("@EstagiarioId", interacao.EstagiarioId);
            command.Parameters.AddWithValue("@EstadoAtualId", interacao.EstadoAtualId);
            command.Parameters.AddWithValue("@AcaoTomadaId", interacao.AcaoTomadaId);
            command.Parameters.AddWithValue("@ProximoEstadoId", interacao.ProximoEstadoId);
            command.Parameters.AddWithValue("@RecompensaRecebida", interacao.RecompensaRecebida);
            command.Parameters.AddWithValue("@DataInteracao", interacao.DataInteracao);

            command.ExecuteNonQuery();
        }

        public void Update(int id, Interacao interacao)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            var command = new SqlCommand(
                "UPDATE Interacao SET EstagiarioId=@EstagiarioId, EstadoAtualId=@EstadoAtualId, AcaoTomadaId=@AcaoTomadaId, " +
                "ProximoEstadoId=@ProximoEstadoId, RecompensaRecebida=@RecompensaRecebida, DataInteracao=@DataInteracao " +
                "WHERE Id=@Id", connection);

            command.Parameters.AddWithValue("@EstagiarioId", interacao.EstagiarioId);
            command.Parameters.AddWithValue("@EstadoAtualId", interacao.EstadoAtualId);
            command.Parameters.AddWithValue("@AcaoTomadaId", interacao.AcaoTomadaId);
            command.Parameters.AddWithValue("@ProximoEstadoId", interacao.ProximoEstadoId);
            command.Parameters.AddWithValue("@RecompensaRecebida", interacao.RecompensaRecebida);
            command.Parameters.AddWithValue("@DataInteracao", interacao.DataInteracao);
            command.Parameters.AddWithValue("@Id", id);

            command.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            var command = new SqlCommand("DELETE FROM Interacao WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);
            command.ExecuteNonQuery();
        }
        public async Task<IEnumerable<Interacao>> ObterPorPeriodoAsync(DateTime inicio, DateTime fim)
        {
            var interacoes = new List<Interacao>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"SELECT Id, EstagiarioId, EstadoAtualId, AcaoTomadaId, ProximoEstadoId, RecompensaRecebida, DataInteracao
                      FROM Interacao
                      WHERE DataInteracao BETWEEN @Inicio AND @Fim";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Inicio", inicio);
                    command.Parameters.AddWithValue("@Fim", fim);

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            interacoes.Add(new Interacao
                            {
                                Id = reader.GetInt32(0),
                                EstagiarioId = reader.GetInt32(1),
                                EstadoAtualId = reader.GetInt32(2),
                                AcaoTomadaId = reader.GetInt32(3),
                                ProximoEstadoId = reader.GetInt32(4),
                                RecompensaRecebida = reader.GetDouble(5),
                                DataInteracao = reader.GetDateTime(6)
                            });
                        }
                    }
                }
            }

            return interacoes;
        }

    }
}
