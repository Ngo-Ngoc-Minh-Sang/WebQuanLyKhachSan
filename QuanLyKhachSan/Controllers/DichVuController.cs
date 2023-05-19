using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLyKhachSan.Models;

namespace QuanLyKhachSan.Controllers
{
    public class DichVuController : Controller
    {
        // GET: DichVu
        dbQLKSDataContext db = new dbQLKSDataContext();
        public ActionResult ShowDichVu()
        {
            return View(db.DICHVUs.ToList());
        }
        public ActionResult DK_DichVu(SUDUNG_DV sddv, CHITIET_SDDV ct_sddv, string mdv)
        {
            if (Session["taikhoan"] == null)
                return RedirectToAction("DangNhap", "NguoiDung");
            DateTime ngayDat = DateTime.Now;
            int maKH = int.Parse(Session["MaKH"].ToString());
            var checkTonTai = db.SUDUNG_DVs.FirstOrDefault(t => t.NGAYDAT == ngayDat && t.MAKH == maKH);
            string ma_sddv;
            if (checkTonTai == null)
            {
                ma_sddv = themTuDongMaSDDV().Trim();
                sddv.MA_SD = ma_sddv;
                sddv.MAKH = int.Parse(Session["MaKH"].ToString());
                sddv.NGAYDAT = ngayDat;
                db.SUDUNG_DVs.InsertOnSubmit(sddv);
                db.SubmitChanges();
            }
            else
                ma_sddv = checkTonTai.MA_SD.Trim();
            var check_DatTrungDV = db.CHITIET_SDDVs.FirstOrDefault(t => t.MADV == mdv && t.MA_SD == ma_sddv);
            if (check_DatTrungDV != null)
            {
                return RedirectToAction("ShowDichVu");
            }
            ct_sddv.MA_SD = ma_sddv;
            ct_sddv.MADV = mdv;
            db.CHITIET_SDDVs.InsertOnSubmit(ct_sddv);
            var timSDDV = db.SUDUNG_DVs.Single(t => t.NGAYDAT == ngayDat && t.MAKH == maKH);
            db.SubmitChanges();
            timSDDV.SOLUONG = demSL_DVu_SDung(ma_sddv);
            timSDDV.TONGTIEN = tinhTongTienDV(ma_sddv);
            db.SubmitChanges();
            return RedirectToAction("ShowDichVu", "DichVu");
        }
        public string themTuDongMaSDDV()
        {
            var query = (from item in db.CHITIET_SDDVs
                         select item).ToList().Count();
            string result = "SDDV0" + (query + 1).ToString();
            return result;
        }
        public int demSL_DVu_SDung(string msd)
        {
            var query = db.CHITIET_SDDVs.Count(t => t.MA_SD == msd);
            return query;
        }
        public float tinhTongTienDV(string msd)
        {
            int sumGiaDV = 0;
            var getDV_Same = db.CHITIET_SDDVs.Where(t => t.MA_SD == msd);
            if(getDV_Same != null)
            {
                sumGiaDV = getDV_Same.Sum(t => t.DICHVU.GIADV);
            }
            return sumGiaDV;
        }
        public ActionResult DichVuDaDat()
        {
            var list = (from ct_sddv in db.CHITIET_SDDVs
                        join sddv in db.SUDUNG_DVs on ct_sddv.MA_SD equals sddv.MA_SD
                        where sddv.MAKH == int.Parse(Session["MaKH"].ToString())
                        select ct_sddv);
            return View(list);
        }
        public ActionResult HuyDichVu(string mdv, string msd)
        {
            var find_DV_Delete = db.CHITIET_SDDVs.FirstOrDefault(t => t.MADV == mdv);
            var check_SL_CTSDDV = db.CHITIET_SDDVs.Count(t => t.MA_SD == msd);
            if(check_SL_CTSDDV == 1)
            {
                SUDUNG_DV sddv =  db.SUDUNG_DVs.FirstOrDefault(t => t.MA_SD == msd);
                db.SUDUNG_DVs.DeleteOnSubmit(sddv);
            }
            db.CHITIET_SDDVs.DeleteOnSubmit(find_DV_Delete);
            db.SubmitChanges();
            return RedirectToAction("DichVuDaDat");
        }
    }
}