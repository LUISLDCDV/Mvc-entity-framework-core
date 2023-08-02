using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FinalBanco.Data;
using FinalBanco.Models;

namespace FinalBanco.Controllers
{
    public class MovimientoesController : Controller
    {
        private readonly MyContext _context;

        public MovimientoesController(MyContext context)
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




        // GET: Movimientoes
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
                    return View(await _context.movimientos.ToListAsync());

                }
                else
                {
                    var userName = HttpContext.Session.GetString("UserName");
                    ViewBag.User = userName;
                    return RedirectToAction("DetalleMovimientosCliente", "Movimientoes");
                }
            }

        }




        public IActionResult DetalleMovimientosCliente()
        {

            var id = HttpContext.Session.GetInt32("UserId");

            var usuarioCopia = _context.usuarios.Where(m => m._dni == id).FirstOrDefault();

            var IdUC = _context.UsuarioCajaDeAhorro.Where(m => m._id_usuario == usuarioCopia._id_usuario).ToList();
            
            List<Movimiento> movUs = new List<Movimiento>();

            foreach (CajaDeAhorro mc in usuarioCopia.cajas)
            {
                movUs.AddRange(_context.movimientos.Where(m => m._id_CajaDeAhorro == mc._id_caja).ToList());
                
            }
            
            return View(movUs);
        }




        // GET: Movimientoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.movimientos == null)
            {
                return NotFound();
            }

            var movimiento = await _context.movimientos
                .Include(m => m._cajaDeAhorro)
                .FirstOrDefaultAsync(m => m._id_Movimiento == id);
            if (movimiento == null)
            {
                return NotFound();
            }

            return View(movimiento);
        }


        public IActionResult BuscarMovimiento()
        {
            return View();
        }




        public IActionResult VerMovimiento(string detalle, DateTime fecha, float monto)
        {
            List<Movimiento> move = new List<Movimiento>();
            
            move.AddRange(movimientosDeUsuario());

            List<Movimiento> movB = new List<Movimiento>();

            foreach (Movimiento mc in move)
            {
                if (mc._detalle == detalle || mc._fecha.Date == fecha || mc._monto == monto)
                {
                    movB.Add(mc);
                }
                
            }

            if (movB != null)
            {
                return View(movB);
            }
            else { return RedirectToAction("DetalleMovimientosCliente", "Movimientoses"); }

        }



        public IActionResult MostrarMovimientos()
        {
            List<Movimiento> movimientos = (List<Movimiento>)TempData["Movimientos"];

            return View(movimientos);
        }



        public bool AltaMovimiento(CajaDeAhorro caja, string detalle, double monto)
        {
            var id = HttpContext.Session.GetInt32("UserId");

            Usuario? usuarioLogueado = _context.usuarios.Where(m => m._dni == id).FirstOrDefault();

            try
            {
                Movimiento movimiento = new Movimiento(caja._id_caja, detalle, monto, DateTime.Now);

                _context.movimientos.Add(movimiento);
                caja._movimientos.Add(movimiento);

                _context.movimientos.Update(movimiento);
                _context.cajas.Update(caja);
                _context.usuarios.Update(usuarioLogueado);
                
                _context.SaveChanges();

                return true;
            }
            catch (Exception ex) { return false; }
        }

        public bool AltaMovimiento(string cbu, string detalle, double monto)
        {
            var id = HttpContext.Session.GetInt32("UserId");

            Usuario? usuarioLogueado = _context.usuarios.Where(m => m._dni == id).FirstOrDefault();

            try
            {
                CajaDeAhorro? caja = _context.cajas.Where(caja => caja._cbu == cbu).FirstOrDefault();
                Movimiento? movimiento = new Movimiento(caja._id_caja, detalle, monto, DateTime.Now);
                _context.movimientos.Add(movimiento);
                caja._movimientos.Add(movimiento);

                _context.movimientos.Update(movimiento);
                _context.cajas.Update(caja);
                _context.usuarios.Update(usuarioLogueado);
                
                _context.SaveChanges();

                return true;
            }
            catch (Exception ex) { return false; }
        }



        public List<Movimiento> movimientosDeUsuario() {

            var id = HttpContext.Session.GetInt32("UserId");

            Usuario? usuarioCopia = _context.usuarios.Where(m => m._dni == id).FirstOrDefault();

            var IdUC = _context.UsuarioCajaDeAhorro.Where(m => m._id_usuario == usuarioCopia._id_usuario).ToList();

            List<CajaDeAhorro> LCajas = new List<CajaDeAhorro>();

            List<Movimiento> movUs = new List<Movimiento>();

            foreach (UsuarioCajaDeAhorro u in IdUC)
            {
                LCajas.Add(_context.cajas.Where(m => m._id_caja == u._id_caja).FirstOrDefault());
            }

            foreach (CajaDeAhorro mc in LCajas)
            {
                movUs.AddRange(_context.movimientos.Where(m => m._id_CajaDeAhorro == mc._id_caja).ToList());
            }
            if (usuarioCopia._esUsuarioAdmin) { return _context.movimientos.ToList(); }
            else { return movUs; }

        }



       



        private bool MovimientoExists(int id)
        {
            return _context.movimientos.Any(e => e._id_Movimiento == id);
        }
    }
}
