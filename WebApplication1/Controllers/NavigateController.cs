using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class NavigateController : Controller
    {
        private NSHNContext db = new NSHNContext();

        // GET: Navigate
        public ActionResult Index()
        {
            return View("~/Views/Index.cshtml");
        }

        public ActionResult About()
        {
            return View("~/Views/Navigate/About.cshtml");
        }

        public ActionResult ThankYou()
        {
            return View();
        }

        public PartialViewResult _BannerImage()
        {
            return PartialView();
        }

        public PartialViewResult _AllDonations(int? id)
        {
            var donations = db.donations.Where(d => d.account_id == id);
            List<donation> don = new List<donation>();
            foreach (var d in donations)
            {
                donation donation = new donation();

                donation.id = d.id;
                donation.amount = d.amount;
                donation.account_id = d.account_id;
                don.Add(donation);
            }
            return PartialView(don);
        }

        public PartialViewResult _ErrorMssg()
        {
            return PartialView();
        }
    }
}