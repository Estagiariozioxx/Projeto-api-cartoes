
using Bank.Domain;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace Bank.Services
{
    public class PagamentoService
    {

        private readonly ILogger<CartaoService> _logger;
        private readonly Bank.BD _bd;

        public PagamentoService(ILogger<CartaoService> logger, BD bd)
        {
            _logger = logger;
            _bd = bd;
        }
        public bool CriarPagamento(Domain.Pagamentos pagamento)
        {
            bool sucesso = false;
            MySqlConnection conexao = _bd.CriarConexao();
            try
            {
                conexao.Open();

                MySqlCommand cmd = conexao.CreateCommand();

                cmd.CommandText = @"insert into Transacao (Valor, Cartao, CVV, Parcelas, Situacao)
                                values (@Valor, @Cartao, @CVV, @Parcelas, @Situacao)";

                cmd.Parameters.AddWithValue("@Valor", pagamento.valor);
                cmd.Parameters.AddWithValue("@Cartao", pagamento.numeroCartao);
                cmd.Parameters.AddWithValue("@CVV", pagamento.CVV);
                cmd.Parameters.AddWithValue("@Parcelas", pagamento.quantidadeParcelas);
                cmd.Parameters.AddWithValue("@Situacao", 1);

                cmd.ExecuteNonQuery();//insert, delete, update, SP, function

                pagamento.transacaoid = (int)cmd.LastInsertedId;
                sucesso = true;
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Erro ao criar o pagamento.");
                throw new Exception(ex.Message);
            }
            finally
            {
                conexao.Close();
            }

            return sucesso;
        }

        public Domain.Pagamentos ObterPagamento(int id)
        {

            Pagamentos pagamento = null;

            MySqlConnection conexao = _bd.CriarConexao();
            try
            {

                conexao.Open();

                MySqlCommand cmd = conexao.CreateCommand();

                cmd.CommandText = @$"select Situacao 
                                     from Transacao 
                                     where TransacaoId = {id}";

                var dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    pagamento = new Pagamentos();
                    pagamento.situacao = (Situacao)Convert.ToInt32(dr["Situacao"]);
                }

                return pagamento;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter o pagamento.");
                throw new Exception(ex.Message);
            }
            finally
            {
                conexao.Close();

            }

        }

        public bool AlterarStatus(int id, string situacaoVerifica,int situacaoAlterar)
        {

            MySqlConnection conexao = _bd.CriarConexao();
            bool sucesso = false;

            try
            {
                conexao.Open();

                MySqlCommand cmd = conexao.CreateCommand();


                cmd.CommandText = @$"select * from Transacao where TransacaoId = {id}";

                var dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    if(dr["Situacao"].ToString() != situacaoVerifica)
                    {
                        dr.Close();
                        cmd.CommandText = @$"UPDATE Transacao SET Situacao = {situacaoAlterar} WHERE TransacaoId = {id}";
                        cmd.ExecuteNonQuery();
                        sucesso = true;

                    }

                    

                }


            }
            catch (Exception ex)
            {
                    
                throw new Exception(ex.Message);

            }
            finally
            {
                conexao.Close();
            }
            return sucesso;

        }


    }
}
