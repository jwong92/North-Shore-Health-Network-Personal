using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;

namespace WebApplication1.Controllers
{
    public class AccountController : Controller
    {
        NSHNContext db = new NSHNContext();
        // GET: Register
        public ActionResult Index()
        {
            return View("~/Views/Index.cshtml");
        }
        
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(user user, FormCollection form)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    NSHNContext db = new NSHNContext();

                    //ENCRYPTION GOES HERE

                    //DETERMINE IF ADMIN TO SHOW THE OPTION OF CHOOSING ADMIN/USER STATUS
                    if (Session["role"] != null)
                    {
                        if (Session["role"].ToString() == "ADM")
                        {
                            user.user_role = form["user-role"];
                        }
                    }
                    else
                    {
                        user.user_role = "USR";
                    }

                    //DO NOT ALLOW A USERNAME TO BE REGISTERED TWICE
                    var users = db.users.Where(u => u.username == user.username);
                    foreach(var u in users)
                    {
                        if(u.username == user.username)
                        {
                            ViewBag.RegisterStatus = "This username has already been taken. Please choose another one.";
                            return View("~/Views/Account/Register.cshtml");
                        }
                    }

                    db.users.Add(user);
                    db.SaveChanges();

                    ModelState.Clear();
                    ViewBag.RegisterStatus = "user " + user.username + " Successfully registered!";
                    return View("~/Views/Account/Register.cshtml");
                }
            }
            catch (DbUpdateException e)
            {
                ViewBag.DbExceptionMessage = e.Message;
            }
            catch (SqlException e)
            {
                ViewBag.SqlExceptionMessage = e.Message;
            }
            catch (Exception e)
            {
                ViewBag.GenericException = e.Message;
            }
            return View("~/Views/Navigate/Errors.cshtml");
        }

        //LOGIN
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(user user)
        {
            try
            {
                NSHNContext db = new NSHNContext();

                //ENCRYPT PASSWORD HERE

                //CREATE A VARIABLE TO HOLD THE FIRST USER WHERE THE DATABASE USERNAME/PASSWORD (ACCESSED WITH LAMBDA EXPRESSION) MATCHES THE POSTED USERNAME/PASSWORD
                var currUser = db.users.Where(u => u.username == user.username && u.password == user.password).FirstOrDefault();

                //IF THE USER IS VALID, ASSIGN SESSION ID AND USERNAME, THEN REDIRECT TO THE LOGIN ACTION
                if (currUser != null)
                {
                    Session["userId"] = currUser.id.ToString();
                    Session["userName"] = currUser.username.ToString();
                    Session["role"] = currUser.role.role_code.ToString();
                    return RedirectToAction("LoggedIn");
                }
                else
                {
                    ModelState.AddModelError("", "Username or password is incorrect");
                }

                return View();
            }
            catch (Exception e)
            {
                ViewBag.GenericException = e.Message;
            }
            return View("~/Views/Navigate/Errors.cshtml");
        }

        public ActionResult LoggedIn()
        {
            try
            {
                if (Session["userId"] != null)
                {
                    if (Session["role"].ToString() == "ADM")
                    {
                        return RedirectToAction("Index", "Donations");

                    }
                    return View("~/Views/Index.cshtml");
                }
                else
                {
                    return RedirectToAction("Login");
                }
            }
            catch (Exception e)
            {
                ViewBag.GenericException = e.Message;
            }
            return View("~/Views/Navigate/Errors.cshtml");
        }

        public ActionResult LoggedOut()
        {
            try
            {
                if (Session["userId"] != null)
                {
                    Session.Abandon();
                }
                return RedirectToAction("Login");
            }
            catch (Exception e)
            {
                ViewBag.GenericException = e.Message;
            }
            return View("~/Views/Navigate/Errors.cshtml");
        }
    }
}