using MySql.Data.MySqlClient;

namespace Bank
{
    public class BD
    {
        public MySql.Data.MySqlClient.MySqlConnection CriarConexao()
        {
            string strCon = Environment.GetEnvironmentVariable("STRING_CONEXAO");
            MySqlConnection conexao = new MySqlConnection(strCon);
            return conexao;
        }
    }
}
