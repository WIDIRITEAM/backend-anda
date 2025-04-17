using Anda.ServiciosAPI.Data;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/clientes")]
public class ClientesController : ControllerBase
{
    private readonly AppDbContext _context;

    public ClientesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetClientes()
    {
        return Ok(_context.Clientes.ToList());
    }

    [HttpPost]
    public IActionResult CrearCliente([FromBody] Cliente cliente)
    {
        Console.WriteLine("creando cliente: " + cliente);
        _context.Clientes.Add(cliente);
        _context.SaveChanges();
        return Created($"/api/clientes/{cliente.ID}", cliente);
    }
}
