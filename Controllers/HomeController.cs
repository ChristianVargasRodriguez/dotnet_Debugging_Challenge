using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DebuggingChallenge.Models;

namespace DebuggingChallenge.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    // Hint on a few errors: pay close attention to how things are named

    [HttpGet("")]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost("user/create")]
    public IActionResult CreateUser(User newUser){
        // Faltaba agregar los errores de validacion por si no se proporciona el Nombre:
        if (String.IsNullOrEmpty(newUser.Name)){
            ModelState.AddModelError("Name", "Name is required!.");
        }

        if(ModelState.IsValid){
            HttpContext.Session.SetString("Username", newUser.Name);
            if(newUser.Location != null){
                HttpContext.Session.SetString("Location", $"from {newUser.Location}"); // Se agregó "from" a   $"{newUser.Location}"  para mostrarlo en el html 
            } else {
                HttpContext.Session.SetString("Location", ""); // se eliminó "Undisclosed", si solo se entrega el Nombre, no se mostrará nada más.
            }
            return RedirectToAction("Generator");
        } else {
            return View("Index", newUser); // Faltaba agregar el modelo "newUser" paramantener el valor ingresado.
        }
    }

    [HttpGet("generator")]
    public IActionResult Generator(){
        if(HttpContext.Session.GetString("Username") == null) // Modificado de "Name" a "Username"
        {
            return RedirectToAction("Index");
        }
        if(HttpContext.Session.GetString("Passcode") == null)
        {
            GeneratePasscode();
        }

        // Faltaba agregar:
        ViewData["Username"] = HttpContext.Session.GetString("Username"); // Para mostrar el nombre en la vista
        ViewData["Location"] = HttpContext.Session.GetString("Location"); // Para mostrar la ubicación en la vista

        return View();
    }


    // Faltaba agregar la accion de Logout
    [HttpPost("logout")]
    public IActionResult Logout(){
        HttpContext.Session.Clear();
        return RedirectToAction("Index");
    }



    [HttpPost("reset")]
    public IActionResult Reset()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index");
    }

    // Hint: Something isn't right here...
    [HttpPost("generate/new")]
    public IActionResult GenerateNew(){
        GeneratePasscode(); // Faltaba para generar un nuevo codigo de acceso ****Está en linea numero **********************
        return RedirectToAction("Generator");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public void GeneratePasscode()
    {
        string passcode = "";
        string CharOptions = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        string NumOptions = "0123456789";
        Random rand = new Random();
        for(int i = 1; i < 15; i++)
        {
            int odds = rand.Next(2);
            if(odds == 0)
            {
                passcode += CharOptions[rand.Next(CharOptions.Length)];
            } else {
                passcode += NumOptions[rand.Next(NumOptions.Length)];
            }
        }
        HttpContext.Session.SetString("Passcode", passcode);
    }
}
