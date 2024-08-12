using Microsoft.AspNetCore.Mvc;

namespace ProductService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{ 
    [HttpGet]
    [Route("{id}")]
    public IActionResult GetById(int id)
    {
        return Ok(new { id });
    }
    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(GetAllProducts());
    }

    private string GetAllProducts()
    {
        return "[{\r\n  \"id\": 1,\r\n  \"name\": \"Potatoes - Pei 10 Oz\"\r\n}, {\r\n  \"id\": 2,\r\n  \"name\": \"Peach - Fresh\"\r\n}, {\r\n  \"id\": 3,\r\n  \"name\": \"Apple - Custard\"\r\n}, {\r\n  \"id\": 4,\r\n  \"name\": \"Muffin - Mix - Bran And Maple 15l\"\r\n}, {\r\n  \"id\": 5,\r\n  \"name\": \"Sugar - Fine\"\r\n}, {\r\n  \"id\": 6,\r\n  \"name\": \"Bar Special K\"\r\n}, {\r\n  \"id\": 7,\r\n  \"name\": \"Halibut - Fletches\"\r\n}, {\r\n  \"id\": 8,\r\n  \"name\": \"Tarragon - Primerba, Paste\"\r\n}, {\r\n  \"id\": 9,\r\n  \"name\": \"Nut - Walnut, Pieces\"\r\n}, {\r\n  \"id\": 10,\r\n  \"name\": \"Tahini Paste\"\r\n}]";
    }
}
