using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLyKhachSan.Models;

namespace QuanLyKhachSan.Controllers
{
    public class HoaDonController : Controller
    {
        // GET: HoaDon
        dbQLKSDataContext db = new dbQLKSDataContext();
        [HttpGet]
        public ActionResult LapHoaDon()
        {
            return View();
        }
        [HttpPost]
        public ActionResult LapHoaDon(FormCollection f)
        {
            HOADON hd = new HOADON();
            CTHD cthd = new CTHD();
            var maHD = themTuDongMaHD().Trim();
            var maNV = Session["MaAdmin"].ToString();
            var maDP = f["MaDP"];
            var ngayLap = DateTime.Now;
            var kiemTraTrungHD = db.HOADONs.Count(t => t.MADP == maDP && t.NGAYLAP == DateTime.Now);
            var cacPhongDaDat = db.CT_DATPHONGs.Where(t => t.MADP == maDP).ToList();
            int n = cacPhongDaDat.Count;
            if (String.IsNullOrEmpty(maDP))
                ViewData["Loi1"] = "Vui lòng nhập mã đặt phòng!";
            if (!String.IsNullOrEmpty(maDP))
            {
                if (kiemTraTrungHD == 0)
                {
                    hd.MAHD = maHD;
                    hd.MANV = maNV;
                    hd.MADP = maDP;
                    hd.NGAYLAP = ngayLap;
                    db.HOADONs.InsertOnSubmit(hd);
                    db.SubmitChanges();
                    for (int i = 0; i < n; i++)
                    {
                        cthd.MAHD = maHD;
                        cthd.MAPHONG = cacPhongDaDat[i].MAPHONG;
                        cthd.GIATIEN = int.Parse(cacPhongDaDat[i].PHONG.GIA.ToString());
                        db.CTHDs.InsertOnSubmit(cthd);
                        db.SubmitChanges();
                        cthd = new CTHD();
                    }
                    var timHD = db.HOADONs.Single(t => t.MAHD == maHD);
                    timHD.TONGTIEN = db.CTHDs.Where(t => t.MAHD == maHD).Sum(y => y.GIATIEN);
                    db.SubmitChanges();
                }
                else
                    return View();
            }
            return View();
        }
        public string themTuDongMaHD()
        {
            var query = (from item in db.HOADONs
                         select item).ToList().Count();
            string result = "HD0" + (query + 1).ToString();
            return result;
        }
        public ActionResult QLHD()
        {
            return View();
        }
        public ActionResult ShowHoaDon()
        {
            return View(db.HOADONs.ToList());
        }
        public ActionResult XemCTHD(string mhd)
        {
            var ds_CTHD = db.CTHDs.Where(t => t.MAHD == mhd).ToList();
            return View(ds_CTHD);
        }
    }
}