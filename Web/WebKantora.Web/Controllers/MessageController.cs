using AutoMapper;
using MimeKit;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using WebKantora.Data.Models;
using WebKantora.Services.Web.Contracts;
using WebKantora.Services.Data.Contracts;
using WebKantora.Web.Models.ContactViewModels;

namespace WebKantora.Web.Controllers
{
    public class MessageController : Controller
    {
        private IMessagesService messagesService;
        private IUsersService usersService;
        private IEmailSenderService emailSenderService;
        private IMapper mapper;

        public MessageController(IMessagesService messagesService, IUsersService usersService, IEmailSenderService emailSenderService, IMapper mapper)
        {
            this.messagesService = messagesService;
            this.usersService = usersService;
            this.emailSenderService = emailSenderService;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new ContactFormViewModel();

            if (User.Identity.IsAuthenticated)
            {
                var userName = this.HttpContext.User.Identity.Name;
                var currentUser = await this.usersService.GetByUserName(userName);

                model.FirstName = currentUser.FirstName;
                model.LastName = currentUser.LastName;
                model.Email = currentUser.Email;
                model.PhoneNumber = currentUser.PhoneNumber;
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Send(ContactFormViewModel model)
        {
            if (ModelState.IsValid)
            {
                string subject = $"��������� �� {model.FirstName} {model.LastName}";
                string messageText = $"{model.FirstName} {model.LastName}\nemail: {model.Email}\nphone number: {model.PhoneNumber}\nmessage: {model.Content}";

                var mimeMessage = new MimeMessage()
                {
                    Subject = subject,
                    Body = new TextPart("plain")
                    {
                        Text = messageText
                    }
                };

                var result = this.emailSenderService.SendEmailForUserRequest(mimeMessage);

                if (result)
                {
                    TempData.Add("SuccessMessage", "���������� �� �� �����������!");

                    var message = this.mapper.Map<Message>(model);

                    if (User.Identity.IsAuthenticated)
                    {
                        var user = await this.usersService.GetByUserName(User.Identity.Name);
                        message.Author = user;
                    }

                    await this.messagesService.Add(message);
                    return this.RedirectToAction("Index", "Home");
                }
                else
                {
                    return this.BadRequest();
                }
            }
            
            return this.View(model);
        }
    }
}