using QuizWebApp.Models;
using QuizWebApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace QuizWebApp.Controllers
{
    public class UserAccountController : Controller
    {
        private QuizDBEntities objQuizDBEntities;
        public UserAccountController()
        {
            objQuizDBEntities = new QuizDBEntities();
        }
        // GET: UserAccount
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(RegisterUserViewModel user)
        {
            if (ModelState.IsValid)
            {
                var existingUsers = objQuizDBEntities.Users.FirstOrDefault(model => model.UserName == user.UserName);

                if(existingUsers != null)
                {
                    ModelState.AddModelError(string.Empty, "User Name already exists");
                }
                else
                {
                    User objUser = new User();
                    objUser.UserName = user.UserName;
                    objUser.UserPassword = user.UserPassword;

                    objQuizDBEntities.Users.Add(objUser);
                    objQuizDBEntities.SaveChanges();
                    return RedirectToAction("Login");
                }
            }
            return View();
        }

        public ActionResult Login(string ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }
        [HttpPost]
        public ActionResult Login(UserViewModel user, string ReturnUrl)
        {
            if (ModelState.IsValid)
            {
                var objUser = objQuizDBEntities.Users.FirstOrDefault(model => model.UserName == user.UserName);

                if(objUser == null)
                {
                    ModelState.AddModelError(string.Empty, "UserName not exists!");
                }
            
                else if(objUser.UserPassword != user.UserPassword)
                {
                    ModelState.AddModelError(string.Empty, "UserName and Password did not matched!");
                }
                else
                {
                    FormsAuthentication.SetAuthCookie(user.UserName, false);
                    var authTicket = new FormsAuthenticationTicket(1, user.UserName, DateTime.Now, DateTime.Now.AddMinutes(20), false, "User");
                    string encryptTicket = FormsAuthentication.Encrypt(authTicket);
                    var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptTicket);
                    HttpContext.Response.Cookies.Add(authCookie);
                    Session["UserId"] = objUser.UserId;
                    Session["IsAdmin"] = 0;
                    if (ReturnUrl != null)
                        return Redirect(ReturnUrl);
                    return RedirectToAction("Index", "Quiz");
                }
            }
            return View();
        }

    }
}