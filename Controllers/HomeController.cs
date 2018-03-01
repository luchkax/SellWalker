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

        private bool checkUserStatus()
        {
            User userCheck = _context.Users.Where(u=>u.UserId == HttpContext.Session.GetInt32("userId")).SingleOrDefault();
            if(userCheck.Status != "Admin")
            {
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
                ViewBag.user = id;
                List<Product> allProducts = _context.Products.Where(s=>s.Status != "Sold").Include(o=>o.Orders).OrderByDescending(h=>h.CreatedAt).ToList();
                User all = _context.Users.Where(a=>a.UserId == id).Include(o=>o.products).ThenInclude(p=>p.Orders).SingleOrDefault();

                ViewBag.all = all;
                ViewBag.allProd = allProducts;
                ViewBag.name = exists.FirstName;
                
                if(checkUserStatus() == false)
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
                ViewBag.user = id;
                if(checkUserStatus() == false)
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
                        Condition = check.Condition,
                        Status = "Active"
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
                ViewBag.user = id;
                ViewBag.userFirst = exists.FirstName;
                ViewBag.userLast = exists.LastName;
                ViewBag.userEmail = exists.Email;
                ViewBag.userPic = exists.ProfilePic;
            
                if(checkUserStatus() == false)
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
                        if(checkUserStatus() == false)
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
                    if(checkUserStatus() == false)
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

// RENDER PRODUCTS CONTROL PAGE 
        [HttpGet]
        [Route("/products_control")]
        public IActionResult ProductsControl()
        {
            int? id = HttpContext.Session.GetInt32("userId");
            if(id == null)
            {
                return RedirectToAction("LoginPage", "User");                           
            }
            else
            {
                User exists = _context.Users.Where(u=>u.UserId == id).SingleOrDefault();
                ViewBag.user = id;
                List<Product> allProducts = _context.Products.Include(u=>u.Seller).OrderBy(y=>y.Title).ToList();
                ViewBag.products = allProducts;
                return View("ProductsControl");
            }
        }

//  DELETE METHOD FOR PRODUCT
        [HttpGet]
        [Route("/product/{productId}/delete")]
        public IActionResult deleteProduct(int productId)        
        {
            int? id = HttpContext.Session.GetInt32("userId");
            if(id == null)
            {
                return RedirectToAction("LoginPage", "User");                           
            }
            else
            {
                Product thisProduct = _context.Products.Where(a=>a.ProductId == productId).SingleOrDefault();
                _context.Products.Remove(thisProduct);
                _context.SaveChanges();
                return RedirectToAction("ProductsControl");
            }
        }

// RENDER PAGE FOR PRODUCT ALL USERS
    
        [HttpGet]
        [Route("/product/{productId}")]
        public IActionResult ProductPage(int productId)        
        {
            int? id = HttpContext.Session.GetInt32("userId");
            if(id == null)
            {
                return RedirectToAction("LoginPage", "User");                           
            }
            else
            {
                User exists = _context.Users.Where(u=>u.UserId == id).SingleOrDefault();
                ViewBag.user = id;
                Product thisProduct = _context.Products.Where(a=>a.ProductId == productId).SingleOrDefault();
                User thisSeller = _context.Users.Where(a=>a.UserId == thisProduct.UserId).SingleOrDefault();
                List<Product> thoseProducts = _context.Products.Where(r=>r.UserId == thisSeller.UserId).ToList();
                string someDate = thisProduct.CreatedAt.ToString();
                DateTime startDate = DateTime.Parse(someDate);
                DateTime now = DateTime.Now;
                TimeSpan elapsed = now.Subtract(startDate);
                double daysAgo = elapsed.TotalDays;

                ViewBag.date = daysAgo.ToString("0");
                ViewBag.thoseProducts = thoseProducts;
                ViewBag.thisSeller = thisSeller;
                ViewBag.thisProduct = thisProduct;
                ViewBag.loggedID = id;

                if(checkUserStatus() == false)
                {
                    return View("ProductPage");
                }
                else
                {
                    return View("AdminProductPage");
                }
            }
        }

// RENDER EDIT PAGE FOR PRODUCT
        [HttpGet]
        [Route("/product/{productId}/edit")]
        public IActionResult ProductPageEdit(int productId)        
        {
            if(checkLogStatus() == false)
            {
                return RedirectToAction("LoginPage", "User");                           
            }
            else
            {   
                int? id = HttpContext.Session.GetInt32("userId");
                ViewBag.user = id;
                Product thisProduct = _context.Products.Where(a=>a.ProductId == productId).SingleOrDefault();
                ViewBag.thisProduct = thisProduct;
                ViewBag.title = thisProduct.Title;
                if(checkUserStatus() == false)
                {
                    return View("ProductPageEdit");
                }
                else
                {
                    return View("AdminProductPageEdit");
                }
            }
        }

// POST FOR EDIT PRODUCT ALL USERS
        [HttpPost]
        [Route("/product/edit/{productId}")]
        public async Task<IActionResult> EditProduct(ProductCheck check, int productId)        
        {
            if(checkLogStatus() == false)
            {
                return RedirectToAction("LoginPage", "User");                           
            }
            else
            {   
                Product thisProduct = _context.Products.Where(a=>a.ProductId == productId).SingleOrDefault();
                if(ModelState.IsValid)
                {
                
                    thisProduct.Title = check.Title;
                    thisProduct.Description = check.Description;
                    thisProduct.Price = check.Price;
                    thisProduct.Condition = check.Condition;
                    thisProduct.Status = check.Status;
                    var uploadDestination = Path.Combine(_hostingEnvironment.WebRootPath, "uploaded_images");
                    if (check.Image != null)
                    {
                        var filepath = Path.Combine(uploadDestination, check.Image.FileName);
                        using (var fileStream = new FileStream(filepath, FileMode.Create))
                        {
                            await check.Image.CopyToAsync(fileStream);
                            thisProduct.Picture = "/uploaded_images/" + check.Image.FileName;
                        }
                    }
                    TempData["complete"] = "true";
                    _context.SaveChanges();
                    return RedirectToAction("ProductPageEdit");
                }
                else
                {
                    if(checkUserStatus() == false)
                    {
                        TempData["Error"] = "Inputed data incorrect";
                        return View("ProductPageEdit");
                    }
                    else
                    {
                        TempData["Error"] = "Inputed data incorrect";                        
                        return View("AdminProductPageEdit");
                    }
                }
            }
        }

// RENDER CUSTOMERS CONTROLL ADMIN PAGE
        [HttpGet]
        [Route("/customers_control")]
        public IActionResult CustomerssControl()
        {
            if(checkLogStatus() == false)
            {
                return RedirectToAction("LoginPage", "User");                           
            }
            else
            {
                int? id = HttpContext.Session.GetInt32("userId");
                ViewBag.user = id;
                List<User> allUsers = _context.Users.Include(u=>u.products).ThenInclude(y=>y.Orders).ToList();
                ViewBag.customers = allUsers;
                if(checkUserStatus() == false)
                {
                    return RedirectToAction("HomePage");
                }
                else
                {
                    return View("CustomerControl");
                }
                
            }
        }

// RENDER USER PAGE
        [HttpGet]
        [Route("/user/{userId}")]
        public IActionResult UserPage(int userId)
        {
            if(checkLogStatus() == false)
            {
                return RedirectToAction("LoginPage", "User");                           
            }
            else
            {
                int? id = HttpContext.Session.GetInt32("userId");
                ViewBag.user = id;
                User thisUser = _context.Users.Where(u=>u.UserId == userId).Include(p=>p.products).SingleOrDefault();
                List<Review> theseReviews = _context.Reviews.Where(r=>r.ReviewedId == thisUser.UserId).Include(u=>u.Reviewer).ToList();
                ViewBag.thisUser = thisUser;
                ViewBag.theseReviews = theseReviews;
                
                

                if(checkUserStatus() == false)
                {
                    return View("UserPage");
                }
                else
                {
                    return View("AdminUserPage");
                }            
            }
        }

// RENDER EDIT PAGE ADMIN
        [HttpGet]
        [Route("/user/{userId}/edit")]
        public IActionResult EditUserPage(int userId)
        {
            if(checkLogStatus() == false)
            {
                return RedirectToAction("LoginPage", "User");                           
            }
            else
            {
                int? id = HttpContext.Session.GetInt32("userId");
                ViewBag.user = id;
                User thisUser = _context.Users.Where(u=>u.UserId == userId).Include(p=>p.products).SingleOrDefault();

                ViewBag.thisUser = thisUser;
                ViewBag.name = thisUser.FirstName+"'s";
                return View("AdminEditUser");
            }
        }

// POST FOR SAVING CHANGES ON ADMIN EDIT PAGE
        [HttpPost]
        [Route("/editstatus/{userId}")]
        public IActionResult EditUserStatus(UpdateUserInfo check, int userId)
        {
            if(checkLogStatus() == false)
            {
                return RedirectToAction("LoginPage", "User");                           
            }
            else
            {
                int? id = HttpContext.Session.GetInt32("userId");
                User thisUser = _context.Users.Where(u=>u.UserId == userId).Include(p=>p.products).SingleOrDefault();
                if(thisUser.UserId != 1)
                {
                    if(ModelState.IsValid){
                        thisUser.Status = check.Status;
                        thisUser.ReviewedId = check.ReviewedId;
                        _context.SaveChanges();
                        TempData["error"] = "User Status Changed successfully!";
                        return RedirectToAction("EditUserPage");
                    }
                    else
                    {
                        TempData["error"] = "Inputs invalid!";
                        return RedirectToAction("EditUserPage");
                    }
                }
                else
                {
                    TempData["error"] = "You cannot change status of this user!";
                    return RedirectToAction("EditUserPage");
                }
            }
        }

// POST ADD TO CART PRODUCT 
        [HttpPost]
        [Route("/addtocart/product/{productId}")]
        public IActionResult AddToCart(int productId)
        {
            if(checkLogStatus() == false)
            {
                return RedirectToAction("LoginPage", "User");                           
            }
            else
            {
                int? id = HttpContext.Session.GetInt32("userId");
                Order thisOrderItem = _context.Orders.Where(u=>u.UserId == id).Where(y=>y.ProductId == productId).SingleOrDefault();
                Product thisProductItem = _context.Products.Where(p=>p.ProductId == productId).SingleOrDefault();
                // Product myProduct = _context.Products.Where(u=>u.UserId == id).SingleOrDefault();
                if(thisOrderItem != null){
                    return RedirectToAction("ProductPage");
                }
                else
                {                    
                        thisProductItem.Status = "InCart";  
                        Order newOrder = new Order{
                            UserId = (int)id,
                            ProductId = productId,
                            Quantity = 1,
                            CreatedAt = DateTime.Now
                        };
                        _context.Add(newOrder);
                        _context.SaveChanges();
                        return RedirectToAction("CartPage");
                }
            }
        }

//GET DELETE PRODUCT FROM CART
        [HttpGet]
        [Route("/deleteCart/{productId}")]
        public IActionResult DeleteFromCart(int productId)
        {
            if(checkLogStatus() == false)
            {
                return RedirectToAction("LoginPage", "User");                           
            }
            else
            {
                int? id = HttpContext.Session.GetInt32("userId");
                Product thisProductItem = _context.Products.Where(p=>p.ProductId == productId).SingleOrDefault();
                thisProductItem.Status = "Active";
                Order order = _context.Orders.Where(u=>u.UserId == id).Where(p=>p.ProductId == productId).SingleOrDefault();
                _context.Orders.Remove(order);
                _context.SaveChanges();

                return RedirectToAction("HomePage");
            }
        }

// RENDER CART ALL USERS
        [HttpGet]
        [Route("/cart")]
        public IActionResult CartPage()
        {
            if(checkLogStatus() == false)
            {
                return RedirectToAction("LoginPage", "User");                           
            }
            else
            {
                int? id = HttpContext.Session.GetInt32("userId");
                ViewBag.user = id;
                User user = _context.Users.Where(a=>a.UserId == id).Include(o=>o.products).ThenInclude(p=>p.Orders).SingleOrDefault();
                List<Order> orders = _context.Orders.Where(a=>a.UserId == id).Include(o=>o.product).ToList();
                ViewBag.Status = user.Status;
                ViewBag.Orders = orders;
                // Order thisOrderItem = _context.Orders.Where(u=>u.UserId == id).Where(y=>y.ProductId == productId).SingleOrDefault();
                return View("Cart");
            }
        }
// RENDER AND POST SEARCH ALL USERS
        [HttpPost]
        [Route("/search")]
        public IActionResult SearchProduct(string result)
        {
            if(checkLogStatus() == false)
            {
                return RedirectToAction("LoginPage", "User");                           
            }
            else
            {
                int? id = HttpContext.Session.GetInt32("userId");
                ViewBag.user = id;
                if(result != null)
                {
                    List<Product> searchProduct = _context.Products.Where(n=>n.Title.ToLower().Contains(result.ToLower())).Include(o=>o.Orders).ToList();
                    if(searchProduct.Count < 1)
                    {
                        ViewBag.error = "There are no products with the name " + result;
                    }
                    ViewBag.searchRes = searchProduct;
                    return View("SearchPage");
                }
                else
                {
                    ViewBag.error = "Please give the product name you want ot search";
                    return RedirectToAction("HomePage");
                }
            }
        }

        [HttpGet]
        [Route("/purchase/{productId}")]
        public IActionResult Purchase (int productId)
        {
            if(checkLogStatus() == false)
            {
                return RedirectToAction("LoginPage", "User");                           
            }
            else
            {
                 int? id = HttpContext.Session.GetInt32("userId");
                Product thisProduct = _context.Products.Where(a=>a.ProductId == productId).SingleOrDefault();
                Order thisOrderItem = _context.Orders.Where(u=>u.UserId == id).Where(y=>y.ProductId == productId).SingleOrDefault();
                if(thisProduct.Status == "InCart")
                {
                    thisProduct.Status = "Sold";
                    _context.Orders.Remove(thisOrderItem);
                    _context.SaveChanges();
                    return RedirectToAction("HomePage");
                }
                else
                {
                    ViewBag.error = "Error occured. Contact your administrator.";
                    return RedirectToAction("CartPage");
                }
            }
        }

        [HttpPost]
        [Route("/create_review/{userId}")]
        public IActionResult CreateReview(ReviewCheck check, int userId)
        {
            if(checkLogStatus() == false)
            {
                return RedirectToAction("LoginPage", "User");                           
            }
            else
            {
                int? id = HttpContext.Session.GetInt32("userId");
                ViewBag.user = id;
                if(ModelState.IsValid)
                {
                    if(userId != id)
                    {
                        User reviewed = _context.Users.Where(u=>u.UserId == userId).SingleOrDefault();
                        User reviewer = _context.Users.Where(u=>u.UserId == id).SingleOrDefault();
                        Review newReview = new Review
                        {
                            Content = check.Content,
                            CreatedAt = DateTime.Now,
                            UserId = reviewer.UserId,
                            ReviewedId = userId
                        };
                        _context.Add(newReview);
                        _context.SaveChanges();

                        return RedirectToAction("UserPage");
                    }
                    else
                    {
                        ViewBag.error = "Input incorrect";
                        return RedirectToAction("UserPage");
                    }
                }
                else
                {
                    ViewBag.error = "Input incorrect";
                    return RedirectToAction("UserPage");
                }
            }
        }


    }               
}
