﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Statii_Incarcare.Models.Db;

namespace Statii_Incarcare.Controllers
{
    public class StatiiController : Controller
    {
        private readonly StatiiIncarcareContext _context;

        public StatiiController(StatiiIncarcareContext context)
        {
            _context = context;
        }

        // GET: Statii
        public async Task<IActionResult> Index()
        {
            return _context.Statiis != null ?
                        View(await _context.Statiis.ToListAsync()) :
                        Problem("Entity set 'StatiiIncarcareContext.Statiis'  is null.");
        }

        // GET: Statii/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Statiis == null)
            {
                return NotFound();
            }

            var statii = await _context.Statiis
                .FirstOrDefaultAsync(m => m.StatieId == id);
            if (statii == null)
            {
                return NotFound();
            }

            return View(statii);
        }

        // GET: Statii/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Statii/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StatieId,Nume,Oras,Adresa")] Statii statii)
        {
            if (ModelState.IsValid)
            {
                _context.Add(statii);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(statii);
        }

        // GET: Statii/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Statiis == null)
            {
                return NotFound();
            }

            var statii = await _context.Statiis.FindAsync(id);
            if (statii == null)
            {
                return NotFound();
            }
            return View(statii);
        }

        // POST: Statii/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("StatieId,Nume,Oras,Adresa")] Statii statii)
        {
            if (id != statii.StatieId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(statii);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StatiiExists(statii.StatieId))
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
            return View(statii);
        }

        // GET: Statii/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Statiis == null)
            {
                return NotFound();
            }

            var statii = await _context.Statiis
                .FirstOrDefaultAsync(m => m.StatieId == id);
            if (statii == null)
            {
                return NotFound();
            }

            return View(statii);
        }

        // POST: Statii/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Statiis == null)
            {
                return Problem("Entity set 'StatiiIncarcareContext.Statiis'  is null.");
            }
            var statii = await _context.Statiis.FindAsync(id);
            if (statii != null)
            {
                _context.Statiis.Remove(statii);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StatiiExists(int id)
        {
            return (_context.Statiis?.Any(e => e.StatieId == id)).GetValueOrDefault();
        }
    }
}