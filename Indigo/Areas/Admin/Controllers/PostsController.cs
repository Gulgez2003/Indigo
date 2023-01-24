using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Indigo.DAL;
using Indigo.Models;
using Indigo.Dtos.PostDtos;

namespace Indigo.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PostsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public PostsController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Admin/Posts
        public async Task<IActionResult> Index()
        {
              return View(await _context.Posts.ToListAsync());
        }


        // GET: Admin/Posts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PostPostDto postDto)
        {

            string imageName=Guid.NewGuid()+postDto.formFile.FileName;
            string imagePath = Path.Combine(_env.WebRootPath, "assets/images", imageName);
            using(FileStream fileStream=new FileStream(imagePath,FileMode.Create))
            {
                postDto.formFile.CopyTo(fileStream);
            }
            await _context.Posts.AddAsync(new Post
            {
                Name = postDto.Name,
                Description = postDto.Description,
                ImagePath = imageName
            });
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/Posts/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            Post? post = _context.Posts.Find(id);
            if (post == null)
            {
                return NotFound();
            }

            PostUpdateDto postUpdateDto = new PostUpdateDto()
            {
                postGetDto = new PostGetDto()
                {
                    Name = post.Name,
                    Description = post.Description,
                    ImagePath = post.ImagePath
                }
            };
            return View(postUpdateDto);
        }

        // POST: Admin/Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit( PostUpdateDto updateDto)
        {
            if (_context.Posts == null)
            {
                return NotFound();
            }
            Post? post = _context.Posts.Find(updateDto.postGetDto.Id);
            if (post == null)
            {
                return NotFound();
            }

            string imageName = $"{Guid.NewGuid()}{updateDto.postPostDto.formFile.FileName}";
            string imagePath = Path.Combine(Guid.NewGuid().ToString(), "assets/images", imageName);
            using (FileStream fileStream = new FileStream(imagePath, FileMode.Create))
            {
                updateDto.postPostDto.formFile.CopyTo(fileStream);
            }

            post.Name = updateDto.postGetDto.Name;
            post.Description=updateDto.postGetDto.Description;
            post.ImagePath = imageName;

            _context.SaveChanges();
            return RedirectToAction(nameof(Index));

        }

        // GET: Admin/Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            Post post = await _context.Posts.FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Admin/Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Posts == null)
            {
                return Problem("Entity set 'AppDbContext.Posts'  is null.");
            }
            var post = await _context.Posts.FindAsync(id);
            if (post != null)
            {
                _context.Posts.Remove(post);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
          return _context.Posts.Any(e => e.Id == id);
        }
    }
}
