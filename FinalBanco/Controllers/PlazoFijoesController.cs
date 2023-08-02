using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FinalBanco.Data;
using FinalBanco.Models;
using System.Threading;

namespace FinalBanco.Controllers
{
    public class PlazoFijoesController : Controller
    {
        private readonly MyContext _context;


        public PlazoFijoesController(MyContext context)
        {
            _context = context;


            _context.usuarios.Include(u => u.cajas)
                               .Include(u => u._plazosFijos)
                               .Include(u => u._tarjetas)
                               .Include(u => u._pagos).Load();

            _context.cajas.Include(u => u._movimientos).Load();

            //cargo los objetos de banco
            _context.cajas.Load();
            _context.movimientos.Load();
            _context.tarjetas.Load();
            _context.plazosFijos.Load();
            _context.pagos.Load();
        }

        // GET: PlazoFijoes
        public async Task<IActionResult> Index()
        {


            @ViewBag.PF = TempData["Error"];

            @ViewBag.PF_OK = TempData["ok"];

            if (HttpContext.Session.GetString("UserName") == null)
            {
                return RedirectToAction("Index", "Acceso");
            }
            else
            {

                if (HttpContext.Session.GetInt32("UserAdm") == 1)
                {
                    var myContext = _context.plazosFijos.Include(p => p._titular);
                    return View(await myContext.ToListAsync());

                }
                else
                {
                    var userName = HttpContext.Session.GetString("UserName");                    
                    ViewBag.User = userName;
                    return RedirectToAction("DetallePlazoFijoCliente", "PlazoFijoes");
                }
            }
        }




        public IActionResult DetallePlazoFijoCliente()
        {
            @ViewBag.PF = TempData["Error"];

            @ViewBag.PF_OK = TempData["ok"];

            var id = HttpContext.Session.GetInt32("UserId");

            Usuario? usuarioCopia = _context.usuarios.Where(m => m._dni == id).FirstOrDefault();

            var IdPLF = _context.plazosFijos.Where(m => m._id_usuario == usuarioCopia._id_usuario).ToList();


            return View(IdPLF);
        }



        // GET: PlazoFijoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.plazosFijos == null)
            {
                return NotFound();
            }

            var plazoFijo = await _context.plazosFijos
                .Include(p => p._titular)
                .FirstOrDefaultAsync(m => m._id_plazoFijo == id);
            if (plazoFijo == null)
            {
                return NotFound();
            }

            return View(plazoFijo);
        }



        // GET: PlazoFijoes/Create
        public IActionResult Create()
        {
            ViewData["_id_usuario"] = new SelectList(_context.usuarios, "_id_usuario", "_id_usuario");
            return View();
        }


        // POST: PlazoFijoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Create([Bind("_id_plazoFijo,_id_usuario,_monto,_fechaIni,_fechaFin,_tasa,_pagado,_cbu")] PlazoFijo plazoFijo)
        {

            _context.plazosFijos.Add(plazoFijo);
            await _context.SaveChangesAsync();

            TempData["ok"] = "Plazo fijo Creado";
            return RedirectToAction("Index", "PlazoFijoes");
        }




       



        





        // GET: PlazoFijoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.plazosFijos == null)
            {
                return NotFound();
            }

            var plazoFijo = await _context.plazosFijos
                .Include(p => p._titular)
                .FirstOrDefaultAsync(m => m._id_plazoFijo == id);
            if (plazoFijo == null)
            {
                return NotFound();
            }

            return View(plazoFijo);
        }





        // POST: PlazoFijoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.plazosFijos == null)
            {
                return Problem("Entity set 'MyContext.plazosFijos'  is null.");
            }
            var plazoFijo = await _context.plazosFijos.FindAsync(id);
            if (plazoFijo != null)
            {
                _context.plazosFijos.Remove(plazoFijo);
            }
            TempData["ok"] = "Plazo fijo eliminado";
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }





        private bool PlazoFijoExists(int id)
        {
            return _context.plazosFijos.Any(e => e._id_plazoFijo == id);
        }



        


        public List<CajaDeAhorro> ListarCajasdeCliente()
        {
            var id = HttpContext.Session.GetInt32("UserId");

            Usuario? usuarioCopia = _context.usuarios.Where(m => m._dni == id).FirstOrDefault();

            List<CajaDeAhorro> LCajas = new List<CajaDeAhorro>();

            LCajas = usuarioCopia.cajas.ToList();

            return LCajas;
        }


        public IActionResult crearPlazoFijo()
        {
            var LCajas = ListarCajasdeCliente();


            return View(LCajas);
        }


        public bool afectarSaldoCAenPF(CajaDeAhorro caja, double monto)
        {
       
            try
            {
                caja._saldo = caja._saldo - monto;
                _context.cajas.Update(caja);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                TempData["Error"] = "No se pudo realizar la operacion";
                return false;
            }

        }


        public IActionResult AltaPlazoFijo(int id)
    {
        var idu = HttpContext.Session.GetInt32("UserId");
        Usuario? IdUsuario = _context.usuarios.Where(m => m._dni == idu).FirstOrDefault();
         

        CajaDeAhorro? Idcaja = IdUsuario.cajas.Where(m => m._id_caja == id).FirstOrDefault();

        double cbuaux = Convert.ToDouble(Idcaja._cbu);
        //var cbuaux = Convert.ToInt32(Idcaja._cbu);
        PlazoFijo plazoFijo = new PlazoFijo(1000.0, DateTime.Now.AddMonths(1), 50, (int)cbuaux);
        try
        {

                CajaDeAhorro caja = Idcaja;

                try
                {
                    afectarSaldoCAenPF(caja, plazoFijo._monto);

                    IdUsuario._plazosFijos.Add(plazoFijo);
                    _context.plazosFijos.Add(plazoFijo);

                    Movimiento mov = new Movimiento(caja._id_caja, "Alta de plazo fijo", plazoFijo._monto, DateTime.Now);
                    _context.movimientos.Add(mov);
                    caja._movimientos.Add(mov);

                    _context.cajas.Update(caja);                    
                    _context.usuarios.Update(IdUsuario);
                    _context.SaveChanges();

                    TempData["ok"] = "Se creo el plazo fijo";
                    return RedirectToAction("DetallePlazoFijoCliente", "PlazoFijoes");
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "no se pudo dar de alta";
                    return RedirectToAction("index", "PlazoFijoes");
                }

        }
        catch (Exception ex)
        {
            TempData["Error"] = "No se pudo crear el plazo fijo";
            return RedirectToAction("index", "PlazoFijoes");
        }
    }


        public IActionResult BajaPlazoFijo(int id)
        {
            var plz = _context.plazosFijos.Where(m => m._id_plazoFijo == id).FirstOrDefault();
            var fechaAct = DateTime.Now;
            var fechaConUnMesMas = fechaAct.AddMonths(1);

            var idu = HttpContext.Session.GetInt32("UserId");
            Usuario? usuario = _context.usuarios.Where(m => m._dni == idu).FirstOrDefault();

            if (plz._pagado && plz._fechaFin < fechaConUnMesMas)
            {
                usuario._plazosFijos.Remove(plz);
                _context.plazosFijos.Remove(plz);
                _context.usuarios.Update(usuario);
                _context.SaveChanges();
                TempData["ok"] = "el plazo fijo se dio de baja";
                return RedirectToAction("Index", "PlazoFijoes");
            }
            else
            {
                TempData["Error"] = "No se pudo dar de baja el plazo fijo";
                return RedirectToAction("Index", "PlazoFijoes");
            }
           
    }


}
}