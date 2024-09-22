namespace IntroAPI.Domain.Pedido
{
    public class Venda
    {
        public int Id { get; set; }
        public DateTime Data { get; set; } = DateTime.Now;
        public decimal Total {  get; set; }
        public List<VendaItem> Itens { get; set; } = new List<VendaItem>();

        public Venda()
        {
            Itens = new List<VendaItem>();
        }

    }
}
