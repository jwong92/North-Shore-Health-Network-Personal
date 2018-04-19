using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;

//https://msdn.microsoft.com/en-us/library/jj713564(v=vs.113).aspx

namespace WebApplication1.Controllers
{
    public class DonationsController : Controller
    {
        private NSHNContext db = new NSHNContext();

        // GET: donations
        public ActionResult Index()
        {
            try
            {
                var donations = db.donations.Include(d => d.north_shore_accounts);
                return View(donations.ToList());
            }
            catch (Exception e)
            {
                ViewBag.GenericException = e.Message;
            }

            return View("~/Navigate/Errors");
        }

        [HttpGet]
        public ActionResult AcctPayInfo()
        {
            try
            {
                List<north_shore_accounts> nsa = db.north_shore_accounts.ToList();
                return View(nsa);
            }
            catch (Exception e)
            {
                ViewBag.GenericException = e.Message;
            }

            return View("~/Navigate/Errors");
        }

        // GET: donations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {
                donation donation = db.donations.Find(id);
                if (donation == null)
                {
                    return HttpNotFound();
                }
                return View(donation);
            }
            catch (Exception e)
            {
                ViewBag.GenericException = e.Message;
            }

            return View("~/Navigate/Errors");
        }

        [HttpGet]
        public ActionResult AccountInfoDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {
                north_shore_accounts nsa = db.north_shore_accounts.Find(id);
                if (nsa == null)
                {
                    return HttpNotFound();
                }
                return View(nsa);
            }
            catch (Exception e)
            {
                ViewBag.GenericException = e.Message;
            }
            return View("~/Navigate/Errors");
        }

        // GET: donations/Create
        public ActionResult Create()
        {
            try
            {
                if (Session["role"] != null)
                {
                    if (Session["role"].ToString() == "USR")
                    {

                        //GET THE USER ID, FROM THERE CROSS CHECK WITH USERS_ID FROM NSA AND FIND THE ID. IF IT EXISTS, THEN REDIRECT
                        int count = 0;
                        int user_id = Convert.ToInt16(Session["userId"]);
                        var account = db.north_shore_accounts.Where(a => a.user_id == user_id);
                        foreach(var item in account)
                        {
                            count += 1;
                        }
                        if(count > 0)
                        {
                            return RedirectToAction("UserCreateView");
                        }
                        //IF THERE IS NO DONATION ACCOUNT, BUT THEY ARE A USER, THEN REDIRECT TO DONATION PAGE WITHOUT REGISTRATION
                        else
                        {
                            return RedirectToAction("UserWithoutAccount");
                        }
                    }
                }
                //IF THEY ARE AN ADMIN OR ANONYMOUS,MAKE THEM CREATE AN ACCOUNT
                donation donation = new donation();
                ViewBag.province_char = new SelectList(db.provinces, "code", "name");
                return View();
            }
            catch(Exception e)
            {
                ViewBag.GenericException = e.Message;
            }
            return View("~/Navigate/Errors");
        }

        // POST: donations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(donation donation, FormCollection form)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //CREATE A NEW USER

                    //DETERMINE IF THE USERNAME EXHISTS
                    var user_exhists = db.users.Where(u => u.username == donation.north_shore_accounts.user.username);
                    int count = 0;
                    foreach(var u in user_exhists)
                    {
                        count += 1;
                    }

                    if(count > 0)
                    {
                        ViewBag.province_char = new SelectList(db.provinces, "code", "name");
                        ViewBag.ErrorMssg = "This username has already been taken. Please choose another one.";
                        return View("~/Views/donations/Create.cshtml");
                    }

                    donation.north_shore_accounts.user.user_role = "USR";
                    db.users.Add(donation.north_shore_accounts.user);
                    db.SaveChanges();

                    db.payment_information.Add(donation.north_shore_accounts.payment_information);
                    db.SaveChanges();

                    //GET THE ID OF INSERTED PAYMENT INFORMATION AND SELECTED DROPDOWN THEN ADD NSA
                    donation.north_shore_accounts.payment_info = donation.north_shore_accounts.payment_information.id;
                    donation.north_shore_accounts.user_id = donation.north_shore_accounts.user.id;
                    donation.north_shore_accounts.province_char = form["province_char"];
                    db.north_shore_accounts.Add(donation.north_shore_accounts);
                    db.SaveChanges();

                    //GET THE ID OF THE INSERTED NSA ID AND ADD TO DONATIONS
                    donation.account_id = donation.north_shore_accounts.id;
                    db.donations.Add(donation);
                    db.SaveChanges();

                    if (Session["role"] != null)
                    {
                        if (Session["role"].ToString() == "ADM")
                        {
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            Session["donated"] = "true";
                            return RedirectToAction("Index", "Navigate");
                        }
                    }

                    else
                    {
                        Session["donated"] = "true";
                        return RedirectToAction("Index", "Navigate");
                    }

                }
                //FOR MODEL STATE ERRORS - DEVELOPER ONLY
                var errors = ModelState.Select(x => x.Value.Errors)
                           .Where(y => y.Count > 0)
                           .ToList();

                ViewBag.province_char = new SelectList(db.provinces, "code", "name");
                return View(donation);
            }
            catch(DbUpdateException e)
            {
                ViewBag.DbExceptionMessage = e.Message;
            }
            catch(SqlException e)
            {
                ViewBag.SqlExceptionMessage = e.Message;
            }
            catch(Exception e)
            {
                ViewBag.GenericException = e.Message;
            }

            return View("~/Navigate/Errors");
        }

        [HttpGet]
        public ActionResult UserCreateView()
        {
            return View();
        }

        [HttpPost]
        //GIVE DONATION IF THE USER IS LOGGED IN
        public ActionResult UserDonate(FormCollection form)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //FINDS AND HOLDS THE USER ACCOUNT
                    north_shore_accounts nsa = new north_shore_accounts();
                    nsa.user_id = Convert.ToInt16(Session["userId"]);
                    var user = db.north_shore_accounts.Where(n => n.user_id == nsa.user_id);

                    foreach (var row in user)
                    {
                        nsa.id = row.id;
                    }

                    nsa = db.north_shore_accounts.Find(nsa.id);

                    //FIND THE PAYMENT INFORMATION FOR THIS USER
                    payment_information pi = new payment_information();
                    pi.credit_card = form["north_shore_accounts.payment_information.credit_card"];
                    pi.ccv = form["north_shore_accounts.payment_information.ccv"];
                    DateTime date = DateTime.Parse(form["north_shore_accounts.payment_information.exp_date"]);
                    int year = date.Year;
                    int day = date.Day;
                    int month = date.Month;
                    string exp_date = year.ToString() + "-" + month.ToString() + "-" + day.ToString();

                    DateTime convertedDate = Convert.ToDateTime(exp_date);

                    pi.exp_date = date;
                    pi.id = nsa.payment_info;

                    ViewBag.UpRowsAffected = db.Database.ExecuteSqlCommand("UPDATE payment_information SET credit_card = @credit_card, ccv = @ccv, exp_date = @exp_date WHERE id = @id", new SqlParameter("@credit_card", pi.credit_card), new SqlParameter("@ccv", pi.ccv), new SqlParameter("@exp_date", pi.exp_date), new SqlParameter("@id", pi.id));

                    //SET THE DONATIONS ID TO USER ACCOUNT ID, THEN ADD THE AMOUNT TO IT
                    donation donation = new donation();
                    donation.account_id = nsa.id;
                    donation.amount = Convert.ToDecimal(form["amount"]);

                    ViewBag.InRowsAffected = db.Database.ExecuteSqlCommand("INSERT INTO donations VALUES (@amount, @account_id)", new SqlParameter("@amount", donation.amount), new SqlParameter("@account_id", donation.account_id));

                    Session["donated"] = "true";
                    return RedirectToAction("Index", "Navigate");
                }
            }
            catch(DbUpdateException e)
            {
                ViewBag.DbExceptionMessage = e.Message;
            }
            catch(SqlException e)
            {
                ViewBag.SqlExceptionMessage = e.Message;
            }
            catch (Exception e)
            {
                ViewBag.GenericException = e.Message;
            }
            return View("~/Views/Navigate/Errors.cshtml");
        }

        [HttpGet]
        public ActionResult UserWithoutAccount()
        {
            ViewBag.province_char = new SelectList(db.provinces, "code", "name");
            return View();
        }

        [HttpPost]
        public ActionResult CreateAccountForUser(donation donation, FormCollection form)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    //ADD A PAYMENT INFORMATION
                    db.payment_information.Add(donation.north_shore_accounts.payment_information);
                    db.SaveChanges();

                    //GET THE ID OF INSERTED PAYMENT INFORMATION AND SELECTED DROPDOWN THEN ADD NSA
                    donation.north_shore_accounts.payment_info = donation.north_shore_accounts.payment_information.id;
                    donation.north_shore_accounts.user_id = Convert.ToInt16(Session["userId"]);
                    donation.north_shore_accounts.province_char = form["province_char"];
                    db.north_shore_accounts.Add(donation.north_shore_accounts);
                    db.SaveChanges();

                    //GET THE ID OF THE INSERTED NSA ID AND ADD TO DONATIONS
                    donation.account_id = donation.north_shore_accounts.id;
                    db.donations.Add(donation);
                    db.SaveChanges();

                    if (Session["role"] != null)
                    {
                        if (Session["role"].ToString() == "ADM")
                        {
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            Session["donated"] = "true";
                            return RedirectToAction("Index", "Navigate");
                        }
                    }
                    else
                    {
                        Session["donated"] = "true";
                        return RedirectToAction("Index", "Navigate");
                    }
                }
                //FOR MODEL STATE ERRORS - DEVELOPER ONLY
                var errors = ModelState.Select(x => x.Value.Errors)
                           .Where(y => y.Count > 0)
                           .ToList();

                ViewBag.province_char = new SelectList(db.provinces, "code", "name");
                return View("~/Views/Create", donation);
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

    public PartialViewResult Thanks()
        {
            return PartialView("~/Views/Navigate/_ThankYou.cshtml");
        }

        // GET: donations/Edit/5 -- THIS WILL BE FOR DONATIONS EDIT
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {
                donation donation = db.donations.Find(id);
                if (donation == null)
                {
                    return HttpNotFound();
                }

                ViewBag.province_char = new SelectList(db.provinces, "code", "name", donation.north_shore_accounts.province.code);
                return View(donation);
            }
            catch (Exception e)
            {
                ViewBag.GenericException = e.Message;
            }
            return View("~/Views/Navigate/Errors.cshtml");
        }

        // POST: donations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PEdit(donation donation, FormCollection form)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    donation.north_shore_accounts.province_char = form["province_char"];
                    db.Entry(donation).State = EntityState.Modified;
                    db.Entry(donation.north_shore_accounts).State = EntityState.Modified;
                    db.Entry(donation.north_shore_accounts.payment_information).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    var errors = ModelState.Select(x => x.Value.Errors)
                   .Where(y => y.Count > 0)
                   .ToList();

                    ViewBag.province_char = new SelectList(db.provinces, "code", "name", donation.north_shore_accounts.province_char);
                    return View("~/Views/donations/Edit.cshtml");
                }
            }
            catch(DbUpdateException e)
            {
                ViewBag.GenericException = e.Message;
            }
            catch(SqlException e)
            {
                ViewBag.SqlExceptionMessage = e.Message;
            }
            catch (Exception e)
            {
                ViewBag.DbExceptionMessage = e.Message;
            }
            return View("~/Views/Navigate/Errors.cshtml");
        }

        //DELETE JUST THEIR DONATION
        // GET: donations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {
                donation donation = db.donations.Find(id);
                if (donation == null)
                {
                    return HttpNotFound();
                }
                return View(donation);
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

        // POST: donations/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteDonor(int id)
        {
            donation donation = db.donations.Find(id);
            db.donations.Remove(donation);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //DELETE DONATION ACCOUNT AND DONTION
        [HttpGet]
        public ActionResult DelAcc(int? id)
        {
            north_shore_accounts nsa = db.north_shore_accounts.Find(id);
            donation d = new donation();

            //find donation from NSA account_id
            var sel_don = db.donations.Where(don => don.account_id == nsa.id);
            foreach (var don in sel_don)
            {
                d = db.donations.Find(don.id);
            }
            if (d == null)
            {
                return HttpNotFound();
            }
            return View(d);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteAccount(int id)
        {
            try
            {
                north_shore_accounts nsa = new north_shore_accounts();
                donation donation = db.donations.Find(id);

                nsa.id = donation.account_id;
                nsa = db.north_shore_accounts.Find(nsa.id);

                db.donations.Remove(donation);
                db.north_shore_accounts.Remove(nsa);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch(DbUpdateException e)
            {
                ViewBag.DbExceptionMessage = e.Message;
            }
            catch(SqlException e)
            {
                ViewBag.SqlExceptionMessage = e.Message;
            }
            catch(Exception e)
            {
                ViewBag.GenericException = e.Message;
            }
            return View("~/Views/Navigate/Errors.cshtml");
        }

        //DELETE THEIR DONATION AND THEIR PAYMENT INFORMATION AND ACCOUNT(EVERYTHING)
        [HttpGet]
        public ActionResult DelP(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            try
            {
                payment_information pi = db.payment_information.Find(id);
                north_shore_accounts nsa = new north_shore_accounts();
                donation d = new donation();

                //find NSA id from pi payment_info
                var sel_nsa = db.north_shore_accounts.Where(n => n.payment_info == pi.id);
                foreach (var n in sel_nsa)
                {
                    nsa = db.north_shore_accounts.Find(n.id);
                }

                //find donation from NSA account_id
                var sel_don = db.donations.Where(don => don.account_id == nsa.id);
                foreach (var don in sel_don)
                {
                    d = db.donations.Find(don.id);
                }
                if (d == null)
                {
                    return HttpNotFound();
                }
                return View(nsa);
            }
            catch(DbUpdateException e)
            {
                ViewBag.DbExceptionMessage = e.Message;
            }
            catch(SqlException e)
            {
                ViewBag.SqlExceptionMessage = e.Message;
            }
            catch(Exception e)
            {
                ViewBag.GenericException = e.Message;
            }
            return View("~/Views/Navigate/Errors.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteDonorPay(int id)
        {
            try
            {
                north_shore_accounts nsa = new north_shore_accounts();
                payment_information pi = new payment_information();
                donation donation = new donation();

                var donor = db.donations.Where(d => d.account_id == id);
                foreach (var d in donor)
                {
                    donation = db.donations.Find(d.id);
                    if (donation != null)
                    {
                        db.donations.Remove(donation);
                    }
                }

                nsa = db.north_shore_accounts.Find(id);
                pi.id = nsa.payment_info;
                pi = db.payment_information.Find(pi.id);

                db.north_shore_accounts.Remove(nsa);
                db.payment_information.Remove(pi);
                db.SaveChanges();
                return RedirectToAction("AcctPayInfo");
        }
            catch(DbUpdateException e)
            {
                ViewBag.DbExceptionMessage = e.Message;
            }
            catch(SqlException e)
            {
                ViewBag.SqlExceptionMessage = e.Message;
            }
            catch (Exception e)
            {
                ViewBag.GenericException = e.Message;
            }
            return View("~/Views/Navigate/Errors.cshtml");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
