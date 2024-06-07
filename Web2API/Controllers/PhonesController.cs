using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web2API.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web2API.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using System.Text.Json;

[Route("api/[controller]")]
[ApiController]
[Authorize]
[EnableCors("AllowSpecificOrigin")]
public class PhonesController : ControllerBase
{
    private readonly WebblogDbContext _context;
    private readonly ILogger<PhonesController> _logger;
    public PhonesController(WebblogDbContext context, ILogger<PhonesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: api/Phones
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Phone>>> GetPhones()
    {
        _logger.LogInformation("GetAll phone Action method was invoked");
        _logger.LogWarning("This is a warning log");
        _logger.LogError("This is a error log");
        _logger.LogInformation($"Finished getphone request with data { JsonSerializer.Serialize(_context.Phones)} ");
        return await _context.Phones.ToListAsync();
    }

    // GET: api/Phones/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Phone>> GetPhone(int id)
    {
        var phone = await _context.Phones.FindAsync(id);

        if (phone == null)
        {
            return NotFound();
        }

        return phone;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Phone>> PostPhone([FromForm] Phone phone, IFormFile ImageURL)
    {
        if (ImageURL == null)
        {
            return BadRequest("Image file is required.");
        }

        // Mã hóa ảnh thành chuỗi Base64
        using (var memoryStream = new MemoryStream())
        {
            await ImageURL.CopyToAsync(memoryStream);
            byte[] imageBytes = memoryStream.ToArray();
            phone.ImageBase64 = Convert.ToBase64String(imageBytes);
        }

        // Lưu đối tượng Phone vào cơ sở dữ liệu
        _context.Phones.Add(phone);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error: " + ex.Message);
        }

        // Trả về đối tượng Phone đã được tạo thành công
        return CreatedAtAction(nameof(GetPhone), new { id = phone.Id }, phone);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> PutPhone(int id, [FromForm] Phone phone, [FromForm] IFormFile ImageURL)
    {
        if (id != phone.Id)
        {
            return BadRequest();
        }
        if (ImageURL == null)
        {
            return BadRequest("Image file is required.");
        }

        // Mã hóa ảnh thành chuỗi Base64
        using (var memoryStream = new MemoryStream())
        {
            await ImageURL.CopyToAsync(memoryStream);
            byte[] imageBytes = memoryStream.ToArray();
            phone.ImageBase64 = Convert.ToBase64String(imageBytes);
        }


        _context.Entry(phone).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!PhoneExists(id))
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
    // DELETE: api/Phones/5
    
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeletePhone(int id)
    {
        var phone = await _context.Phones.FindAsync(id);
        if (phone == null)
        {
            return NotFound();
        }

        _context.Phones.Remove(phone);
        await _context.SaveChangesAsync();

        return NoContent();
    }
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<Phone>>> SearchPhones(string query)
    {
        if (string.IsNullOrEmpty(query))
        {
            return BadRequest("Query cannot be empty");
        }

        var phones = await _context.Phones
            .Where(p => p.Name.Contains(query) || p.Manufacturer.Contains(query))
            .ToListAsync();

        return Ok(phones);
    }
    [HttpGet("sorted")]
    public async Task<ActionResult<IEnumerable<Phone>>> GetSortedPhones(string sortOrder, int pageNumber = 1, int pageSize = 6)
    {
        var phones = from p in _context.Phones
                     select p;

        switch (sortOrder)
        {
            case "price_asc":
                phones = phones.OrderBy(p => p.Price);
                break;
            case "price_desc":
                phones = phones.OrderByDescending(p => p.Price);
                break;
            default:
                return BadRequest("Thứ tự sắp xếp không hợp lệ");
        }

        var pagedPhones = await phones.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        return Ok(pagedPhones);
    }

    private bool PhoneExists(int id)
    {
        return _context.Phones.Any(e => e.Id == id);
    }

}
