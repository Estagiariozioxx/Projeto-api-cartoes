using System.ComponentModel.DataAnnotations;

namespace Bank.ViewModel
{

    public class PagamentosViewModel
    {

        [Required]
        public decimal valor { get; set; }
        [Required]
        public string numeroCartao { get; set; }
        [Required]
        public int CVV { get; set; }
        [Required]
        public int quantidadeParcelas { get; set; }
        
    }
}
