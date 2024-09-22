
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace Bank.Services
{
    public class CartaoService
    {
        private readonly ILogger<CartaoService> _logger;
        private readonly Bank.BD _bd;

        public CartaoService(ILogger<CartaoService> logger, BD bd)
        {
            _logger = logger;
            _bd = bd;
        }

        public bool ValidaCartao(string numeroCartao)
        {
            MySqlConnection conexao = _bd.CriarConexao();
            try
            {
                conexao.Open();

                MySqlCommand cmd = conexao.CreateCommand();

                cmd.CommandText = @$"select * 
                                 from Cartao where Numero  = {numeroCartao}  AND Validade > NOW();";
             

                var dr = cmd.ExecuteReader();
                return dr.HasRows;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao ValidaCartao.");

            }
            finally
            {
                conexao.Close();

            }
            return false;

        

        }
    }
}