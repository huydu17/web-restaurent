using WebRestaurant.DataAccess.Repository.IRepository;
using WebRestaurant.DataAcess.Data;
using WebRestaurant.Models;
using WebRestaurant.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Data;
using WebRestaurant.Utility;
using AspNetCoreHero.ToastNotification.Abstractions;

namespace WebRestaurant.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotyfService _notyf;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment, INotyfService notyf)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
            _notyf = notyf;
        }
        public IActionResult Index() 
        {
            List<Product> products = _unitOfWork.Product.GetAll(includeProperties:"Category").ToList();
           
            return View(products);
        }

        public IActionResult Create()
        {
            ProductVM productVM = new()
            {
                CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Product = new Product()
            };
            return View(productVM);        
        }
        [HttpPost]
        public IActionResult Create(ProductVM productVM, List<IFormFile> files)
        {
            if (!ModelState.IsValid)
            {
                productVM.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                return View(productVM);
            }            
            _unitOfWork.Product.Add(productVM.Product);
            _unitOfWork.Save();
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            if (files != null)
            {
                foreach(IFormFile file in files) 
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = @"images\products\product-" + productVM.Product.Id;
                    string finalPath = Path.Combine(wwwRootPath, productPath);

                    if (!Directory.Exists(finalPath))
                        Directory.CreateDirectory(finalPath);

                    using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create)) {
                        file.CopyTo(fileStream);
                    }

                    ProductImage productImage = new() {
                        ImageUrl = @"\" + productPath + @"\" + fileName,
                        ProductId=productVM.Product.Id,
                    };

                    if (productVM.Product.ProductImages == null)
                        productVM.Product.ProductImages = new List<ProductImage>();

                    productVM.Product.ProductImages.Add(productImage);
                }
                _unitOfWork.Product.Update(productVM.Product);
                _unitOfWork.Save();
                
            }
            _notyf.Success("Thêm sản phẩm thành công");
            return RedirectToAction("Index");           
        }

        public IActionResult Edit(int id)
        {
            var check = _unitOfWork.Product.Get(x=>x.Id == id);
            if(check == null) {
                return NotFound();
            }
            ProductVM productVM = new ProductVM()
            {
                CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Product = _unitOfWork.Product.Get(u => u.Id == id, includeProperties: "ProductImages")
            };
            return View(productVM);
        }

        [HttpPost]
        public IActionResult Edit(ProductVM productVM, List<IFormFile> files)
        {
            var product = _unitOfWork.Product.Get(x => x.Id == productVM.Product.Id);
            if(product == null)
            {
                return NotFound();  
            }
            if (!ModelState.IsValid)
            {
                productVM.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                return View();
            }  
            _unitOfWork.Product.Update(productVM.Product);   
            _unitOfWork.Save();
            //lấy - cập nhật ảnh
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            if (files != null)
            {
                foreach (IFormFile file in files)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = @"images\products\product-" + productVM.Product.Id;
                    string finalPath = Path.Combine(wwwRootPath, productPath);

                    if (!Directory.Exists(finalPath))
                        Directory.CreateDirectory(finalPath);

                    using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    ProductImage productImage = new()
                    {
                        ImageUrl = @"\" + productPath + @"\" + fileName,
                        ProductId = productVM.Product.Id,
                    };

                    if (productVM.Product.ProductImages == null)
                        productVM.Product.ProductImages = new List<ProductImage>();

                    productVM.Product.ProductImages.Add(productImage);

                }

                _unitOfWork.Product.Update(productVM.Product);
                _unitOfWork.Save();
            }
            productVM.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });
            _notyf.Success("Cập nhật sản phẩm thành công");
            return RedirectToAction("Index");
        }

        public IActionResult DeleteImage(int imageId) {
            var imageToBeDeleted = _unitOfWork.ProductImage.Get(u => u.Id == imageId);
            int productId = imageToBeDeleted.ProductId;
            if (imageToBeDeleted != null) {
                if (!string.IsNullOrEmpty(imageToBeDeleted.ImageUrl)) {
                    var oldImagePath =
                                   Path.Combine(_webHostEnvironment.WebRootPath,
                                   imageToBeDeleted.ImageUrl.TrimStart('\\'));

                    if (System.IO.File.Exists(oldImagePath)) {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                _unitOfWork.ProductImage.Remove(imageToBeDeleted);
                _unitOfWork.Save();

                _notyf.Success("Xoá ảnh thành công");
            }

            return RedirectToAction(nameof(Edit), new { id = productId });
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").OrderByDescending(x=>x.UpdateAt).ToList();
            return Json(new { data = objProductList });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var productToBeDeleted = _unitOfWork.Product.Get(u => u.Id == id);
            if (productToBeDeleted == null)
            {
                return Json(new { success = false, message = "Lỗi trong khi xoá!" });
            }

            string productPath = @"images\products\product-" + id;
            string finalPath = Path.Combine(_webHostEnvironment.WebRootPath, productPath);

            if (Directory.Exists(finalPath)) {
                string[] filePaths = Directory.GetFiles(finalPath);
                foreach (string filePath in filePaths) {
                    System.IO.File.Delete(filePath);
                }

                Directory.Delete(finalPath);
            }
            _unitOfWork.Product.Remove(productToBeDeleted);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Xoá sản phầm thành công!" });
        }

        #endregion
    }
}
