using Booking.BaseRepo;
using Booking.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Net.payOS;
using Net.payOS.Types;
using System.Transactions;

namespace Booking.APIController
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<AppUser> _userMane;
        private readonly PayOS _payos;
        private readonly ManagerTransastions _transactionManager;

        public UserController(UserManager<AppUser> userMane, PayOS payos, ManagerTransastions transactionManager)
        {
            _userMane = userMane;
            _payos = payos;
            _transactionManager = transactionManager;
        }

        [HttpPost("webhook-url")]
        public async Task<IActionResult> ReceivePaymentAsync([FromBody] WebhookType webhook)
        {

            try
            {
                WebhookData data = _payos.verifyPaymentWebhookData(webhook);
                if (data != null && webhook.success)
                {
                    var getInvoice = new List<string>();
                    
                    if (getInvoice != null)
                    {
                        await _transactionManager.ExecuteInTransactionAsync(async () =>
                        {
                           
                        });
                     /*   await this._dongtien.SaveChangesAsync();
                        await this._invoice.SaveChangesAsync();*/

                    }
                    return Ok(new { success = true });
                }
                else
                {
                    return BadRequest(new { success = false, message = "Thanh toán không thành công" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi hệ thống" });
            }
        }
    }
}
