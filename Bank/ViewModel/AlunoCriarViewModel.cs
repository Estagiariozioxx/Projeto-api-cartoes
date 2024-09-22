using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace IntroAPI.ViewModel
{
    public class AlunoCriarViewModel
    {
        /// <summary>
        /// Nome do Aluno
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Nome { get; set; }
     
        [Required]
        public DateTime DataNascimento { get; set; }

        [Required]
        public int CidadeId { get; set; }
    }
}
