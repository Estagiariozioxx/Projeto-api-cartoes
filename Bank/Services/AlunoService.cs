/* using IntroAPI.Domain;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace IntroAPI.Services
{
    public class AlunoService
    {
        private readonly ILogger<AlunoService> _logger;
        private readonly IntroAPI.BD _bd;

        public AlunoService(ILogger<AlunoService> logger, BD bd)
        {
            _logger = logger;
            _bd = bd;
        }


        public bool Criar(Domain.Aluno aluno)
        {
            bool sucesso = false;
            MySqlConnection conexao = _bd.CriarConexao();
            try
            {
                conexao.Open();

                MySqlCommand cmd = conexao.CreateCommand();

                cmd.CommandText = @"insert into Aluno (Nome, DataNascimento, CidadeId)
                                values (@Nome, @DataNascimento, @CidadeId)";

                cmd.Parameters.AddWithValue("@Nome", aluno.Nome);
                cmd.Parameters.AddWithValue("@DataNascimento", aluno.DataNascimento);
                cmd.Parameters.AddWithValue("@CidadeId", aluno.Cidade.Id);

                cmd.ExecuteNonQuery();//insert, delete, update, SP, function

                aluno.Id = (int)cmd.LastInsertedId;
                sucesso = true;
            }
            catch (Exception ex) {

                _logger.LogError(ex, "Erro ao criar o alunos.");
                throw new Exception(ex.Message);
            }
            finally
            {
                conexao.Close();
            }

            return sucesso;
        }


        public void Excluir(int id)
        {

            MySqlConnection conexao = _bd.CriarConexao();

            try
            {
                conexao.Open();

                var transacao = conexao.BeginTransaction();

                try
                {

                    MySqlCommand cmd = conexao.CreateCommand();

                    //cmd.CommandText = @$"delete from Matricula where AlunoId = {id}";
                    //cmd.ExecuteNonQuery();//insert, delete, update, SP, function


                    cmd.CommandText = @$"delete from Aluno where AlunoId = {id}";
                    cmd.ExecuteNonQuery();//insert, delete, update, SP, function

                    transacao.Commit();
                }
                catch (Exception ex)
                {
                    transacao.Rollback();
                    throw new Exception(ex.Message);

                }
                finally
                {
                    conexao.Close();
                }
            }
            catch (Exception ex) {

                throw new Exception(ex.Message);
            }

        }

        public Domain.Aluno Obter(int id)
        {

            Aluno aluno = null;

            MySqlConnection conexao = _bd.CriarConexao();

            conexao.Open();

            MySqlCommand cmd = conexao.CreateCommand();

            cmd.CommandText = @$"select * 
                                 from Aluno 
                                 where AlunoId = {id}";

            var dr = cmd.ExecuteReader();

            aluno = Map(dr).FirstOrDefault();

            //if (dr.Read())
            //{
            //    aluno = new Aluno();
            //    aluno.Id = (int)dr["AlunoId"];
            //    aluno.Nome = dr["Nome"].ToString();
            //    aluno.DataNascimento = Convert.ToDateTime(dr["DataNascimento"]);
            //    aluno.Cidade = new Cidade();
            //    aluno.Cidade.Id = Convert.ToInt32(dr["CidadeId"]);
            //}

            conexao.Close();

            return aluno;

        }


        public List<Domain.Aluno> ObterTodos()
        {

            List<Aluno> alunos = new();

            MySqlConnection conexao = _bd.CriarConexao();
            try
            {
                conexao.Open();

                MySqlCommand cmd = conexao.CreateCommand();

                cmd.CommandText = @$"select * 
                                 from Aluno";

                var dr = cmd.ExecuteReader();

                alunos = Map(dr);
                //while (dr.Read())
                //{
                //    var aluno = new Aluno();
                //    aluno.Id = (int)dr["AlunoId"];
                //    aluno.Nome = dr["Nome"].ToString();
                //    aluno.DataNascimento = Convert.ToDateTime(dr["DataNascimento"]);
                //    aluno.Cidade = new Cidade();
                //    aluno.Cidade.Id = Convert.ToInt32(dr["CidadeId"]);

                //    alunos.Add(aluno);
                //}

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter todos os alunos.");

            }
            finally
            {
                conexao.Close();

            }

            return alunos;

        }


        public int QuantidadeAlunos()
        {


            MySqlConnection conexao = _bd.CriarConexao();

            conexao.Open();

            MySqlCommand cmd = conexao.CreateCommand();

            cmd.CommandText = @$"select count(*) 
                                 from Aluno";


            int qtde = (int)cmd.ExecuteScalar();

            conexao.Close();

            return qtde;

        }


        private List<Domain.Aluno> Map(MySqlDataReader dr)
        {

            List<Domain.Aluno> alunos = new List<Domain.Aluno>();
            while (dr.Read())
            {
                var aluno = new Aluno();
                aluno.Id = (int)dr["AlunoId"];
                aluno.Nome = dr["Nome"].ToString();
                aluno.DataNascimento = Convert.ToDateTime(dr["DataNascimento"]);
                aluno.Cidade = new Cidade();
                aluno.Cidade.Id = Convert.ToInt32(dr["CidadeId"]);

                alunos.Add(aluno);
            }

            return alunos;

        }
    }

}
*/