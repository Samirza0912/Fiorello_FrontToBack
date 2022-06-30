﻿using Friello.DAL;
using Friello.Models;
using Friello.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Friello.Controllers
{
    public class BasketController : Controller
    {
        private readonly AppDbContext _context;

        public BasketController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> AddItem(int? id)
        {
            if (id == null) return NotFound();
            Product dbProduct = await _context.Products.FindAsync(id);
            if (dbProduct == null) return NotFound();
            List<BasketVM> products;
            if (Request.Cookies["basket"] == null)
            {
                products = new List<BasketVM>();
            }
            else
            {
                products = JsonConvert.DeserializeObject<List<BasketVM>>(Request.Cookies["basket"]);
            }
            BasketVM existProduct = products.Find(x => x.Id == id);
            if (existProduct==null)
            {
                BasketVM basketVm = new BasketVM
                {
                    Id = dbProduct.Id,
                    ProductCount = 1
                };
                products.Add(basketVm);
            }
            else
            {
                existProduct.ProductCount++;
            }
        
            Response.Cookies.Append("basket", JsonConvert.SerializeObject(products), new CookieOptions { MaxAge = TimeSpan.FromDays(5) });
            //HttpContext.Session.SetString("name", "Samir");
            //Response.Cookies.Append("group", "p322", new CookieOptions { MaxAge = TimeSpan.FromDays(14) });

            return RedirectToAction("index", "home");
        }
        public IActionResult ShowItem()
        {
            //string name = HttpContext.Session.GetString("name");
            //string group = Request.Cookies["group"];
            string basket = Request.Cookies["basket"];
            List<BasketVM> products;
            if (basket!=null)
            {
                products = JsonConvert.DeserializeObject<List<BasketVM>>(basket);
                foreach (var item in products)
                {
                    Product dbProduct = _context.Products.FirstOrDefault(p => p.Id == item.Id);
                    item.Price = dbProduct.Price;
                    item.ImageUrl = dbProduct.ImageUrl;
                    item.Name = dbProduct.Name;
                }
            }
            else
            {
                products = new List<BasketVM>();
            }
            return View(products);
        }
        public IActionResult Remove(int id)
        {
            string basket = Request.Cookies["basket"];
            List<BasketVM> products = JsonConvert.DeserializeObject<List<BasketVM>>(basket);
            BasketVM removeProduct = products.Find(p => p.Id == id);
            products.Remove(removeProduct);
            Response.Cookies.Append("basket", JsonConvert.SerializeObject(products), new CookieOptions { MaxAge = TimeSpan.FromDays(14) });
            return RedirectToAction("showItem", "basket");
        }
        public IActionResult Minus(int id)
        {

        }
        public IActionResult Plus(int id)
        {

        }
    }
}