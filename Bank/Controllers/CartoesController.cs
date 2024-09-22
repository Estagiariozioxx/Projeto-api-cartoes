using Microsoft.AspNetCore.Mvc;

namespace Bank.Controllers
{

    /// <summary>
    /// Endpoints para gerenciar alunos.
    /// </summary>
    ///[Obsolete]
    [Route("cartoes/")]
    [ApiController]
    public class CartoesController : Controller
    {


        /// <summary>
        /// Obtém todos os alunos
        /// </summary>
        /// <returns>Uma lista de alunos.</returns>
        [HttpGet("{cartao}/obter-bandeira")]
        public IActionResult ObterBandeira(string cartao)
        {
            string primeiros4Digitos = cartao.Substring(0, 4);
            char OitvoDigito = cartao[10];

            switch (primeiros4Digitos)
            {

                case "1111":
                    if(OitvoDigito == '1')
                    {
                        return Ok("VISA");
                    }
                 break;
                case "2222":
                    if (OitvoDigito == '2')
                        return Ok("MASTERCARD");
                    break;
                case "3333":
                    if (OitvoDigito == '3')
                        return Ok("ELO");
                    break;
                default:
                    return NotFound();
            }
            return Ok(OitvoDigito);


        }
    }
}
