using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TechStore.DAL;
using TechStore.Models;
using TechStore.Repository;

namespace TechStore.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin

        public List<SelectListItem> GetCategory()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            var category = _unitOfWork.GetRepositoryInstance<Category>().GetAllRecords();
            foreach(var item in category)
            {
                list.Add(new SelectListItem { Value = item.CategoryId.ToString(), Text = item.CategoryName });
            }
            return list;
        }


        public GenericUnitOfWork _unitOfWork = new GenericUnitOfWork();
        public ActionResult Dashboard()
        {
            return View();
        }
        public ActionResult Categories()
        {
            List<Category> allcategories = _unitOfWork.GetRepositoryInstance<Category>().GetAllRecordsIQueryable().Where(i => i.IsDelete == false).ToList();
            return View(allcategories);
        }

        public ActionResult AddCategory()
        {
            return UpdateCategory(0);
        }

        public ActionResult UpdateCategory(int? CategoryId)
        {
            CategoryDetail cd;
            if(CategoryId != null)
            {
                int cid = (int)CategoryId;
                cd = JsonConvert.DeserializeObject<CategoryDetail>(JsonConvert.SerializeObject(_unitOfWork.GetRepositoryInstance<Category>().GetFirstorDefault(cid)));
            }
            else
            {
                cd = new CategoryDetail();
            }
            return View("UpdateCategory", cd);
        }

        public ActionResult Product()
        {
            return View(_unitOfWork.GetRepositoryInstance<Product>().GetProduct());
        }

        public ActionResult CategoryEdit(int categoryId)
        {
            return View(_unitOfWork.GetRepositoryInstance<Category>().GetFirstorDefault(categoryId));
        }

        [HttpPost]
        public ActionResult CategoryEdit(Category cd)
        {
            _unitOfWork.GetRepositoryInstance<Category>().Update(cd);
            return RedirectToAction("Categories");
        }

        public ActionResult ProductEdit(int productId)
        {
            ViewBag.CategoryList = GetCategory();
            return View(_unitOfWork.GetRepositoryInstance<Product>().GetFirstorDefault(productId));

        }

        [HttpPost]
        public ActionResult ProductEdit(Product pd, HttpPostedFileBase file)
        {
            string pic = null;
            if (file != null)
            {
                pic = System.IO.Path.GetFileName(file.FileName);
                string path = System.IO.Path.Combine(Server.MapPath("~/Images/"), pic);
                file.SaveAs(path);
            }
            pd.ProductImage = file != null ? pic:pd.ProductImage;
            pd.ModifiedDate = DateTime.Now;
            _unitOfWork.GetRepositoryInstance<Product>().Update(pd);
            return RedirectToAction("Product");
        }

        public ActionResult ProductAdd()
        {
            ViewBag.CategoryList = GetCategory();
            return View();

        }

        [HttpPost]
        public ActionResult ProductAdd(Product pd, HttpPostedFileBase file)
        {
            string pic=null;
            if(file != null)
            {
                pic = System.IO.Path.GetFileName(file.FileName);
                string path = System.IO.Path.Combine(Server.MapPath("~/Images/"), pic);
                file.SaveAs(path);
            }
            pd.ProductImage = pic;
            pd.CreatedDate = DateTime.Now;
            _unitOfWork.GetRepositoryInstance<Product>().Add(pd);
            return RedirectToAction("Product");
        }

    }
}