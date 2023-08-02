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
    public class PagoesController : Controller
    {
        private readonly MyContext _context;
        


        public PagoesController(MyContext context)
        {
            _context = context;


            _context.usuarios.Include(u => u.cajas)
                               .Include(u => u._plazosFijos)
                               .Include(u => u._tarjetas)
                               .Include(u => u._pagos).Load();

            _context.cajas.Include(u => u._movimientos)
                                .Include(u => u._titulares).Load();

            //cargo los objetos de banco            
            _context.movimientos.Load();
            _context.tarjetas.Load();
            _context.plazosFijos.Load();
            _context.pagos.Load();

        }

        // GET: Pagoes
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("UserName") == null)
            {
                return RedirectToAction("Index", "Acceso");
            }
            else
            {

                if (HttpContext.Session.GetInt32("UserAdm") == 1)
                {
                    @ViewBag.EP = TempData["ErrorP"];
                    @ViewBag.EP_OK = TempData["okp"];
                    var myContext = _context.pagos.Include(p => p._usuario);
                    return View(await myContext.ToListAsync());

                }
                else
                {
                    
                    var userName = HttpContext.Session.GetString("UserName");
                    ViewBag.User = userName;
                    return RedirectToAction("DetallePagosCliente", "Pagoes");
                }
            }
        }


        public IActionResult DetallePagosCliente()
        {
            @ViewBag.EP = TempData["ErrorP"];
            @ViewBag.EP_OK = TempData["okp"];
            var id = HttpContext.Session.GetInt32("UserId");

            var IdUsuarioTarjeta = _context.usuarios.Where(m => m._dni == id).FirstOrDefault();
            var IdPagos = _context.pagos.Where(m => m._id_usuario == IdUsuarioTarjeta._id_usuario).ToList();


            return View(IdPagos);
        }





       
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.pagos == null)
            {
                return NotFound();
            }

            var pago = await _context.pagos
                .Include(p => p._usuario)
                .FirstOrDefaultAsync(m => m._id_pago == id);
            if (pago == null)
            {
                return NotFound();
            }

            return View(pago);
        }



        
        public IActionResult Create()
        {
            ViewData["_id_usuario"] = new SelectList(_context.usuarios, "_id_usuario", "_id_usuario");
            return View();
        }

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("_id_pago,_id_usuario,_monto,_pagado,_metodo,_detalle,_id_metodo")] Pago pago)
        {
            
            _context.pagos.Add(pago);
            await _context.SaveChangesAsync();
            TempData["okp"] = "Pago Creado";
            return RedirectToAction("Index", "Pagoes");
        }

        



        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.pagos == null)
            {
                return NotFound();
            }

            var pago = await _context.pagos.FindAsync(id);
            if (pago == null)
            {
                return NotFound();
            }
            ViewData["_id_usuario"] = new SelectList(_context.usuarios, "_id_usuario", "_id_usuario", pago._id_usuario);
            return View(pago);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("_id_pago,_id_usuario,_monto,_pagado,_metodo,_detalle,_id_metodo")] Pago pago)
        {
            if (id != pago._id_pago)
            {
                return NotFound();
            }

            Pago? pa = _context.pagos.Where(u => u._id_pago == pago._id_pago).FirstOrDefault();
            
            pa._pagado = pago._pagado;
            pa._metodo = pago._metodo;
            pa._detalle = pago._detalle;
            pa._monto = pago._monto;
            pa._id_metodo = pago._id_metodo;

            _context.pagos.Update(pa);
            _context.SaveChangesAsync();


            TempData["okp"] = "Pago Editado";
            return RedirectToAction("Index", "Pagoes");
        }

        


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.pagos == null)
            {
                return NotFound();
            }

            var pago = await _context.pagos
                .Include(p => p._usuario)
                .FirstOrDefaultAsync(m => m._id_pago == id);
            if (pago == null)
            {
                return NotFound();
            }

            return View(pago);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.pagos == null)
            {
                return Problem("Entity set 'MyContext.pagos'  is null.");
            }
            var pago = await _context.pagos.FindAsync(id);
            if (pago != null)
            {
                _context.pagos.Remove(pago);
            }
            TempData["okp"] = "Pago Eliminado";
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        


        public bool confirmarAltaPago(double monto, string metodo, string numero= "0")
        {
            //var num = (string)(numero);

            TarjetaDeCredito tarjeta = _context.tarjetas.Where(m => m._numero == numero).FirstOrDefault();

            CajaDeAhorro caja = _context.cajas.Where(m => m._cbu == numero).FirstOrDefault();
            

            if (metodo.Equals("TC"))
            {                
                if (tarjeta != null)
                {
                    tarjeta._consumos = tarjeta._consumos + monto;
                    _context.tarjetas.Update(tarjeta);
                    _context.SaveChanges();

                    return true;
                }
                else { return false; }


            }
            else
            {              
                if (caja != null)
                {
                    if (caja._saldo >= monto)
                    {
                        caja._saldo = caja._saldo - monto;
                        Movimiento mov = new Movimiento(caja._id_caja, "Pago por caja", monto,DateTime.Now);
                        _context.movimientos.Add(mov);
                        caja._movimientos.Add(mov);
                        _context.cajas.Update(caja);
                        _context.SaveChanges();
                        return true;
                    }
                    else { return false; }

                }
                else { return false; }
            }
        }

        public bool confirmarEstadoPago(int id)
        {
            Pago pago = null;
            foreach (Pago p in _context.pagos)
            {
                if (p._id_pago == id)
                {
                    pago = p;
                }
            }

            if (pago != null)
            {
                pago._pagado = true;
                _context.pagos.Update(pago);
                _context.SaveChanges();
                return true;
            }
            else { return false; }

        }


        public Pago buscarPago(int id)
        {

            foreach (Pago p in _context.pagos)
            {
                if (p._id_pago == id)
                {
                    return p;
                }
            }
            return null;

        }


        public IActionResult pagoFinalizar(int id)
        {
            Pago pago = _context.pagos.Where(m => m._id_pago == id).FirstOrDefault();
            pago._pagado = true;
            _context.pagos.Update(pago);
            _context.SaveChanges();
            TempData["okp"] = "Pago Finalizado";
            return RedirectToAction("DetallePagosCliente", "Pagoes");
        }






        public IActionResult crearPAGO()
        {
            @ViewBag.EP = TempData["ErrorP"];
            return View();
        }


        public IActionResult definirMEDIOPAGO(double monto, string metodo, string detalle)
        {
            if (monto == 0 || metodo== "0" || detalle==null) { 
                TempData["ErrorP"] = "Asegúrese de ingresar bien los campos.";
                return RedirectToAction("crearPAGO", "Pagoes");
            }else{
            
            var _monto = monto.ToString();
            HttpContext.Session.SetString("detalle", detalle);
            HttpContext.Session.SetString("monto", _monto);

                switch (metodo)
                {
                    case "0":
                        return RedirectToAction("Index", "Acceso");
                    case "1":
                        return RedirectToAction("pagoPorCaja", "Pagoes");
                    case "2":
                        return RedirectToAction("pagoTarjeta", "Pagoes");
                    default:
                        return RedirectToAction("Index", "Acceso");
                }
            }
        }


        //listado de cajas
        public IActionResult pagoPorCaja()
        {
            var id = HttpContext.Session.GetInt32("UserId");
            Usuario? us = _context.usuarios.Where(m => m._dni == id).FirstOrDefault();
            List<UsuarioCajaDeAhorro> IdCU = _context.UsuarioCajaDeAhorro.Where(m => m._id_usuario == us._id_usuario).ToList();
            List<CajaDeAhorro> IdCajas = new List<CajaDeAhorro>();

            foreach (UsuarioCajaDeAhorro u in IdCU)
            {
                IdCajas.Add(_context.cajas.Where(m => m._id_caja == u._id_caja).FirstOrDefault());
            }

            ViewBag.EP = TempData["ErrorP"];
            return View(IdCajas);
        }


        //listado de tarjetas
        public IActionResult pagoTarjeta()
        {

            var id = HttpContext.Session.GetInt32("UserId");

            var IdUsuarioTarjeta = _context.usuarios.Where(m => m._dni == id).FirstOrDefault();
            List<TarjetaDeCredito> IdTarjeta = _context.tarjetas.Where(m => m._id_usuario == IdUsuarioTarjeta._id_usuario).ToList();


            ViewBag.ErrorPta = TempData["ErrorP"];

            return View(IdTarjeta);
        }


        //pago por caja
        [Route("Pagoes/AltaPagoCaja/{_id_caja}")]
        public IActionResult AltaPagoCaja(int _id_caja)
        {
            var id = HttpContext.Session.GetInt32("UserId");
            Usuario? usuarioLogueado = _context.usuarios.Where(m => m._dni == id).FirstOrDefault();

            CajaDeAhorro? caja = usuarioLogueado.cajas.Where(m => m._id_caja == _id_caja).FirstOrDefault();

            var saldoCa = caja._saldo;

            var detalle = HttpContext.Session.GetString("detalle");
            var monto1 = HttpContext.Session.GetString("monto");
            double monto = Convert.ToDouble(monto1);
            
            if (saldoCa> monto){
                caja._saldo = saldoCa - monto;

                _context.cajas.Update(caja);

                double numero = Convert.ToDouble(caja._cbu);

                Pago pago = new Pago(monto, false, "Caja", detalle, (long)numero);
                usuarioLogueado._pagos.Add(pago);

                Movimiento mov = new Movimiento(caja._id_caja, detalle, monto, DateTime.Now);
                _context.movimientos.Add(mov);
                caja._movimientos.Add(mov);

                _context.usuarios.Update(usuarioLogueado);
                _context.SaveChanges();
                TempData["okp"] = "Pago Creado por caja";
                return RedirectToAction("index", "Pagoes");

            }
            else{
                TempData["ErrorP"] = "No tiene saldo suficiente.";
                return RedirectToAction("pagoPorCaja", "Pagoes");
            }

            

        }

        //pago por tarjeta
        [Route("Pagoes/ConfirmarPago/{_id_tarjeta}")]
        public IActionResult ConfirmarPago(int _id_tarjeta)
        {

            TarjetaDeCredito? tc = _context.tarjetas.Where(m => m._id_tarjeta == _id_tarjeta).FirstOrDefault();

            var numero = tc._numero;
            var numeroT = Convert.ToDouble(numero);//definimos el numero de tarjeta
            
            var detalle = HttpContext.Session.GetString("detalle");
            
            var monto1 = HttpContext.Session.GetString("monto");
            double monto = Convert.ToDouble(monto1);
            
            double consumos = tc._consumos;
            


            var id = HttpContext.Session.GetInt32("UserId");
            var usuarioLogueado = _context.usuarios.Where(m => m._dni == id).FirstOrDefault();

            if (tc._limite >= (consumos + monto))
            {
                Pago pago = new Pago(monto, false, "Tarjeta", detalle, (long)numeroT);
                usuarioLogueado._pagos.Add(pago);

                tc._consumos = (double)(tc._consumos + monto);

                _context.usuarios.Update(usuarioLogueado);
                _context.SaveChanges();


                TempData["okp"] = "Pago Creado por tarjeta";
                return RedirectToAction("index", "Pagoes");
            }
            else
            {
                TempData["ErrorP"] = "No tiene limite en la tearjeta.";
                return RedirectToAction("pagoTarjeta", "Pagoes");
            }

        }






        public List<Pago> MostrarPagos(bool Espagado)
        {
            var id = HttpContext.Session.GetInt32("UserId");
            var usuarioLogueado = _context.usuarios.Where(m => m._dni == id).FirstOrDefault();


            List<Pago> TotalPagosFiltroEstado = new List<Pago>();
            TotalPagosFiltroEstado = _context.pagos.Where(p => p._pagado == Espagado).ToList();

            List<Pago> TotalPagos = new List<Pago>();
            TotalPagos = _context.pagos.Where(p => p._pagado != Espagado).ToList();

            List<Pago> lista = new List<Pago>();
            if (TotalPagosFiltroEstado != null)

            {
                if (usuarioLogueado._esUsuarioAdmin == false)
                {
                    List<Pago> PagosDelUsuarioFiltroEstado = new List<Pago>();
                    PagosDelUsuarioFiltroEstado = TotalPagosFiltroEstado.Where(p => p._id_usuario == usuarioLogueado._id_usuario).ToList();
                    lista = PagosDelUsuarioFiltroEstado;
                }
                else
                {
                    lista = TotalPagosFiltroEstado;
                }

            }

            return lista;
        }

        public bool ModificarPago(int id)
        {
            //(double monto, string metodo, long numero)
            Pago pago = buscarPago(id);
            try
            {
                if (confirmarAltaPago(pago._monto, pago._metodo ))
                {// confirmarAltaPago(double monto, string metodo, string numero)
                    return confirmarEstadoPago(id);
                }
                else { return false; }
            }
            catch { return false; }
        }

        public IActionResult eliminarPago(int id)
        {
            Pago pago = _context.pagos.Where(m => m._id_pago == id).FirstOrDefault();

            _context.pagos.Remove(pago);
            _context.SaveChanges();
            TempData["okp"] = "Pago eliminado";
            return RedirectToAction("DetallePagosCliente", "Pagoes");
        }






        private bool PagoExists(int id)
        {
            return _context.pagos.Any(e => e._id_pago == id);
        }
    }
}
