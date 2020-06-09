using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Amazon.AspNetCore.Identity.Cognito;
using Amazon.Extensions.CognitoAuthentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebAdvert.Web.Models.Accounts;


namespace WebAdvert.Web.Controllers
{
    public class Accounts : Controller
    {
        private readonly SignInManager<CognitoUser> _signInManager;
        private readonly UserManager<CognitoUser> _userManager;
        private readonly CognitoUserPool _pool;

        //public IActionResult Index()
        //{
        //    return View();
        //}

        //12
        public Accounts(SignInManager<CognitoUser> SignInManager,
                  UserManager<CognitoUser> userManager, CognitoUserPool pool )
        {
            _signInManager = SignInManager;
            _userManager = userManager;
            _pool = pool;
        }

        // 12
        public async Task<IActionResult> SignUp()
        {
            var model = new SignupModel();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignupModel model)
        {
            var user = _pool.GetUser(model.Email);
            if (ModelState.IsValid)
            {
                
                if(user.Status != null)
                {
                    ModelState.AddModelError("UserExists", "User with this email already exists");
                }
            }
           // user.Attributes.Add(CognitoAttributesConstants.Name, model.Email);
            user.Attributes.Add(CognitoAttribute.Name.AttributeName, model.Email);

            //user.Attributes.Add("name", model.Email);

            try
            {
                var createdUser = await _userManager.CreateAsync(user, model.Password);


                if (createdUser.Succeeded)
                {
                    RedirectToAction("Confirm");
                }
            }
            catch (Exception ex)
            {

                var err = ex.Message;

            }
            
          


            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Confirm(ConfirmModel model)
        {
            return View(model);
        }

        [HttpPost]
        [ActionName("Confirm")]
        public async Task<IActionResult> Confirm_Post(ConfirmModel model)
        {
            if (ModelState.IsValid)
            {
                 var user = await _userManager.FindByEmailAsync(model.Email).ConfigureAwait(false);
                if (user ==  null)
                {
                    ModelState.AddModelError("Notfound", "A user with the given email address was not found");
                    return View(model);
                }

                var result = await _userManager.ConfirmEmailAsync(user, model.Code).ConfigureAwait(false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError(item.Code, item.Description);
                    }
                    return View(model);
                }
            }
            


            return View(model);
        }
    }
}
