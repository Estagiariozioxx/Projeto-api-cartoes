namespace IntroAPI.ViewModel.Pedido
{
    public class VendaCriarViewModel
    {
        public decimal Total {  get; set; }
        public List<VendaItemCriarViewModel> Itens { get; set; } = new List<VendaItemCriarViewModel>();

        public VendaCriarViewModel()
        {
            Itens = new List<VendaItemCriarViewModel>();
        }

    }
}
