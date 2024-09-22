namespace IntroAPI.Domain.Pedido
{
    public class VendaItem
    {
        public int Id { get; set; }
        public Produto Produto { get; set; }
        public decimal Preco { get; set; }
        public decimal Quantidade { get; set; }
        public decimal ValorFinal { get; set; }
    }

}
