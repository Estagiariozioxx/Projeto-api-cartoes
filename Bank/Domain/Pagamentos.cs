using Bank.ViewModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Bank.Domain
{
    public enum Situacao
    {
        Pendente = 1,
        Confirmado = 2,
        Cancelado = 3

    }
    public class Pagamentos
    {
        public int transacaoid {  get; set; }
        
        public decimal valor { get; set; }
        
        public string numeroCartao { get; set; }
      
        public string CVV { get; set; }
       
        public int quantidadeParcelas { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Situacao situacao { get; set; }
    }
}
