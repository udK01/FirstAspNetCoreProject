﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FirstAspNetCoreProject.Data;
using FirstAspNetCoreProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace FirstAspNetCoreProject.Controllers
{
    public class JokesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public JokesController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _environment = environment;
            _context = context;
        }

        // GET: Jokes
        public async Task<IActionResult> Index()
        {
            return View(await _context.Joke.ToListAsync());
        }

        // GET: Jokes/ShowSearchForm
        public async Task<IActionResult> ShowSearchForm()
        {
            return View();
        }

        // POST: Jokes/ShowSearchResults
        public async Task<IActionResult> ShowSearchResults(String SearchPhrase)
        {
            return View("Index", await _context.Joke.Where(j => j.JokeQuestion.Contains(SearchPhrase)).ToListAsync());
        }

        // GET: Jokes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var joke = await _context.Joke
                .FirstOrDefaultAsync(m => m.ID == id);
            if (joke == null)
            {
                return NotFound();
            }

            return View(joke);
        }

        // GET: Jokes/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Jokes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,JokeQuestion,JokeAnswer")] Joke joke, IFormFile JokeImagePath)
        {
            if (ModelState.IsValid)
            {
                joke.JokeOwner = User.Identity.Name;

                if (JokeImagePath != null && JokeImagePath.Length > 0)
                {
                    // Generate a unique file name (e.g., using a GUID).
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + JokeImagePath.FileName;

                    // Set the file path in the Joke model.
                    joke.JokeImagePath = uniqueFileName;

                    // Specify the directory where you want to save the file.
                    var imageFolder = Path.Combine(_environment.WebRootPath, "images");

                    // Combine the directory and file name to get the full path.
                    var filePath = Path.Combine(imageFolder, uniqueFileName);

                    // Save the file to the server.
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await JokeImagePath.CopyToAsync(stream);
                    }
                }
                _context.Add(joke);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(joke);
        }

        // GET: Jokes/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var joke = await _context.Joke.FindAsync(id);
            if (joke == null)
            {
                return NotFound();
            }
            return View(joke);
        }


        // POST: Jokes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,JokeQuestion,JokeAnswer,JokeImagePath")] Joke joke, IFormFile JokeImagePath)
        {
            if (id != joke.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var originalJoke = await _context.Joke.FindAsync(id);

                originalJoke.JokeQuestion = joke.JokeQuestion;
                originalJoke.JokeAnswer = joke.JokeAnswer;

                // Check if a new image has been uploaded
                if (JokeImagePath != null && JokeImagePath.Length > 0)
                {
                    // Specify the directory where you want to save the file.
                    var imageFolder = Path.Combine(_environment.WebRootPath, "images");

                    // Delete the old image if it exists
                    if (!string.IsNullOrEmpty(originalJoke.JokeImagePath))
                    {
                        var oldImagePath = Path.Combine(imageFolder, originalJoke.JokeImagePath);
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    // Generate a unique file name for the new image (e.g., using a GUID).
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + JokeImagePath.FileName;

                    // Set the file path in the Joke model.
                    originalJoke.JokeImagePath = uniqueFileName;

                    // Combine the directory and file name to get the full path.
                    var filePath = Path.Combine(imageFolder, uniqueFileName);

                    // Save the new file to the server.
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await JokeImagePath.CopyToAsync(stream);
                    }
                }

                try
                {
                    _context.Update(originalJoke);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JokeExists(originalJoke.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(joke);
        }

        // GET: Jokes/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var joke = await _context.Joke
                .FirstOrDefaultAsync(m => m.ID == id);
            if (joke == null)
            {
                return NotFound();
            }

            return View(joke);
        }

        // POST: Jokes/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var joke = await _context.Joke.FindAsync(id);
            if (!string.IsNullOrEmpty(joke.JokeImagePath))
            {
                // Construct the full file path based on your project's structure.
                var filePath = Path.Combine(_environment.WebRootPath, "images", joke.JokeImagePath);

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
            _context.Joke.Remove(joke);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JokeExists(int id)
        {
            return _context.Joke.Any(e => e.ID == id);
        }
    }
}
