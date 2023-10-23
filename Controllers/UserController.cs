using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ProjetoIBGE.Data;
using ProjetoIBGE.Extensions;
using ProjetoIBGE.Models;
using ProjetoIBGE.Services;
using ProjetoIBGE.ViewModels;
using SecureIdentity.Password;

namespace ProjetoIBGE.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {



        [HttpGet("v1/users")]
        public async Task<IActionResult> Get(
   [FromServices] IBGEDataContext context)
        {

            try
            {
                var users = await context.User
                    .AsNoTracking()
                    .Select(x => new
                    {
                        Id = x.Id,
                        Nome = x.Name
                    }).ToListAsync();
                return Ok(users);
            }
            catch(Exception ex)
            {
                return StatusCode(500, new ResultViewModel<List<User>>("Falha no servidor. Erro:" + ex.Message));
            }
        }


        [HttpPost("v1/users/login")]
        public async Task<IActionResult> Login(
            [FromServices] IBGEDataContext context,
            [FromServices] TokenService tokenService,
             [FromBody] LoginViewModel model
             )
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResultViewModel<string>(ModelState.GetErros()));
            var user = await context.User
              .AsNoTracking()
              .FirstOrDefaultAsync(x => x.Name == model.Login);

            if (user == null)
                return StatusCode(401, new ResultViewModel<string>("Usuario ou senha invalida!"));

            var hash = PasswordHasher.Hash(model.PassWord);
            if (!PasswordHasher.Verify(user.PassWordHash, model.PassWord))
                return StatusCode(401, new ResultViewModel<string>("Usuario ou senha invalida!"));

            try
            {
                var token = tokenService.GenerateToken(user);
                return Ok(new ResultViewModel<string>(token, null));
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<string>("05x04 - Falha interna"));

            }
        }

        [HttpPost("v1/users")]
        public async Task<IActionResult> Post([FromServices] IBGEDataContext context, [FromBody] UserViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResultViewModel<User>(ModelState.GetErros()));

            if (model.Password != model.PasswordConfirmation)
                return BadRequest(new ResultViewModel<User>("POST_USER_PASSWORD - Senhas não conferem"));
            try
            {
                var user = new User();
                user.Name = model.Name;
               
                user.PassWordHash = PasswordHasher.Hash(model.Password);
                user.Id = Guid.NewGuid();
                user.IsActive = true;
                user.DateCreate = DateTime.UtcNow;

                await context.AddAsync(user);
                await context.SaveChangesAsync();

                user.PassWordHash = model.Password;
                return Created($"v1/users/{user.Id}", new ResultViewModel<User>(user));
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new ResultViewModel<User>("POST_USER - Não foi possivel adicionar um novo usuario"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultViewModel<User>("POST_USER_SERVER - Falha interna no servidor"));
            }
        }
    }


}
