using Bank.ViewModel;
using Bank.Services;
using Bank.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;


namespace Bank.Controllers
{

    /// <summary>
    /// Endpoints para Pagamentos.
    /// </summary>
    [Authorize("APIAuth")]
    [Route("pagamentos/")]
    [ApiController]
    public class PagamentosController : Controller
    {

        private readonly Services.PagamentoService _pagamentoService;
        private readonly Services.CartaoService _cartaoService;

        public PagamentosController(Services.CartaoService cartaoService, Services.PagamentoService pagamentoService)
        {
            _pagamentoService = pagamentoService;
            _cartaoService = cartaoService;
            //_cartaoService = new CartaoService();
        }

        /// <summary>
        /// Calcula o valor das parcelas de um pagamento.
        /// </summary>
        /// <param name="request">O modelo contendo os dados necessários para o cálculo das parcelas.</param>
        /// <returns>Uma lista de parcelas calculadas.</returns>
        /// <response code="200">Retorna a lista de parcelas calculadas.</response>
        /// <response code="400">Retorna erros de validação se os dados de entrada forem inválidos.</response>
        [HttpPost("calcular-parcelas")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        public IActionResult CalcularParcelas(CalcularParcelasViewModel request)
        {
            if (request.ValorTotal <= 0)
            {
                ApiError error = new ApiError { Field = "Valor Total", Message = "O valor total deve ser maior que zero." };
                return BadRequest(ApiResponse<string>.ErrorResponse("Erro ao processar a requisição.", error));
            }

            if (request.TaxaJuros < 0)
            {
                ApiError error = new ApiError { Field = "Taxa de Juros", Message = "A taxa de juros não pode ser negativa.." };
                return BadRequest(ApiResponse<string>.ErrorResponse("Erro ao processar a requisição.", error));
            }

            if (request.QuantidadeParcelas <= 0)
            {
                ApiError error = new ApiError { Field = "Quantidade de Parcelas", Message = "A quantidade de parcelas deve ser maior que zero" };
                return BadRequest(ApiResponse<string>.ErrorResponse("Erro ao processar a requisição.", error));
            }

            var parcelas = new List<object>();
            decimal juros = request.ValorTotal * request.TaxaJuros;

            //decimal valorComJuros = juros + request.ValorTotal;
            decimal valorParcelasComJuros = juros / request.QuantidadeParcelas;

            for (int i = 1; i <= request.QuantidadeParcelas; i++)
            {
                parcelas.Add(new
                {
                    parcela = i,
                    valor = Math.Round(valorParcelasComJuros, 2)


                });
            }

            return Ok(ApiResponse<object>.SuccessResponse(parcelas, "Parcelas calculada com sucessso"));


        }



        /// <summary>
        /// Processa o pagamento de uma transação.
        /// </summary>
        /// <param name="request">O modelo contendo os dados necessários para processar o pagamento.</param>
        /// <returns>Um objeto contendo o ID da transação se o pagamento for criado com sucesso.</returns>
        /// <response code="201">Pagamento criado com sucesso.</response>
        /// <response code="400">Erro de validação, como cartão inválido ou dados incorretos.</response>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        public IActionResult Pagamentos(PagamentosViewModel request)
        {
            try
            {
                if (_cartaoService.ValidaCartao(request.numeroCartao))
                {

                    Pagamentos pagamentos = new Pagamentos();
                    pagamentos.CVV = request.CVV.ToString();
                    pagamentos.numeroCartao = request.numeroCartao;
                    pagamentos.valor = request.valor;
                    pagamentos.quantidadeParcelas = request.quantidadeParcelas;

                    if (_pagamentoService.CriarPagamento(pagamentos))
                    {
                        var response = new { transacao = pagamentos.transacaoid };
                        return StatusCode(201, ApiResponse<object>.SuccessResponse(response, "Pagamento criado com sucesso."));
                    }

                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }


            ApiError error = new ApiError { Field = "cartao", Message = "O número do cartão é inválido." };

            return BadRequest(ApiResponse<string>.ErrorResponse("Erro ao processar a requisição.", error));

        }



        /// <summary>
        /// Obtém a situação de um pagamento específico pelo ID.
        /// </summary>
        /// <param name="id">O ID do pagamento para o qual se deseja obter a situação.</param>
        /// <returns>Retorna a situação do pagamento.</returns>
        /// <response code="200">Retorna a situação do pagamento como uma string.</response>
        /// <response code="400">Retorna um erro de validação se o ID do pagamento não for encontrado.</response>
        [HttpGet("{id}/situacao")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        public IActionResult ObterSituacao(int id)
        {
            Pagamentos pagamento = null;

            try
            {
                pagamento = _pagamentoService.ObterPagamento(id);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }



            if (pagamento!=null)
            {
                return StatusCode(201, ApiResponse<string>.SuccessResponse(pagamento.situacao.ToString(), "Situação o pagamento"));

            }
            ApiError error = new ApiError { Field = "Id", Message = "Não foi possível localizar este pagamento" };

            return BadRequest(ApiResponse<string>.ErrorResponse("Erro ao processar a requisição.", error));

        }

        /// <summary>
        /// Confirma o pagamento do ID informado, trocando a situação para "confirmado/2".
        /// </summary>
        /// <param name="id">O ID do pagamento que deve ser confirmado.</param>
        /// <returns>Retorna um status code 200 se a confirmação foi bem-sucedida, ou BadRequest se não foi possível confirmar o pagamento.</returns>
        /// <response code="200">Confirmação bem-sucedida do pagamento.</response>
        /// <response code="400">Erro ao processar a requisição ou pagamento não encontrado.</response>
        [HttpPut("{id}/confirmar")]
        [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        public IActionResult ConfirmarPagamento(int id)
        {
            try
            {
                if (_pagamentoService.AlterarStatus(id, "3", 2))
                {
                    return StatusCode(201, ApiResponse<int>.SuccessResponse(id, "Pagamento alterado com sucesso."));

                }

                ApiError error = new ApiError { Field = "Id", Message = "Não foi possível alterar este pagamento" };

                return BadRequest(ApiResponse<string>.ErrorResponse("Erro ao processar a requisição.", error));

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }


        }


        /// <summary>
        /// Confirma o pagamento do ID informado, trocando a situação para "cancelado/3".
        /// </summary>
        /// <param name="id">O ID do pagamento que deve ser confirmado.</param>
        /// <returns>Retorna um status code 200 se a confirmação foi bem-sucedida, ou BadRequest se não foi possível confirmar o pagamento.</returns>
        /// <response code="200">Confirmação bem-sucedida do pagamento.</response>
        /// <response code="400">Erro ao processar a requisição ou pagamento não encontrado.</response>
        [HttpPut("{id}/cancelar")]
        [ProducesResponseType(typeof(ApiResponse<int>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        public IActionResult CancelarPagamento(int id)
        {
            try
            {
                if (_pagamentoService.AlterarStatus(id, "2", 3))
                {
                    return StatusCode(201, ApiResponse<int>.SuccessResponse(id, "Pagamento alterado com sucesso."));

                }

                ApiError error = new ApiError { Field = "Id", Message = "Não foi possível alterar este pagamento" };

                return BadRequest(ApiResponse<string>.ErrorResponse("Erro ao processar a requisição.", error));

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }


        }


    }
}
