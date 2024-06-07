using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;
using Web2API.Models;
using Microsoft.EntityFrameworkCore;

namespace Web2API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        [HttpPost("Contact")]
        public async Task<IActionResult> Contact([FromBody] ContactModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var mailMessage = new MailMessage
                {
                    From = new MailAddress("vibana.org@gmail.com"),
                    Subject = "New Contact Message",
                    Body = $"Name: {model.Name}<br/>Email: {model.Email}<br/>Message: {model.Message}",
                    IsBodyHtml = true
                };
                mailMessage.To.Add("vibana.org@gmail.com");

                using (var smtpClient = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtpClient.EnableSsl = true;
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = new NetworkCredential("vibana.org@gmail.com", "tdhw taru muiq rxzb");
                    await smtpClient.SendMailAsync(mailMessage);
                }

                return Ok(new { Message = "Email sent successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to send email: {ex.Message}");
            }
        }


    }
}
