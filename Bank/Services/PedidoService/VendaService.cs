/* using IntroAPI.Domain.Pedido;

namespace IntroAPI.Services.PedidoService
{
    public class VendaService
    {
        private readonly BD _bd;
        private readonly ILogger<VendaService> _logger;

        public VendaService(BD bd, ILogger<VendaService> logger)
        {
            _bd = bd;
            _logger = logger;
        }

        public (bool, string) Criar(Domain.Pedido.Venda venda) {

            bool sucesso = false;
            int novoId = 0;

            if (venda.Total != CalcularTotal(venda))
            {
                return (false, "Total não bate");
            }


            try
            {
                var conexao = _bd.CriarConexao();
                conexao.Open();

                MySql.Data.MySqlClient.MySqlTransaction transacao = conexao.BeginTransaction();

                try
                {
                    var cmd = conexao.CreateCommand();

                    cmd.CommandText = "INSERT INTO Venda  (Data, Total) Values (@Data, @Total)";
                    cmd.Parameters.AddWithValue("@Data", venda.Data);
                    cmd.Parameters.AddWithValue("@Total", venda.Total);
                    cmd.ExecuteNonQuery();

                    novoId = (int)cmd.LastInsertedId;

                    foreach (var item in venda.Itens)
                    {
                        cmd.Parameters.Clear();
                        cmd.CommandText = $@"INSERT INTO VendaItem  (VendaId, ProdutoId, Preco, Quantidade, ValorFinal) 
                                              Values (@VendaId, @ProdutoId, @Preco, @Quantidade, @ValorFinal)";
                        
                        cmd.Parameters.AddWithValue("@VendaId", novoId);
                        cmd.Parameters.AddWithValue("@ProdutoId", item.Produto.Id);
                        cmd.Parameters.AddWithValue("@Preco", item.Preco);
                        cmd.Parameters.AddWithValue("@Quantidade", item.Quantidade);
                        cmd.Parameters.AddWithValue("@ValorFinal", item.ValorFinal);

                        cmd.ExecuteNonQuery();

                        item.Id = (int)cmd.LastInsertedId;
                    }

                    transacao.Commit();
                    sucesso = true;

                    venda.Id = novoId;
                }
                catch (Exception ex)
                {
                    transacao.Rollback();

                    _logger.LogError(ex, "Erro ao Criar a Venda.");
                    throw new Exception(ex.Message);

                }
                finally
                {
                    conexao.Close();
                }
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Erro ao iniciar a criação da venda.");
                throw new Exception(ex.Message);
            }

            return (sucesso, "");

        }

        public bool Alterar(Domain.Pedido.Venda venda)
        {
            bool sucesso = false;
            try
            {
                var conexao = _bd.CriarConexao();
                conexao.Open();

                MySql.Data.MySqlClient.MySqlTransaction transacao = conexao.BeginTransaction();
              
                try
                {
                    var cmd = conexao.CreateCommand();

                    cmd.CommandText = $@"UPDATE Venda 
                                         SET Data = @Data, Total = @Total
                                         WHERE VendaId = @VendaId"
                                         ;
                    cmd.Parameters.AddWithValue("@Data", venda.Data);
                    cmd.Parameters.AddWithValue("@Total", venda.Total);
                    cmd.Parameters.AddWithValue("@VendaId", venda.Id);

                    cmd.ExecuteNonQuery();

                    //excluindo os itens que não estão presentes
                    var idsManter = string.Join(",", venda.Itens.Where(p => p.Id > 0).Select(p => p.Id).ToList());

                    cmd.CommandText = $@"DELETE FROM VendaItem   
                                         WHERE VendaId = @VendaId AND
                                               VendaItemId not in ({idsManter}) ";


                    foreach (var item in venda.Itens)
                    {
                        cmd.Parameters.Clear();

                        if (item.Id == 0)
                        {
                            cmd.CommandText = $@"INSERT INTO VendaItem  (VendaId, ProdutoId, Preco, Quantidade, ValorFinal) 
                                                 Values (@VendaId, @ProdutoId, @Preco, @Quantidade, @ValorFinal)";
                        }
                        else
                        {
                            cmd.CommandText = $@"UPDATE VendaItem 
                                                 SET 
                                                     VendaId = @VendaId,
                                                     ProdutoId = @ProdutoId,
                                                     Preco = @Preco,
                                                     Quantidade = @Quantidade,
                                                     ValorFinal = @ValorFinal) 
                                                WHERE VendaItemId = @VendaItemId";

                            cmd.Parameters.AddWithValue("@VendaItemId", item.Id);

                        }

                        cmd.Parameters.AddWithValue("@VendaId", venda.Id);
                        cmd.Parameters.AddWithValue("@ProdutoId", item.Produto.Id);
                        cmd.Parameters.AddWithValue("@Preco", item.Preco);
                        cmd.Parameters.AddWithValue("@Quantidade", item.Quantidade);
                        cmd.Parameters.AddWithValue("@ValorFinal", item.ValorFinal);


                        cmd.ExecuteNonQuery();


                        if (item.Id == 0)
                            item.Id = (int)cmd.LastInsertedId;

                    }

                    transacao.Commit();
                    sucesso = true;

                }
                catch (Exception ex)
                {
                    transacao.Rollback();

                    _logger.LogError(ex, "Erro ao atualizar a Venda.");
                    throw new Exception(ex.Message);

                }
                finally
                {
                    conexao.Close();
                }
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Erro ao iniciar a atualização da venda.");
                throw new Exception(ex.Message);
            }

            return sucesso;

        }

        public bool Excluir(int id)
        {
            bool sucesso = false;
            try
            {
                var conexao = _bd.CriarConexao();
                conexao.Open();

                MySql.Data.MySqlClient.MySqlTransaction transacao = conexao.BeginTransaction();

                try
                {
                    var cmd = conexao.CreateCommand();

                    cmd.CommandText = "DELETE FROM VendaItem where VendaId = @vendaId";
                    cmd.Parameters.AddWithValue("@vendaId", id);
                    cmd.ExecuteNonQuery();

                    //cmd.Parameters.Clear();
                    cmd.CommandText = "DELETE FROM Venda where VendaId = @vendaId";
                    //cmd.Parameters.AddWithValue("@vendaId", id);
                    cmd.ExecuteNonQuery();


                    transacao.Commit();
                    sucesso = true;
                }
                catch (Exception ex)
                {
                    transacao.Rollback();

                    _logger.LogError(ex, "Erro ao Excluir a Venda.");
                    throw new Exception(ex.Message);

                }
                finally
                {
                    conexao.Close();
                }
            }
            catch (Exception ex) {

                _logger.LogError(ex, "Erro ao iniciar a exclusão da venda.");
                throw new Exception(ex.Message);
            }

            return sucesso;
        }

        public bool RemoverItem(int itemId) {

            bool sucesso = false;

            var conexao = _bd.CriarConexao();

            try
            {

                conexao.Open();

                var cmd = conexao.CreateCommand();

                cmd.CommandText = "DELETE FROM VendaItem where VendaItemId = @vendaItemId";
                cmd.Parameters.AddWithValue("@vendaItemId", itemId);
                cmd.ExecuteNonQuery();

                sucesso = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao Excluir a Venda.");
                throw new Exception(ex.Message);
            }
            finally
            {
                conexao.Close();
            }
           

            return sucesso;
        }

        private decimal CalcularTotal(Venda venda)
        {
            decimal total = 0;

            foreach (var item in venda.Itens)
            {
                total += item.Preco * item.Quantidade;
            }

            return total;
        }

        public int QuantidadeVenda(DateTime data)
        {

            return 0;

        }

        public IEnumerable<Domain.Pedido.Venda> ObterTodos()
        {
            var vendas = new List<Venda>();
           
            try
            {
                var conexao = _bd.CriarConexao();
                conexao.Open();

                try
                {
                    var cmd = conexao.CreateCommand();

                    cmd.CommandText = $@"select * 
                                         from Venda v
                                         join VendaItem vi
                                           on v.VendaId = vi.VendaId
                                         join Produto p on p.ProdutoId = vi.ProdutoId";
                    cmd.ExecuteNonQuery();

                    var dr = cmd.ExecuteReader();

                    while(dr.Read())
                    {
                        Venda venda = vendas.Find(vf => vf.Id == Convert.ToInt32(dr["VendaId"]));
                        if (venda == null)
                        {
                            venda = new Venda();
                            venda.Id = Convert.ToInt32(dr["VendaId"]);
                            venda.Data = Convert.ToDateTime(dr["Data"]);
                            venda.Total = Convert.ToDecimal(dr["Total"]);
                            venda.Itens = new List<VendaItem>();

                            vendas.Add(venda);  
                        }

                        //itens.
                        VendaItem vendaItem = new VendaItem();
                        vendaItem.Id = Convert.ToInt32(dr["VendaItemId"]);
                        vendaItem.Quantidade = Convert.ToDecimal(dr["Quantidade"]);
                        vendaItem.Preco = Convert.ToDecimal(dr["Preco"]);
                        vendaItem.ValorFinal = Convert.ToDecimal(dr["ValorFinal"]);
                        vendaItem.Produto = new()
                        {
                            Id = Convert.ToInt32(dr["ProdutoId"]),
                            Nome = dr["Nome"].ToString()
                        };

                        venda.Itens.Add(vendaItem);
                       // vendaItem.Produto.Id = Convert.ToInt32(dr["ProdutoId"]);
                       // vendaItem.Produto.Nome = dr["Nome"].ToString();
                    }


                }
                catch (Exception ex)
                {

                    _logger.LogError(ex, "Erro ao Obter as Vendas.");
                    throw new Exception(ex.Message);

                }
                finally
                {
                    conexao.Close();
                }
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "...");
                throw new Exception(ex.Message);
            }

            return vendas;

        }
    }
}
*/