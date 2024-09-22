/* using IntroAPI.Domain;
using IntroAPI.Services;
using IntroAPI.ViewModel;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace IntroAPI.Controllers
{

    // Restful
    // 1 - Comunicação usando verbos HTTP (get, post, put, patch, delete, etc)
    // 2 - Retornar um código HTTP (200, 201...)
    // 3 - Padronizar retornos.


    /// <summary>
    /// Endpoints para gerenciar alunos.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AlunosController : ControllerDefault
    {

        private readonly Services.AlunoService _alunoService;

        public AlunosController(Services.AlunoService alunoService)
        {
            _alunoService = alunoService;
            //_alunoService = new AlunoService();
        }

        /// <summary>
        /// Obtém todos os alunos
        /// </summary>
        /// <returns>Uma lista de alunos.</returns>
        [HttpGet]
        public IActionResult ObterTodos()
        {
            var alunos = _alunoService.ObterTodos();
            return Ok(alunos);

        }

        /// <summary>
        /// Obtém um aluno.
        /// </summary>
        /// <param name="id">Id do Aluno</param>
        /// <returns>Retorna o aluno.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Obter(int id)
        {

            var aluno = _alunoService.Obter(id);

            if (aluno != null)
                return Ok(aluno);
            else return NotFound();

            ////objeto anônimo
            //var retorno = new
            //{
            //    sucesso = true,
            //    alunos = new List<AlunoCriarViewModel>(),
            //    mensagem = "deu certo"
            //};

            //if (id == 10)
            //    return Ok(retorno);
            //else return NotFound(retorno);

        }

        [HttpGet("{id}/{cpf}")]
        public IActionResult Obter3(int id, int cpf)
        {

            return Ok("teste teste teste " + id + " " + cpf);

        }

        [HttpGet()]
        [Route("[action]")]
        public IActionResult Obter2(int id)
        {

            return Ok("teste teste teste " + id);

        }

        //[HttpGet()]
        //[Route("matriculados")]
        //public IActionResult ObterTodosMatriculados()
        //{

        //    return Ok("Matriculados");

        //}

        [HttpGet()]
        [Route("matriculados/{ano?}")]
        public IActionResult ObterTodosMatriculados(int? ano)
        {

            return Ok("Matriculados XXXX " + ano);

        }


        //[HttpPost]
        //public IActionResult Gravar(JsonElement dados)
        //{
        //    string nome = dados.GetProperty("nome").GetString();
        //    int idade = dados.GetProperty("idade").GetInt32();

        //    return Ok("gravou");
        //}

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public IActionResult Criar(AlunoCriarViewModel alunoVM)
        {



            Aluno aluno = new Aluno();
            aluno.Nome = alunoVM.Nome;
            aluno.DataNascimento = alunoVM.DataNascimento;
            aluno.Cidade = new Cidade();
            aluno.Cidade.Id = alunoVM.CidadeId;

            try
            {
                var sucesso = _alunoService.Criar(aluno);

                if (sucesso)
                {
                    //faz isso
                }
                else
                {
                    //faz isso
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

            return Ok(aluno); //200
            //return Created(" http://unoeste.br/aluno/obter",new { id= 10}); //201

            //return UnprocessableEntity(); //422 Utilizado para indicar erros de semântica ou validação dados da solitação.
            //return StatusCode(500, "deu erro...");
            
        }

        [HttpPut("{id}")]
        public IActionResult Alterar(int id, AlunoAlterarViewModel alunoVM)
        {

            return Ok("Alterou");
        }

        [HttpPatch("{id}")]
        public IActionResult AlterarNome(int id, [FromBody] JsonElement dados)
        {

            return Ok("Alterou" + dados.GetProperty("nome").GetString());
        }



        [HttpDelete("{id}")]
        public IActionResult Excluir(int id)
        {
            _alunoService.Excluir(id);

            return Ok("Excluir " + id);
        }





    }
}
*/