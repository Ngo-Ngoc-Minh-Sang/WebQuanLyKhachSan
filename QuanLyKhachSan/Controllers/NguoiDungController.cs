using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLyKhachSan.Models;
using System.Text.RegularExpressions;

namespace QuanLyKhachSan.Controllers
{
    public class NguoiDungController : Controller
    {
        dbQLKSDataContext db = new dbQLKSDataContext();
        // GET: NguoiDung
        [HttpGet]
        public ActionResult DangKi()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DangKi(KHACHHANG kh ,FormCollection f)
        {
            var hoTen = f["HotenKH"];
            var tenDN = f["TenDN"];
            var matKhau = f["Matkhau"];
            var reMatKhau = f["ReMatkhau"];
            var dienThoai = f["Dienthoai"];
            var ngaySinh = String.Format("{0:MM/DD/YYYY}", f["Ngaysinh"]);
            var email = f["Email"];
            var diaChi = f["DiaChi"];
            var gioiTinh = f["GioiTinh"];
            var cccd = f["CCCD"];
            // Kiểm tra họ tên
            if (String.IsNullOrEmpty(hoTen))
                ViewData["Loi1"] = "Họ tên không được bỏ trống";
            // Kiểm tra tên đăng nhập
            if (String.IsNullOrEmpty(tenDN))
                ViewData["Loi2"] = "Tên đăng nhập không được bỏ trống";
            else if (!kiemTraSpace(tenDN))
                ViewData["Loi2"] = "Tên đăng nhập không được chứa dấu cách";
            // Kiểm tra mật khẩu
            if (String.IsNullOrEmpty(matKhau))
                ViewData["Loi3"] = "Mật khẩu không được bỏ trống";
            // Kiểm tra nhập lại mật khẩu
            if (String.IsNullOrEmpty(reMatKhau))
                ViewData["Loi4"] = "Nhập lại mật khẩu không được bỏ trống";
            else if (matKhau != reMatKhau)
                ViewData["Loi4"] = "Không giống với mật khẩu vừa nhập";
            // Kiểm tra số điện thoại
            if (String.IsNullOrEmpty(dienThoai))
                ViewData["Loi5"] = "Số điện thoại không được bỏ trống";
            else if (dienThoai.Length > 10)
                ViewData["Loi5"] = "Không được quá 10 số";
            // Kiểm tra ngày sinh
            if (String.IsNullOrEmpty(ngaySinh))
                ViewData["Loi6"] = "Ngày sinh không được bỏ trống";
            // Kiểm tra email
            if (String.IsNullOrEmpty(email))
                ViewData["Loi7"] = "Email không được bỏ trống";
            else if (!IsEmail(email))
                ViewData["Loi7"] = "Định dạng email không hợp lệ";
            // Kiểm tra địa chỉ
            if (String.IsNullOrEmpty(diaChi))
                ViewData["Loi8"] = "Địa chỉ không được bỏ trống";
            // Kiểm tra căn cước
            if (String.IsNullOrEmpty(cccd))
                ViewData["Loi10"] = "CCCD không được bỏ trống";
            else if (!kiemTraSpace(cccd))
                ViewData["Loi10"] = "CCCD không được có dấu cách";
            if (!String.IsNullOrEmpty(hoTen) && !String.IsNullOrEmpty(tenDN) && !String.IsNullOrEmpty(matKhau)
                && !String.IsNullOrEmpty(reMatKhau) && !String.IsNullOrEmpty(dienThoai) && !String.IsNullOrEmpty(ngaySinh)
                && !String.IsNullOrEmpty(email) && !String.IsNullOrEmpty(diaChi) && !String.IsNullOrEmpty(gioiTinh) && !String.IsNullOrEmpty(cccd))
            {
                kh.HOTEN = hoTen.Trim();
                kh.TAIKHOAN = tenDN;
                kh.MATKHAU = matKhau;
                kh.SDT = dienThoai.Trim();
                kh.NGAYSINH = DateTime.Parse(ngaySinh);
                kh.EMAIL = email;
                kh.DIACHI = diaChi.Trim();
                kh.GIOITINH = gioiTinh;
                kh.CCCD = cccd;
                db.KHACHHANGs.InsertOnSubmit(kh);
                db.SubmitChanges();
                return RedirectToAction("DangNhap", "NguoiDung");
            }
            return View();
        }
        [HttpGet]
        public ActionResult DangNhap()
        {
            Session["taiKhoan"] = null;
            return View();
        }
        [HttpPost]
        public ActionResult DangNhap(FormCollection f)
        {
            var tenDN = f["TenDN"];
            var matKhau = f["MatKhau"];
            if (String.IsNullOrEmpty(tenDN))
                ViewData["Loi1"] = "Tên đăng nhập không được bỏ trống";
            if (String.IsNullOrEmpty(matKhau))
                ViewData["Loi2"] = "Vui lòng nhập mật khẩu";
            if (!String.IsNullOrEmpty(tenDN) && !String.IsNullOrEmpty(matKhau))
            {
                TK_ADMIN ad = db.TK_ADMINs.SingleOrDefault(c => c.TAIKHOAN == tenDN && c.MATKHAU == matKhau);
                KHACHHANG kh = db.KHACHHANGs.SingleOrDefault(c => c.TAIKHOAN == tenDN && c.MATKHAU == matKhau);
                if (kh != null)
                {
                    Session["Ten"] = layTen(kh.HOTEN);
                    Session["taiKhoan"] = kh;
                    Session["MaKH"] = kh.MAKH;
                    return RedirectToAction("Index", "Home");
                }
                else if (ad != null)
                {
                    var kq = (from item in db.TK_ADMINs
                              join item2 in db.NHANVIENs
                              on item.MANV equals item2.MANV
                              where item2.MANV == ad.MANV
                              select item2.TENNV).ToList();
                    string name = kq[0];
                    Session["Ten"] = layTen(name);
                    Session["taiKhoanAD"] = ad;
                    Session["MaAdmin"] = ad.MANV;
                    return RedirectToAction("Admin", "Home");
                }
                else
                    ViewBag.TB = "Sai tên đăng nhập hoặc mật khẩu, vui lòng nhập lại!";
            }
            return View();
        }
        public string layTen(string name)
        {
            string ten = "";
            Stack<char> stack = new Stack<char>();
            for (int i = name.Length - 1; i > 0; i--)
            {
                if (name[i] == ' ')
                    break;
                stack.Push(name[i]);
            }
            foreach(char c in stack)
                ten += c;
            return ten;
        }
        public bool kiemTraSpace(string str)
        {
            for(int i = 0; i < str.Length; i++)
            {
                if (str[i] == ' ')
                    return false;
            }
            return true;
        }
        public static bool IsEmail(string email)
        {
            return Regex.IsMatch(email, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
        }
        
        public ActionResult DangXuat()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }

    }
}