using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLyKhachSan.Models;

namespace QuanLyKhachSan.Controllers
{
    public class DatPhongController : Controller
    {
        dbQLKSDataContext db = new dbQLKSDataContext();
        // GET: DatPhong
        public string themTuDongMaDP()
        {
            var query = (from item in db.CT_DATPHONGs
                         select item).ToList().Count();
            string result = "DPH0" + (query + 1).ToString();
            return result;
        }
        [HttpGet]
        public ActionResult DK_DatPhong(string tt)
        {
            bool checkDP = false;
            if (string.Compare(tt, "Có Người") == 0)
                checkDP = true;
            if (Session["taiKhoan"] == null)
                return RedirectToAction("DangNhap", "NguoiDung");
            if (checkDP == true)
            {
                Session["KTDP"] = tt;
                return RedirectToAction("ShowPhong", "Phong");
            }
            Session["DatPhong"] = null;
            return View();
        }
        [HttpPost]
        public ActionResult DK_DatPhong(DATPHONG dp, CT_DATPHONG ctdp, FormCollection f, string mp)
        {
            var NgayDat = DateTime.Now.ToString();
            var MaPhong = mp;
            var NgayNP = f["NgayNP"];
            var NgayTP = f["NgayTP"];
            var SoLuongNguoiO = f["SoNguoiO"];
            var TienCoc = f["TienCoc"];
            var MaKH = Session["MaKH"];
            if (String.IsNullOrEmpty(NgayNP))
                ViewData["Loi1"] = "Ngày nhận phòng không được bỏ trống";
            if (String.IsNullOrEmpty(NgayTP))
                ViewData["Loi2"] = "Ngày trả phòng không được bỏ trống";
            if (String.IsNullOrEmpty(SoLuongNguoiO))
                ViewData["Loi3"] = "Số lượng người ở không được bỏ trống";
            if (String.IsNullOrEmpty(TienCoc))
                ViewData["Loi4"] = "Tiền cọc không được bỏ trống";
            if (!String.IsNullOrEmpty(NgayNP) && !String.IsNullOrEmpty(NgayTP) && !String.IsNullOrEmpty(SoLuongNguoiO) && !String.IsNullOrEmpty(TienCoc))
            {
                if (!kiemTraDatPhong(DateTime.Now, int.Parse(MaKH.ToString())))
                {
                    dp.MADP = themTuDongMaDP().Trim();
                    dp.NGAYDAT = DateTime.Now;
                    dp.MAKH = int.Parse(MaKH.ToString());
                    db.DATPHONGs.InsertOnSubmit(dp);
                    db.SubmitChanges();
                }
                if (kiemTraDatPhong(DateTime.Now, int.Parse(MaKH.ToString())))
                {
                    var findKey_DP = db.DATPHONGs.FirstOrDefault(t => t.NGAYDAT == DateTime.Now && t.MAKH == int.Parse(MaKH.ToString()));
                    ctdp.MADP = findKey_DP.MADP.Trim();
                    ctdp.MAPHONG = MaPhong.Trim();
                    ctdp.NGAYNP = Convert.ToDateTime(NgayNP);
                    ctdp.NGAYTP = Convert.ToDateTime(NgayTP);
                    ctdp.TIENCOC = int.Parse(TienCoc.ToString());
                    ctdp.SONGUOIO = int.Parse(SoLuongNguoiO.ToString());
                    db.CT_DATPHONGs.InsertOnSubmit(ctdp);
                }
                db.SubmitChanges();
                Session["DatPhong"] = ".";
            }
            return View();
        }
        public ActionResult PhongDaDat()
        {
            var list = (from ctdp in db.CT_DATPHONGs
                        join dp in db.DATPHONGs on ctdp.MADP equals dp.MADP
                        where dp.MAKH == int.Parse(Session["MaKH"].ToString())
                        select ctdp);
            return View(list);
        }
        public ActionResult XoaPhong(string mdp, string mp)
        {
            CT_DATPHONG ctdp = db.CT_DATPHONGs.FirstOrDefault(t => t.MADP == mdp && t.MAPHONG == mp);
            var checkEmpty_CTDP = db.CT_DATPHONGs.Count(t => t.MADP == mdp);
            if(checkEmpty_CTDP == 1)
            {
                DATPHONG dp = db.DATPHONGs.FirstOrDefault(t => t.MADP == mdp);
                db.DATPHONGs.DeleteOnSubmit(dp);
            }
            db.CT_DATPHONGs.DeleteOnSubmit(ctdp);
            db.SubmitChanges();
            return RedirectToAction("PhongDaDat");
        }
        public bool kiemTraDatPhong(DateTime date, int maKH)
        {
            var check = db.DATPHONGs.Count(t => t.NGAYDAT == date && t.MAKH == maKH);
            if (check > 0)
                return true;
            return false;
        }
    }
}