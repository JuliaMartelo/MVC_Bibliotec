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
    public class LivroController : Controller
    {
        private readonly ILogger<LivroController> _logger;

        public LivroController(ILogger<LivroController> logger)
        {
            _logger = logger;
        }

        Context context => new Context();
        public IActionResult Index()
        {
            ViewBag.Admin = HttpContext.Session.GetString("Admin");

            //Criar uma lista de Livros
            List<Livro> listaLivros = context.Livro.ToList();

            //Verificar se o livro twm reserva ou nao 
            var livroReservado = context.LivroReserva.ToDictionary(livro => livro.LivroID, livror => livror.DtReserva);

            ViewBag.Livros = listaLivros;
            ViewBag.LivroComReserva = livroReservado;


            return View();
        }

        [Route("Cadastro")]
        //Metodo que retorna a tela de cadastro
        public IActionResult Cadastro()
        {
            ViewBag.Admin = HttpContext.Session.GetString("Admin");

            ViewBag.Categorias = context.Categoria.ToList();
            //Retorna a View de cadastro:
            return View();
        }

        //Metodo para cadastrar um livro:
        [Route("Cadastrar")]
        public IActionResult Cadastrar(IFormCollection form){

            //PRIMEIRA PARTE: Cadastrar um livro na tabela livro
            Livro novoLivro = new Livro();

            //Oque meu usuario escrever no formulario sera atribuido ao novoLivro:

            novoLivro.Nome = form["Nome"].ToString();
            novoLivro.Descricao = form["Descricao"].ToString();
            novoLivro.Editora = form["Editora"].ToString();
            novoLivro.Escritor = form["Escritor"].ToString();
            novoLivro.Idioma = form["Idioma"].ToString();

            //Trabalhar com idiomas
            if(form.Files.Count > 0){
                //Primeiro passo: 
                //Armazenamos o arquivo enviado pelo usuario
                var arquivo = form.Files[0];

                //Segundo passo:
                //Validaremos se a pasta que sera armazenada as imagens, existe. Caso nao exista, criaremos uma nova pasta

                var pasta = Path.Combine(Directory.GetCurrentDirectory(), "wwroot/images/Livros");

                if(Directory.Exists(pasta)){
                    Directory.CreateDirectory(pasta);
                }
                //Terceiro passo:
                //Criar a variavel para armazenar o caminho em que meu arquivo estara, alem do nome dele
                var caminho = Path.Combine(pasta, arquivo.FileName);

                using(var stream = new FileStream (caminho, FileMode.Create)){
                    //Copiou o arquivo para o meu diretorio
                    arquivo.CopyTo(stream);
                }
                novoLivro.Imagem = arquivo.FileName;

                
            }else{
                novoLivro.Imagem = "Padrao.png";
            }

            //img
            context.Livro.Add(novoLivro);

            context.SaveChanges();

            //SEGUNDA PARTE: Adicionar dentro de LivroCategoria a categoria que pertence ao novo livro

            List<LivroCategoria> ListaLivroCategoria = new List<LivroCategoria>(); //Lista as categorias
            
            //Array que possui as categorias selecionadas pelo usuario
            string [] categoriaSelecionadas = form["Categoria"].ToString().Split(',');
            //Acao, terror, suspense

            foreach (string categoria in categoriaSelecionadas){
                LivroCategoria livrocategoria = new LivroCategoria();
                livrocategoria.CategoriaID = int.Parse(categoria);
                livrocategoria.LivroID = novoLivro.LivroID;
                livrocategoria.LivroID = novoLivro.LivroID;

                ListaLivroCategoria.Add(livrocategoria);

                
            }

    // Peguei a colecao da ListaLivroCategoria e coloquei na tabela LivroCategoria
            context.LivroCategoria.AddRange(ListaLivroCategoria);

            context.SaveChanges();

            return LocalRedirect("/Cadastro");


        }

        // [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        // public IActionResult Error()
        // {
        //     return View("Error!");
        // }
    }
}