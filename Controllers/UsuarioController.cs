using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Bibliotec.Contexts;
using Bibliotec.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Bibliotec_mvc.Controllers
{
    [Route("[controller]")]
    public class UsuarioController : Controller
    {
        private readonly ILogger<UsuarioController> _logger;

        //Criando um obj de classe
        Context context = new Context();

        public UsuarioController(ILogger<UsuarioController> logger)
        {
            _logger = logger;
        }


        //O metodo esta retornando a View Usuario/Index.cshtml
        public IActionResult Index()
        {
            //Pegar as informacoes da session que sao nescessarias para que aparece os detalhes do meu usuario.
            int id = int.Parse(HttpContext.Session.GetString("UsuarioID")!);
            ViewBag.Admin= HttpContext.Session.GetString("Admin");

            // Busquei o usuario que esta logado (Beatriz)
            Usuario usuarioEncontrado = context.Usuario.FirstOrDefault(usuario => usuario.UsuarioID == id)!;
            //Se nao for encontrado ninguem
            if (usuarioEncontrado == null)
            {
                return NotFound();
            }

            //Procurar o curso que meu usuarioEncontrado esta cadastrado
            Curso cursoEncontrado = context.Curso.FirstOrDefault(curso => curso.CursoID == usuarioEncontrado.UsuarioID)!;

            //Verificar se o usuario possui ou nao o curso
            if(cursoEncontrado == null){
                //O usuario nao possui curso cadastrafo - manda essa mensagem pra view
                ViewBag.Curso = "O usuario nao possui curso cadastrado";
            }else{
                //O usuario possui o curso xxx
                ViewBag.Curso = cursoEncontrado.Nome;
            }

            ViewBag.Nome = usuarioEncontrado.Nome;
            ViewBag.Email = usuarioEncontrado.Email;
            ViewBag.Contato = usuarioEncontrado.Contato;
            ViewBag.DtNascimento = usuarioEncontrado.DtNascimento.ToString("dd/MM/yyyy");  

            return View();
        }



        // [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        // public IActionResult Error()
        // {
        //     return View("Error!");
        // }
    }
}