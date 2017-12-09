using SportsStore.Domain.Abstract;
using SportsStore.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SportsStore.WebUI.Controllers
{
    public class ProductController : Controller
    {
        private IProductRepository _productRepository;
        public int pageSize = 4;

        public ProductController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public ViewResult List(string category, int page = 1)
        {
            var model = new ProductListViewModel()
            {
                Products = _productRepository.Products
                .Where(x => category == null || x.Category == category)
                .OrderBy(p => p.ProductID)
                .Skip((page - 1) * pageSize).Take(pageSize),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = pageSize,
                    TotalItems = category == null ?
                    _productRepository.Products.Count() :
                    _productRepository.Products.Where(x => x.Category == category).Count()
                },
                CurrentCategory = category                
            };

            return View(model);
        }

        public FileContentResult GetImage(int productId)
        {
            var prod = _productRepository.Products.FirstOrDefault(x => x.ProductID == productId);
            if(prod != null)
            {
                return File(prod.ImageData, prod.ImageMimeType);
            }
            else
            {
                return null;
            }
        }
    }
}