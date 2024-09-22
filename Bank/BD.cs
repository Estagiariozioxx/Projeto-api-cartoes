using MySql.Data.MySqlClient;

namespace Bank
{
    public class BD
    {
        private readonly string _connectionString;

        public BD(string connectionString)
        {
            _connectionString = connectionString;
        }

        public MySqlConnection CriarConexao()
        {
            MySqlConnection conexao = new MySqlConnection(_connectionString);
            return conexao;
        }
    }
}
