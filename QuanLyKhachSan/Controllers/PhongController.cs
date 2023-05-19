using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLyKhachSan.Models;

namespace QuanLyKhachSan.Controllers
{
    public class PhongController : Controller
    {
        // GET: Phong
        dbQLKSDataContext db = new dbQLKSDataContext();
        [HttpGet]
        public ActionResult ShowPhong()
        {
            return View(db.PHONGs.ToList());
        }
        [HttpPost]
        public ActionResult ShowPhong(string txt_search)
        {
            if (txt_search != "")
            {
                var query = (from item in db.PHONGs
                             where item.TENPHONG == txt_search
                             select item).ToList();
                return View(query);
            }
            return View();
        }
        [HttpGet]
        public ActionResult ShowPhong2()
        {
            return View(db.PHONGs.ToList());
        }
        public ActionResult ShowPhong2(int cbo_sort)
        {
            if (cbo_sort == 1)
            {
                var query = (from item in db.PHONGs
                             orderby item.LOAIPHONG
                             select item).ToList();
                return View(query);
            }
            else if (cbo_sort == 2)
            {
                var query = (from item in db.PHONGs
                             orderby item.TINHTRANG
                             select item).ToList();
                return View(query);
            }
            else
                return RedirectToAction("ShowPhong", "Phong");
        }
        public ActionResult ChiTietPhong(string mp)
        {
            PHONG phong = db.PHONGs.Single(s => s.MAPHONG == mp);
            if (phong == null)
            {
                return HttpNotFound();
            }
            return View(phong);
        }
    }
}