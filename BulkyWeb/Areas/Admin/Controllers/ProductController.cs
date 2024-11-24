
using Bulky.DataAccess.Migrations;
using Bulky.DataAccess.Repository.IRepository;

using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Data;
using System.Drawing;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        // private readonly ApplicationDbContext _db;
        // private readonly IProductRepository _ProductRepo;
        private readonly IUnitOfWork _unitofwork;
        private readonly IWebHostEnvironment _webHostEnvironment;//no need to register hving inbuilt functionality
        public ProductController(IUnitOfWork unitofwork, IWebHostEnvironment webHostEnvironment)
        {
            _unitofwork = unitofwork;
            _webHostEnvironment= webHostEnvironment;
        }
        public IActionResult Index()
        {
            List<Product> objProductList = _unitofwork.Product.GetAll(includeProperties:"Category").ToList();
           
            return View(objProductList);
        }

        public IActionResult Upsert(int? id)//create +update
        {
            // ViewBag.CategoryList = CategoryList;
            // ViewData["CategoryList"]= CategoryList
            ProductVM productVM = new()
            {

               CategoryList = _unitofwork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()

                }),

                Product = new Product()
           };

            if(id==null || id==0)
            {
                //create
                return View(productVM);
            }
            else
            {
                //update
                productVM.Product = _unitofwork.Product.Get(u => u.Id == id);
                return View(productVM);
            }
           
        }
        [HttpPost]
        public IActionResult Upsert(ProductVM productVM,IFormFile? file)
        {

          
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if(file !=null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productpath= Path.Combine(wwwRootPath,@"images\product");

                    if(!String.IsNullOrEmpty(productVM.Product.ImageUrl))
                    {
                        //delete the old image
                        string OldImagePath = Path.Combine(wwwRootPath, productVM.Product.ImageUrl.Trim('\\')); 
                       
                        if(System.IO.File.Exists(OldImagePath))
                        {
                            System.IO.File.Delete(OldImagePath);
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(productpath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    productVM.Product.ImageUrl = @"\images\product\" + fileName;
                    if(productVM.Product.Id==0)
                    {
                        _unitofwork.Product.Add(productVM.Product);
                    }
                    else
                    {
                        _unitofwork.Product.Update(productVM.Product);
                    }
;                }

               // _unitofwork.Product.Add(productVM.Product);
                _unitofwork.Save();
                TempData["success"] = "Product Created Successfully";
                return RedirectToAction("Index");
            }
            else
            {

                productVM.CategoryList = _unitofwork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()

                });

                
                return View(productVM);
            }
         
        }






        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var productToBeDeleted = _unitofwork.Product.Get(u => u.Id == id);
            if(productToBeDeleted==null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            //delete the old image
            string OldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, productToBeDeleted.ImageUrl.Trim('\\'));

            if (System.IO.File.Exists(OldImagePath))
            {
                System.IO.File.Delete(OldImagePath);
            }
            _unitofwork.Product.Remove(productToBeDeleted);
            _unitofwork.Save();
           // List<Product> objProductList = _unitofwork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new { success=true,message="Delete Successful" });

        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> objProductList = _unitofwork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = objProductList });

        }

        #endregion


    }

}
