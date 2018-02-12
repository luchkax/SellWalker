using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Linq;
using sellwalker.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace sellwalker.Controllers
{
    public class HomeController : Controller
    {
        private SellContext _context;
        private IHostingEnvironment _hostingEnvironment;

        public HomeController(SellContext context, IHostingEnvironment environment)
        {
            _context = context;
            _hostingEnvironment = environment;
        }

        private bool checkLogStatus()
        {
            int? id = HttpContext.Session.GetInt32("userId");
            if (id == null)
            {
                TempData["UserError"] = "You must be logged in!";
                return false;
            }
            else
            {
                return true;
            }
        }

// RENDER HOMEPAGE //
        [HttpGet]
        [Route("/home")]
        public IActionResult Homepage()
        {
            int? id = HttpContext.Session.GetInt32("userId");
            if(checkLogStatus() == false)
            {
                return RedirectToAction("LoginPage", "User");                           
            }
            else
            {   
                User exists = _context.Users.Where(u=>u.UserId == id).SingleOrDefault();
                ViewBag.name = exists.FirstName;
                if(exists.Status != "Admin")
                {
                    return View("Homepage");
                }
                else
                {
                    return View("AdminHomepage");
                }
            }
        }
// RENDER ADD_PRODUCT 
        [HttpGet]
        [Route("/add_product")]
        public IActionResult AddProduct()
        {
            int? id = HttpContext.Session.GetInt32("userId");
            if(id == null)
            {
                return RedirectToAction("LoginPage", "User");                           
            }
            else
            {
                User exists = _context.Users.Where(u=>u.UserId == id).SingleOrDefault();
                if(exists.Status != "Admin")
                {
                    return View("NewProduct");
                }
                else
                {
                    
                    return View("AdminNewProduct");
                }
            }
        }
// POST CREATE_PRODUCT
        [HttpPost]
        [Route("/create_product")]
        public async Task<IActionResult> productAdd(ProductCheck check)
        {
            int? id = HttpContext.Session.GetInt32("userId");
            if(id == null)
            {
                return RedirectToAction("LoginPage", "User");                           
            }
            else
            {
                if(ModelState.IsValid)
                {
                    User exists = _context.Users.Where(u=>u.UserId == id).SingleOrDefault();
                    Product newProduct = new Product
                    {
                        Title = check.Title,
                        Description = check.Description,
                        Price = check.Price,  
                        UserId = (int)id,
                        CreatedAt = DateTime.Now,
                        Condition = check.Condition
                    };
                    var uploadDestination = Path.Combine(_hostingEnvironment.WebRootPath, "uploaded_images");
                    if (check.Image != null)
                    {
                        var filepath = Path.Combine(uploadDestination, check.Image.FileName);
                        using (var fileStream = new FileStream(filepath, FileMode.Create))
                        {
                            await check.Image.CopyToAsync(fileStream);
                            newProduct.Picture = "/uploaded_images/" + check.Image.FileName;
                        }
                    }
                    _context.Add(newProduct);
                    _context.SaveChanges();
                }
                else
                {
                    TempData["error"] = "Not added. Error";
                }
                return RedirectToAction("Homepage");
            }
        }
// RENDER SETTINGS
        [HttpGet]
        [Route("/settings")]
        public IActionResult Settings()
        {
            int? id = HttpContext.Session.GetInt32("userId");
            if(id == null)
            {
                return RedirectToAction("LoginPage", "User");                           
            }
            else
            {
                User exists = _context.Users.Where(u=>u.UserId == id).SingleOrDefault();
                ViewBag.userFirst = exists.FirstName;
                ViewBag.userLast = exists.LastName;
                ViewBag.userEmail = exists.Email;
                ViewBag.userPic = exists.ProfilePic;
            
                if(exists.Status != "Admin")
                {
                    return View("Settings");
                }
                else
                {
                    return View("AdminSettings");
                }
            }
        }

// POST UPDATE USER FROM SETTINGS
        [HttpPost]
        [Route("/update_user")]
        public async Task<IActionResult> UpdateInfo(UpdateUser check)
        {
            int? id = HttpContext.Session.GetInt32("userId");
            if(id == null)
            {
                return RedirectToAction("LoginPage", "User");                           
            }
            else
            {
                User thisUser = _context.Users.Where(u=>u.UserId == id).SingleOrDefault();
                if(ModelState.IsValid)
                {
                    thisUser.FirstName = check.FirstName;
                    thisUser.LastName = check.LastName;
                    thisUser.Email = check.Email;
                    _context.SaveChanges();
                    var uploadDestination = Path.Combine(_hostingEnvironment.WebRootPath, "profile_images");
                    if (check.ProfileImage == null)
                    {
                        _context.SaveChanges();
                        if(thisUser.Status != "Admin")
                        {
                            return RedirectToAction("Settings");
                        }
                        else
                        {
                            return View("AdminSettings");
                        }
                    }
                    else
                    {
                        var filepath = Path.Combine(uploadDestination, check.ProfileImage.FileName);
                        using (var fileStream = new FileStream(filepath, FileMode.Create))
                        {
                            await check.ProfileImage.CopyToAsync(fileStream);
                            thisUser.ProfilePic = "/profile_images/" + check.ProfileImage.FileName;
                        }
                        _context.SaveChanges();

                        if(thisUser.Status != "Admin")
                        {
                            TempData["updated"] = "+";
                            return View("Settings");
                        }
                        else
                        {
                            TempData["updated"] = "+";
                            return View("AdminSettings");
                        }
                    }
                }
                else
                {
                    if(thisUser.Status != "Admin")
                    {
                        TempData["updated"] = "-";
                        return View("Settings");
                    }
                    else
                    {
                        TempData["updated"] = "-";                       
                        return View("AdminSettings");
                    }
                }
            }



        }

    }               
}
