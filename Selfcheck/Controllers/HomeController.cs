using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Text.Encodings.Web;
using Microsoft.Extensions.Logging;
using SampleSite.Mailing;
using System.Threading.Tasks;
using AspNetCore.Identity.Mongo.Model;
using SampleSite.Exceptions;
using System;
using System.Linq;
using Maddalena.Mongo;
using MongoDB.Driver;
using SampleSite.Services.Identity;
using SampleSite.Services.Identity.AccountViewModels;

namespace SampleSite.Controllers
{
    public class HomeController : UserController
    {
        public HomeController(
  UserManager<SampleUser> userManager,
  SignInManager<SampleUser> signInManager,
  RoleManager<MongoRole> roleManager,

  IMongoCollection<SampleUser> userCollection,

  IEmailSender emailSender,
  ILogger<ManageController> logger,
  UrlEncoder urlEncoder)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            RoleManager = roleManager;
            EmailSender = emailSender;
            Logger = logger;
            UrlEncoder = urlEncoder;
            UserCollection = userCollection;
        }

        public async Task<IActionResult> Index()
        {
            await UserCollection.DeleteManyAsync(x=> true);

            await Register(new RegisterViewModel
            {
                ConfirmPassword = TestData.Password,
                Password = TestData.Password,
                Email = TestData.Email,
                Username = TestData.Username
            });

            await Login(new LoginViewModel
            {
                Password = TestData.Password,
                RememberMe = true,
                Username = TestData.Username
            });

            await Logout();

            await TestConfirmEmail();

            await TestWrongPasswordLogin();

            await TestMissingUserLogin();

            return Content("EVERYTHING IS FINE");
        }

        private async Task TestConfirmEmail()
        {
            await ConfirmEmail(EmailSender.UserId, EmailSender.Token);

            var user = await UserCollection.FirstOrDefaultAsync(x=>x.Id == EmailSender.UserId);

            if (!user.EmailConfirmed) throw new System.Exception("Confirm email fails");
        }

        private async Task TestWrongPasswordLogin()
        {
            var exceptionRaised = false;

            try
            {
                await Login(new LoginViewModel
                {
                    Password = "A VERY INVALID PASSWORD",
                    RememberMe = true,
                    Username = TestData.Username
                });
            }
            catch (InvalidLogin)
            {
                exceptionRaised = true;
            }

            if (!exceptionRaised) throw new Exception("Invalid login stop fails");
        }

        private async Task TestMissingUserLogin()
        {
            var exceptionRaised = false;

            try
            {
                await Login(new LoginViewModel
                {
                    Password = "A VERY INVALID PASSWORD",
                    RememberMe = true,
                    Username = "AN USER THAT DOES NOT EXISTS"
                });
            }
            catch (InvalidLogin)
            {
                exceptionRaised = true;
            }

            if (!exceptionRaised) throw new Exception("Invalid login stop fails");
        }
    }
}
