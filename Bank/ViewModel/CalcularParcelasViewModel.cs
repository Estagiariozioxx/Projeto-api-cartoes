using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Bank.ViewModel
{
    public class CalcularParcelasViewModel
    {
        /*
        /// <summary>
        /// Nome do Aluno
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Nome { get; set; }
     
        [Required]
        public DateTime DataNascimento { get; set; }

        [Required]
        public int CidadeId { get; set; }*/
        [Required]
        public decimal ValorTotal { get; set; }
        [Required]
        public decimal TaxaJuros { get; set; }
        [Required]
        public int QuantidadeParcelas { get; set; }




    }
}
