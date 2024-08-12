using Microsoft.AspNetCore.Mvc;

namespace InventoryService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
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
            return Ok(GetAllInventory());
        }
        private string GetAllInventory()
        {
            return "[{\r\n  \"id\": 1,\r\n  \"name\": \"Beef - Ground Lean Fresh\",\r\n  \"price\": \"$59.63\"\r\n}, {\r\n  \"id\": 2,\r\n  \"name\": \"Bread Cranberry Foccacia\",\r\n  \"price\": \"$37.38\"\r\n}, {\r\n  \"id\": 3,\r\n  \"name\": \"Tabasco Sauce, 2 Oz\",\r\n  \"price\": \"$90.78\"\r\n}, {\r\n  \"id\": 4,\r\n  \"name\": \"Pork Loin Cutlets\",\r\n  \"price\": \"$95.07\"\r\n}, {\r\n  \"id\": 5,\r\n  \"name\": \"Cheese - Victor Et Berthold\",\r\n  \"price\": \"$80.84\"\r\n}, {\r\n  \"id\": 6,\r\n  \"name\": \"Bread - 10 Grain Parisian\",\r\n  \"price\": \"$45.00\"\r\n}, {\r\n  \"id\": 7,\r\n  \"name\": \"Cleaner - Bleach\",\r\n  \"price\": \"$94.42\"\r\n}, {\r\n  \"id\": 8,\r\n  \"name\": \"Foie Gras\",\r\n  \"price\": \"$11.19\"\r\n}, {\r\n  \"id\": 9,\r\n  \"name\": \"Pie Box - Cello Window 2.5\",\r\n  \"price\": \"$66.84\"\r\n}, {\r\n  \"id\": 10,\r\n  \"name\": \"Piping Jelly - All Colours\",\r\n  \"price\": \"$40.26\"\r\n}]";
        }
}
