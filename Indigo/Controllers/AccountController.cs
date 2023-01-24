using Indigo.Dtos.UserDtos;
using Indigo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace Indigo.Controllers
{
    public class AccountController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        public AccountController(RoleManager<IdentityRole> roleManager, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
        }
 
        //public async Task<IActionResult> Create()
        //{
        //    await _roleManager.CreateAsync(new IdentityRole { Name = "User" });
        //    await _roleManager.CreateAsync(new IdentityRole { Name = "Admin" });
        //    await _roleManager.CreateAsync(new IdentityRole { Name = "SuperAdmin" });
        //    return Json("okay");
        //}

        // GET: AccountController/Edit/5
        public ActionResult Register()
        {
            return View();
        }

        // POST: AccountController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterDto registerDto)
        {
            User user = new User()
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Email = registerDto.Email,
                UserName = registerDto.UserName
            };
            IdentityResult result= await _userManager.CreateAsync(user,registerDto.Password);
            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("",item.Description);
                }
            }
            return RedirectToAction("Index","Home");   
        }
        public ActionResult Login()
        {
            return View();
        }

        // POST: AccountController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginDto loginDto)
        {
            if(string.IsNullOrEmpty(loginDto.UserName)|| string.IsNullOrEmpty(loginDto.Password))
            {
                ModelState.AddModelError("", "Username or Password incorrect");
                return View();
            }
            User user = await _userManager.FindByNameAsync(loginDto.UserName);
            if(user == null)
            {
                if (string.IsNullOrEmpty(loginDto.UserName) || string.IsNullOrEmpty(loginDto.Password))
                {
                    ModelState.AddModelError("", "Username or Password incorrect");
                    return View();
                }
            }
            Microsoft.AspNetCore.Identity.SignInResult signInManager = await _signInManager.PasswordSignInAsync(user, loginDto.Password, loginDto.RememberMe, true);
            if (!signInManager.Succeeded)
            {
                ModelState.AddModelError("", "Username or Password incorrect");
                return View();
            }
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
