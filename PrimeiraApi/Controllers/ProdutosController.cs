using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PrimeiraApi.Data;
using PrimeiraApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace PrimeiraApi.Controllers
{
    [Authorize] //[AllowAnonymous] abre e desfaz a authorização em metodos específicos
    [ApiController]
    [Route("api/produtos")]
    public class ProdutosController : ControllerBase
    {
        private readonly ApiDbContext _context;

        public ProdutosController(ApiDbContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<IEnumerable<Produto>>> GetProdutos()
        {
            if(_context.Produtos == null)
            {
                return NotFound();
            }

            return await _context.Produtos.ToListAsync();
        }

        [Authorize]
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<Produto>> GetProduto(int id)
        {
            if (_context.Produtos == null)
            {
                return NotFound();
            }

            var produto = await _context.Produtos.FindAsync(id);

            if(produto == null)
            {
                return NotFound();
            }

            return produto;
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<Produto>> PostProduto(Produto produto)
        {
            if (_context.Produtos == null)
            {
                return Problem("Erro ao tentar criar um produto, contate o suporte", statusCode: 500);
            }

            if (!ModelState.IsValid)
            {
                //return BadRequest(ModelState);
                
                /*return ValidationProblem(new ValidationProblemDetails(ModelState)
                {
                    Title = "Um ou mais erros ocorreram durante a validação de envio do produto.",
                    Status = 400
                });*/

                return ValidationProblem(ModelState);
            }

            _context.Add(produto);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetProduto), new { id = produto.Id }, produto);
        }

        [Authorize]
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> PutProduto(int id, Produto produto)
        {
            if(id != produto.Id) return BadRequest();

            if(!ModelState.IsValid) return ValidationProblem(ModelState);

            //_context.Produtos.Update(produto);

            _context.Entry(produto).State = EntityState.Modified;

            //Evitar problemas de concorrência
            try
            {
                await _context.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException)
            {
                if(!ProdutoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            
            return NoContent();
        }

        private bool ProdutoExists(int id)
        {
            return _context.Produtos.Any(e => e.Id == id);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> DeleteProduto(int id)
        {
            if (_context.Produtos == null)
            {
                return NotFound();
            }

            var produto = await _context.Produtos.FindAsync(id);

            if (produto == null)
            {
                return NotFound();
            }

            _context.Produtos.Remove(produto);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
