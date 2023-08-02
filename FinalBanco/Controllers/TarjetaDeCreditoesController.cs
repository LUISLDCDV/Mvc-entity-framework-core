using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FinalBanco.Data;
using FinalBanco.Models;
using FinalBanco.Controllers;


namespace FinalBanco.Controllers
{
    public class TarjetaDeCreditoesController : Controller
    {
        private readonly MyContext _context;
      


        public TarjetaDeCreditoesController(MyContext context)
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





        // GET: TarjetaDeCreditoes
        public async Task<IActionResult> Index()
        {
       
            if (HttpContext.Session.GetString("UserName") == null)
            {
                return RedirectToAction("Index", "Acceso");
            }
            else
            {
                @ViewBag.ETAR = TempData["ErrorTar"];
                @ViewBag.OKTAR = TempData["oktar"];

                if (HttpContext.Session.GetInt32("UserAdm") == 1)
                {
                    var myContext = _context.tarjetas.Include(t => t._titular);
                    return View(await myContext.ToListAsync());

                }
                else
                {
                    var userName = HttpContext.Session.GetString("UserName");
                    ViewBag.User = userName;
                    return RedirectToAction("DetalleTarjetaCliente", "TarjetaDeCreditoes");
                }
            }
        }

        public IActionResult DetalleTarjetaCliente()
        {
            @ViewBag.ETAR = TempData["ErrorTar"];
            @ViewBag.OKTAR = TempData["oktar"];
            var id = HttpContext.Session.GetInt32("UserId");

            Usuario UsuarioTarjeta = _context.usuarios.Where(m => m._dni == id).FirstOrDefault();

            var IdTarjeta = UsuarioTarjeta._tarjetas.ToList();

            return View(IdTarjeta);
        }




        // GET: TarjetaDeCreditoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.tarjetas == null)
            {
                return NotFound();
            }

            var tarjetaDeCredito = await _context.tarjetas
                .Include(t => t._titular)
                .FirstOrDefaultAsync(m => m._id_tarjeta == id);
            if (tarjetaDeCredito == null)
            {
                return NotFound();
            }

            return View(tarjetaDeCredito);
        }




        // GET: TarjetaDeCreditoes/Create
        public IActionResult Create()
        {
            ViewData["_id_usuario"] = new SelectList(_context.usuarios, "_id_usuario", "_id_usuario");
            return View();
        }






        // POST: TarjetaDeCreditoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("_id_tarjeta,_id_usuario,_numero,_codigoV,_limite,_consumos")] TarjetaDeCredito tarjetaDeCredito)
        {      
            _context.tarjetas.Add(tarjetaDeCredito);
            await _context.SaveChangesAsync();
            TempData["oktar"] = "Tarjeta de Credito creada";
            return RedirectToAction("Index", "TarjetaDeCreditoes");
        }




        // GET: TarjetaDeCreditoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.tarjetas == null)
            {
                return NotFound();
            }

            var tarjetaDeCredito = await _context.tarjetas.FindAsync(id);
            if (tarjetaDeCredito == null)
            {
                return NotFound();
            }
            ViewData["_id_usuario"] = new SelectList(_context.usuarios, "_id_usuario", "_id_usuario", tarjetaDeCredito._id_usuario);
            return View(tarjetaDeCredito);
        }






        // POST: TarjetaDeCreditoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("_id_tarjeta,_id_usuario,_numero,_codigoV,_limite,_consumos")] TarjetaDeCredito tarjetaDeCredito)
        {
            if (id != tarjetaDeCredito._id_tarjeta)
            {
                return NotFound();
            }

            TarjetaDeCredito ta = _context.tarjetas.Where(u => u._id_tarjeta == tarjetaDeCredito._id_tarjeta).FirstOrDefault();
            ta._limite = tarjetaDeCredito._limite;
            ta._consumos = tarjetaDeCredito._consumos;
            ta._numero = tarjetaDeCredito._numero;
            ta._codigoV = tarjetaDeCredito._codigoV;

            _context.tarjetas.Update(ta);
            _context.SaveChanges();


            TempData["oktar"] = "Tarjeta editada";
            return RedirectToAction("Index", "TarjetaDeCreditoes");
        }







        // GET: TarjetaDeCreditoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.tarjetas == null)
            {
                return NotFound();
            }

            var tarjetaDeCredito = await _context.tarjetas
                .Include(t => t._titular)
                .FirstOrDefaultAsync(m => m._id_tarjeta == id);
            if (tarjetaDeCredito == null)
            {
                return NotFound();
            }

            return View(tarjetaDeCredito);
        }





        // POST: TarjetaDeCreditoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.tarjetas == null)
            {
                return Problem("Entity set 'MyContext.tarjetas'  is null.");
            }
            var tarjetaDeCredito = await _context.tarjetas.FindAsync(id);
            if (tarjetaDeCredito != null)
            {
                _context.tarjetas.Remove(tarjetaDeCredito);
            }
            TempData["oktar"] = "Tarjeta eliminada";
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TarjetaDeCreditoExists(int id)
        {
            return _context.tarjetas.Any(e => e._id_tarjeta == id);
        }





        public List<TarjetaDeCredito> MostrarTarjetasDeCredito()
        {
            var id = HttpContext.Session.GetInt32("UserId");
            
            //var IdUsuario = _context.usuarios.Where(m => m._dni == id).FirstOrDefault();


            Usuario? us = _context.usuarios.Where(u => u._dni == id ).FirstOrDefault();
            
              
            
            List<TarjetaDeCredito> lista = new List<TarjetaDeCredito>();
            if (us != null)

            {
                if (us._esUsuarioAdmin == false)
                {
                    lista = us._tarjetas.ToList();
                }
                else
                {
                    lista = _context.tarjetas.ToList();
                }

            }

            return lista;
        }

        [Route("TarjetaDeCreditoes/PagarConsumo/{_id_tarjeta}")]
        public IActionResult PagarConsumo(int _id_tarjeta)
        {
            HttpContext.Session.SetInt32("idtarjeta", _id_tarjeta);
            var id = HttpContext.Session.GetInt32("UserId");
            Usuario? us = _context.usuarios.Where(m => m._dni == id).FirstOrDefault();
            List<UsuarioCajaDeAhorro> IdCU = _context.UsuarioCajaDeAhorro.Where(m => m._id_usuario == us._id_usuario).ToList();
            List<CajaDeAhorro> IdCajas = new List<CajaDeAhorro>();

            foreach (UsuarioCajaDeAhorro u in IdCU)
            {
                IdCajas.Add(_context.cajas.Where(m => m._id_caja == u._id_caja).FirstOrDefault());
            }

            ViewBag.ETAR = TempData["ErrorTar"];
            return View(IdCajas);
        }


        [Route("TarjetaDeCreditoes/PagarConsumoTarjeta/{cbu}")]
        public IActionResult PagarConsumoTarjeta(string cbu)
        {
        var id = HttpContext.Session.GetInt32("UserId");
        var usuarioLogueado = _context.usuarios.Where(m => m._dni == id).FirstOrDefault();
        var idt = HttpContext.Session.GetInt32("idtarjeta");

            try
            {
                CajaDeAhorro caja = usuarioLogueado.cajas.Where(m => m._cbu == cbu).FirstOrDefault();
                TarjetaDeCredito tc = usuarioLogueado._tarjetas.Where(m => m._id_tarjeta == idt).FirstOrDefault();

                if (tc._consumos == 0) {
                    TempData["ErrorTar"] = "Sin Consumo";
                    return RedirectToAction("DetalleTarjetaCliente", "TarjetaDeCreditoes");

                }


                    if (tc._consumos <= caja._saldo)
            {
                double consumosAux = tc._consumos;
                caja._saldo = caja._saldo - consumosAux;
                tc._consumos = 0;

                Movimiento mov = new Movimiento(caja._id_caja, "Consumos de Tarjeta", consumosAux,DateTime.Now);
                _context.movimientos.Add(mov);
                _context.usuarios.Update(usuarioLogueado);
                _context.SaveChanges();
                TempData["oktar"] = "Consumo pagado";
                return RedirectToAction("DetalleTarjetaCliente", "TarjetaDeCreditoes");

            }
            else {
                TempData["ErrorTar"] = "Saldo insuficiente";
                return RedirectToAction("DetalleTarjetaCliente", "TarjetaDeCreditoes");
            }

        }
        catch (Exception ex)
        {
            TempData["ErrorTar"] = ex.Message;
            return RedirectToAction("Index", "TarjetaDeCreditoes");
        }

            
        }


    public async Task<IActionResult> bajaTarjeta(int id)
    {

        TarjetaDeCredito? tarjeta = _context.tarjetas.Where(t => t._id_tarjeta == id).FirstOrDefault();


        if (tarjeta._consumos == 0)
        {
            _context.tarjetas.Remove(tarjeta);
            _context.SaveChanges();
            TempData["oktar"] = "Se a eliminado correctamente";
        }
        else { TempData["ErrorTar"] = "No se podido eliminar"; }
        return RedirectToAction("DetalleTarjetaCliente", "TarjetaDeCreditoes");
    }


    public IActionResult altaTarjeta()
    {
        var id = HttpContext.Session.GetInt32("UserId");
        var usuarioLogueado = _context.usuarios.Where(m => m._dni == id).FirstOrDefault();

        string idNuevaTarjeta = obtieneSecuencia(usuarioLogueado);
        TarjetaDeCredito tc = new TarjetaDeCredito(idNuevaTarjeta, 1, 500000, 0);
        agregarTarjeta(usuarioLogueado, tc);
        TempData["oktar"] = "Se a creado caja";
        return RedirectToAction("DetalleTarjetaCliente", "TarjetaDeCreditoes");


    }


    public string obtieneSecuencia(Usuario usuario)
    {

        DateTimeOffset now = (DateTimeOffset)DateTime.UtcNow;
        string fecha = now.ToString("yyyyMMddHHmmssfff");

        return usuario._id_usuario + fecha;

    }


    public bool agregarTarjeta(Usuario usuario, TarjetaDeCredito tarjeta)
    {
        try
        {
            usuario._tarjetas.Add(tarjeta);
            _context.usuarios.Update(usuario);
            _context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            TempData["ErrorTar"] = ex.Message;
            return false;
        }
    }



   
}
}
