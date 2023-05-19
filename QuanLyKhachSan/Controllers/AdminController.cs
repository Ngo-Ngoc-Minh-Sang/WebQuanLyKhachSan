using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLyKhachSan.Models;

namespace QuanLyKhachSan.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        dbQLKSDataContext db = new dbQLKSDataContext();
        public ActionResult QLKH()
        {
            return View();
        }
        public ActionResult DSKH()
        {
            return View(db.KHACHHANGs.ToList());
        }
        public ActionResult DSNV()
        {
            return View(db.NHANVIENs.ToList());
        }
    }
}