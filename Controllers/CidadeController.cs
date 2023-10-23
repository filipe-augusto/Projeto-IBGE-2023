using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ProjetoIBGE.Data;
using ProjetoIBGE.Extensions;
using ProjetoIBGE.Models;
using ProjetoIBGE.ViewModels;
using System.Runtime.Intrinsics;

namespace ProjetoIBGE.Controllers
{
    [ApiController]
    [Authorize]
    public class CidadeController : ControllerBase
    {

        //[HttpGet("v1/cidades")]
        //public List<Cidade> GetList([FromServices] IBGEDataContext context)
        //{
        //    return context.Cidade.ToList();
        //}
        [Authorize]
        [HttpPost("v1/cidades")]
        public async Task<IActionResult> Post([FromServices] IBGEDataContext context, [FromBody] Cidade model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResultViewModel<Cidade>(ModelState.GetErros()));

            try
            {
                var JaExisteCidadeComEsseNome = context.Cidade.Count(x => x.State == model.State && x.City == model.City) > 0;
                if (JaExisteCidadeComEsseNome)
                    return NotFound(new ResultViewModel<Cidade>("Já existe uma cidade com esse nome nesse estado."));

                await context.AddAsync(model);
                await context.SaveChangesAsync();
                return Created($"v1/ibge/{model.Id}", new ResultViewModel<Cidade>(model));
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new ResultViewModel<Cidade>("POST_IBGE - Não foi possivel adicionar um novo registro"));
            }
            catch 
            {
                return StatusCode(500, new ResultViewModel<Cidade>("POST_IBGE_SERVER - Falha interna no servidor"));
            }
        }
       
        [Authorize]
        [HttpPut("v1/cidades/{id}")]
        public async Task<IActionResult> Put([FromServices] IBGEDataContext context, [FromBody] Cidade model,
            [FromRoute] string id)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResultViewModel<Cidade>(ModelState.GetErros()));
        
            try
            {
                var registro = await context.Cidade.FirstOrDefaultAsync(x => x.Id == id.ToString());
                if (registro == null)
                    return NotFound(new ResultViewModel<Cidade>("Registro não encontrado"));

                var JaExisteCidadeComEsseNome = context.Cidade.Count(x => x.State == model.State && x.City == model.City) > 0;
                if (JaExisteCidadeComEsseNome)
                    return NotFound(new ResultViewModel<Cidade>("Já existe uma cidade com esse nome nesse estado."));


                registro.State = model.State;
                registro.City = model.City;
                context.Cidade.Update(registro);
                await context.SaveChangesAsync();

                return Created($"v1/ibge/{model.Id}", new ResultViewModel<Cidade>(model));
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new ResultViewModel<Cidade>("Não foi possivel adicionar um novo registro"));
            }
            catch 
            {
                return StatusCode(500, new ResultViewModel<Cidade>("Falha no servidor"));
            }
        }
        
        [Authorize]
        [HttpDelete("v1/cidades/{id:int}")]
        public async Task<IActionResult> Delete([FromServices] IBGEDataContext context,
            [FromRoute] int id)
        {
            var registro = await context.Cidade.FirstOrDefaultAsync(x => x.Id == id.ToString());
            if (registro == null)
                return NotFound(new ResultViewModel<Cidade>("Registro não encontrado"));

            context.Cidade.Remove(registro);
           await context.SaveChangesAsync();

            return Ok(new ResultViewModel<Cidade>(registro));
        }


        [HttpGet("v1/cidades")]
        [Authorize]
        public async Task<IActionResult> GetList(
        [FromServices] IBGEDataContext context,[FromServices] IMemoryCache cache)
        {
            try
            {
                var cities = cache.GetOrCreate("CidadeCache", entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
                    return GetCitiesCache(context);
                });
           
                return Ok(cities);
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<List<Cidade>>("Falha no servidor"));
            }
        }
        private List<Cidade> GetCitiesCache(IBGEDataContext context)
        {
            return context.Cidade.ToList();
        }

        [Authorize]
        [HttpGet("v1/cidades/por-codigo/{codigo}")]
        public async Task<IActionResult> GetListWithCode(
            [FromServices] IBGEDataContext context, [FromRoute] string codigo)
        {

            if (string.IsNullOrEmpty(codigo))
                return NotFound(new ResultViewModel<Cidade>("Não foi informado nenhum texto."));

            if (codigo.Length < 7)
                return NotFound(new ResultViewModel<Cidade>("Codigo incompleto"));
            try
            {
                var cities = await context.Cidade
                    .AsNoTracking()
                    .Where(x => x.Id == codigo)
                    .Select(x => new
                    {
                        Codigo = x.Id,
                        Cidade = x.City,
                        Estado = x.State,

                    })
                    .ToListAsync();
                return Ok(cities);
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<List<Cidade>>("Falha no servidor"));
            }
        }

        [Authorize]
        [HttpGet("v1/cidades/por-estado/{state}")]
        public async Task<IActionResult> GetListWithState(
        [FromServices] IBGEDataContext context,
        [FromRoute] string state, 
        [FromQuery] int pagina = 0,
        [FromQuery] int paginaTamanho = 25 )
        {
            if (string.IsNullOrEmpty(state))
                return NotFound(new ResultViewModel<Cidade>("Não foi informado nenhum texto."));
            try
            {
                var cities = await context.Cidade
                    .AsNoTracking()
                    .Where(x => x.State == state)
                    .Select(x => new 
                    {
                        Codigo = x.Id,
                        Cidade = x.City,
                        Estado = x.State,
                        
                    })
                    .Skip(pagina * paginaTamanho)
                    .Take(paginaTamanho)
                    .ToListAsync();
                var total = await context.Cidade.CountAsync(x => x.State ==state);
                var result = new
                {
                    TotalCidades = total,
                    Cidadades = cities
                };
                return Ok(result);
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<List<Cidade>>(" Erro no servidor"));
            }
        }
       
        [Authorize]
        [HttpGet("v1/cidades/por-cidade/{city}")]
        public async Task<IActionResult> GetListWithCity(
         [FromServices] IBGEDataContext context,
         [FromRoute] string city)
        {

            if (string.IsNullOrEmpty(city))
                return NotFound(new ResultViewModel<Cidade>("Não foi informado nenhum texto."));
            try
            {
                var cities = await context.Cidade
                    .AsNoTracking()
                    .Where(x=>x.City.Contains(city.Trim()))
                    .Select(x => new
                    {
                        Codigo = x.Id,
                        Cidade = x.City,
                        Estado = x.State,
                    })
           
                    .ToListAsync();


                var total = await context.Cidade.CountAsync(x => x.City.Contains(city.Trim()));
                var result = new
                {
                    TotalCidades = total,
                    Cidadades = cities
                };
                return Ok(result);
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<List<Cidade>>("Falha no servidor"));
            }
        }


    }
}
