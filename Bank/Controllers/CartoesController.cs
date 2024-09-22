using Microsoft.AspNetCore.Mvc;
using Bank.Services;
using Microsoft.AspNetCore.Authorization;

namespace Bank.Controllers
{

    /// <summary>
    /// Endpoints para cartoes.
    /// </summary>
    [Authorize("APIAuth")]
    [Route("cartoes/")]
    [ApiController]
    public class CartoesController : Controller
    {


        private readonly Services.CartaoService _cartaoService;

        public CartoesController(Services.CartaoService cartaoService)
        {
            _cartaoService = cartaoService;
            //_cartaoService = new CartaoService();
        }


        /// <summary>
        /// Obtém a bandeira do cartão com base nos primeiros 4 dígitos e o 8º dígito.
        /// </summary>
        /// <param name="cartao">O número do cartão de crédito (deve ter pelo menos 8 dígitos).</param>
        /// <returns>Retorna a bandeira do cartão, se válida, ou uma mensagem de erro.</returns>
        /// <response code="200">Retorna a bandeira do cartão (VISA, MASTERCARD, ELO).</response>
        /// <response code="400">Retorna um erro de validação se o número do cartão for inválido.</response>
        /// <response code="404">Retorna um erro se a bandeira do cartão não for encontrada.</response>
        [HttpGet("{cartao}/obter-bandeira")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        public IActionResult ObterBandeira(string cartao)
        {
            ApiError error = new ApiError { Field = "cartao", Message = "O número do cartão é inválido." };
            cartao = cartao.Replace("-", "");
            if (cartao.Length != 13)
            {
                return BadRequest(ApiResponse<string>.ErrorResponse("Erro ao processar a requisição.", error));
            }
            string primeiros4Digitos = cartao.Substring(0, 4);
            char OitvoDigito = cartao[8];

            switch (primeiros4Digitos)
            {

                case "1111":
                    if (OitvoDigito == '1')
                        return Ok(ApiResponse<string>.SuccessResponse("VISA", "Bandeira do cartão obtida com sucesso."));
                    break;
                case "2222":
                    if (OitvoDigito == '2')
                        return Ok(ApiResponse<string>.SuccessResponse("MASTERCARD", "Bandeira do cartão obtida com sucesso."));
                    break;
                case "3333":
                    if (OitvoDigito == '3')
                        return Ok(ApiResponse<string>.SuccessResponse("ELO", "Bandeira do cartão obtida com sucesso."));
                    break;
            }

            return BadRequest(ApiResponse<string>.ErrorResponse("Erro ao processar a requisição.", error));

        }



        /// <summary>
        /// Verifica se o cartão de crédito existe e está dentro da validade.
        /// </summary>
        /// <param name="cartao">O número do cartão de crédito (deve ter exatamente 16 dígitos).</param>
        /// <returns>Retorna true se o cartão existir e estiver válido, caso contrário, retorna false.</returns>
        /// <response code="200">Retorna um objeto ApiResponse contendo true ou false, indicando a validade do cartão.</response>
        /// <response code="400">Retorna um erro de validação se o número do cartão for inválido.</response>
        [HttpGet("{cartao}/valido")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        public IActionResult ValidaCartao(string cartao)
        {
            bool sucesso = false;

            if (cartao.Length != 16)
            {
                ApiError error = new ApiError { Field = "cartao", Message = "O número do cartão é inválido." };
                return BadRequest(ApiResponse<string>.ErrorResponse("Erro ao processar a requisição.", error));
            }
            try
            {
                sucesso = _cartaoService.ValidaCartao(cartao);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }


            if (!sucesso)
            {
                return Ok(ApiResponse<string>.SuccessResponse(false, "Cartão não existente"));
            }
            return Ok(ApiResponse<string>.SuccessResponse(true, "Cartão existente"));

        }
    }
}
