using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FinalBanco.Data;
using FinalBanco.Models;
using System.Collections;
using Microsoft.Build.Framework;
using Microsoft.AspNetCore.Http;

namespace FinalBanco.Controllers
{
    public class CajaDeAhorroesController : Controller
    {
        private readonly MyContext _context;

        public CajaDeAhorroesController(MyContext context)
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



        //Scaffoulding Admin
        public async Task<IActionResult> Index()
        {
            @ViewBag.ETC = TempData["Errorc"];
            @ViewBag.EC_OK = TempData["okc"];
            if (HttpContext.Session.GetString("UserName") == null)
            {
                return RedirectToAction("Index", "Acceso");
            }
            else
            {

                if (HttpContext.Session.GetInt32("UserAdm") == 1)
                {
                    return View(await _context.cajas.ToListAsync());

                }
                else
                {
                    var userName = HttpContext.Session.GetString("UserName");
                    ViewBag.UserName = userName;
                    return RedirectToAction("DetalleCajaCliente", "CajaDeAhorroes");
                }

            }
        }

        public async Task<IActionResult> Details(int? id)
        {
            CajaDeAhorro? cajaDeAhorro = _context.cajas.FirstOrDefault(m => m._id_caja == id);
            
            return View(cajaDeAhorro);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("_id_caja,_cbu,_saldo")] CajaDeAhorro cajaDeAhorro)
        {
            var id = HttpContext.Session.GetInt32("UserId");

            Usuario? usuarioLogueado = _context.usuarios.Where(m => m._dni == id).FirstOrDefault();

            _context.Add(cajaDeAhorro);
            usuarioLogueado.cajas.Add(cajaDeAhorro);
            _context.usuarios.Update(usuarioLogueado);
            _context.SaveChanges();

            TempData["okc"]="Caja Credo";
            return RedirectToAction("Index", "CajaDeAhorroes");
        }
      
        public async Task<IActionResult> Edit(int? id)
        {
            CajaDeAhorro? cajaDeAhorro = await _context.cajas.FindAsync(id);
            
            return View(cajaDeAhorro);
        }

        [Route("CajaDeAhorroes/Editar")]
        public IActionResult Editar(int _id_caja, string _cbu, int _saldo)
        {
            CajaDeAhorro? caja = _context.cajas.Where(u => u._id_caja == _id_caja).FirstOrDefault();

            if (caja._id_caja == null)
            {
                return RedirectToAction("Index", "CajaDeAhorroes");
            }
            else
            {
                caja._cbu = _cbu;
                caja._saldo = _saldo;
                

                _context.cajas.Update(caja);
                _context.SaveChanges();
                TempData["okc"] = "Caja editada ok";
                return RedirectToAction("Index", "CajaDeAhorroes");
            }
        }








        [Route("CajaDeAhorroes/EliminarTitular/{_id_usuario}")]
        public IActionResult EliminarTitular(int _id_usuario)
        {
            Usuario? usuario = _context.usuarios.Where(u => u._id_usuario == _id_usuario).FirstOrDefault();

            int idc = (int)HttpContext.Session.GetInt32("UserCajaId");
            CajaDeAhorro? ca = _context.cajas.Where(m => m._id_caja == idc).FirstOrDefault();

            UsuarioCajaDeAhorro? usCa = _context.UsuarioCajaDeAhorro.Where(m => m._id_caja == ca._id_caja && m._id_usuario == usuario._id_usuario).FirstOrDefault();


            if (ca._id_caja == null)
            {
                TempData["Errorc"] = "No se pudo realizar la operacion";
                return RedirectToAction("Index", "CajaDeAhorroes");
            }
            else
            {                
                ca._titulares.Remove(usuario);
                usuario.cajas.Remove(ca);
                _context.UsuarioCajaDeAhorro.Remove(usCa);
                _context.cajas.Update(ca);
                _context.usuarios.Update(usuario);
                _context.SaveChanges();
                TempData["okc"] = "Titular eliminado";
                return RedirectToAction("Index", "CajaDeAhorroes");
            }
        }


        [Route("CajaDeAhorroes/EditarTitulares/{_id_caja}")]
        public async Task<IActionResult> EditarTitulares(int _id_caja)
        {
            HttpContext.Session.SetInt32("UserCajaId", _id_caja);
            CajaDeAhorro? cajaDeAhorro = await _context.cajas.FindAsync(_id_caja);
            return View(cajaDeAhorro._titulares.ToList());
        }




        public async Task<IActionResult> AgregarTitular(int _dni)
        {
            Usuario? usuario = _context.usuarios.Where(u => u._dni == _dni).FirstOrDefault();

            return View(usuario);
        }


        [Route("CajaDeAhorroes/ConfirmarAgregadoTitular")]
        public IActionResult ConfirmarAgregadoTitular(int _id_usuario)
        {
            Usuario? usuario = _context.usuarios.Where(u => u._id_usuario == _id_usuario).FirstOrDefault();

            int idc = (int)HttpContext.Session.GetInt32("UserCajaId");
            CajaDeAhorro? ca = _context.cajas.Where(m => m._id_caja == idc).FirstOrDefault();



            if (ca._id_caja == null)
            {
                TempData["Errorc"] = "No se pudo realizar la operacion";
                return RedirectToAction("Index", "CajaDeAhorroes");
            }
            else
            {
                ca._titulares.Add(usuario);
                _context.cajas.Update(ca);
                _context.SaveChanges();
                TempData["okc"] = "Titular agregado";
                return RedirectToAction("Index", "CajaDeAhorroes");
            }
        }





        public async Task<IActionResult> EliminarCaja(int id)
        {
            CajaDeAhorro? caja = _context.cajas.Where(m => m._id_caja == id).FirstOrDefault();

            UsuarioCajaDeAhorro? us = _context.UsuarioCajaDeAhorro.Where(m => m._id_caja == caja._id_caja).FirstOrDefault();

            Usuario? u = caja._titulares.FirstOrDefault();

            if (caja._saldo == 0)
            {
                _context.cajas.Remove(caja);
                _context.UsuarioCajaDeAhorro.Remove(us);
                _context.usuarios.Update(u);
                _context.SaveChanges();
                TempData["okc"] = "Se a eliminado la caja correctamente";
            }
            else { TempData["Errorc"] = "No se pudo realizar la operacion, la cuenta no debe tener saldo"; }
            return RedirectToAction("Index", "CajaDeAhorroes");
        }










        //Vistas Usuarios

        [HttpGet]
        [Route("DetalleCajaCliente")]
        public async Task<IActionResult> DetalleCajaCliente()
        {
      
            List<CajaDeAhorro> LCajas = ListarCajasdeCliente();
            @ViewBag.EC_OK = TempData["okc"];
            @ViewBag.ETC = TempData["Errorc"];
            return View(LCajas);
        }


        public List<CajaDeAhorro> ListarCajasdeCliente()
        {
            var id = HttpContext.Session.GetInt32("UserId");


            Usuario usuarioCopia = _context.usuarios.Where(m => m._dni == id).FirstOrDefault();

           
            List<CajaDeAhorro> LCajas = new List<CajaDeAhorro>();


            LCajas.AddRange(usuarioCopia.cajas.ToList());

           
            return LCajas;
        }


        public CajaDeAhorro CajadeCliente()
        {
            int idc = (int)HttpContext.Session.GetInt32("cajaIdc");
            CajaDeAhorro? ca = _context.cajas.Where(m => m._id_caja == idc).FirstOrDefault();


            return ca;
        }






        //Operaciones de Caja

        public IActionResult RealizarTransaccion(double _monto= 0, int _operacion = 0, string _CbuAt = null)
        {
                CajaDeAhorro? ca = CajadeCliente();
                var cbu = ca._cbu;

                if ( _monto <= 0)
                {
                    TempData["Errorc"] = "asegúrese de ingresar un monto mayor a cero.";
                    return RedirectToAction("Transacciones", "CajaDeAhorroes");
                }
               
                    switch (_operacion)
                    {
                        case 0:
                            TempData["Errorc"] = "No se pudo realizar la operacion debe ingresar operacion.";
                            //return View();
                            return RedirectToAction("Transacciones", "CajaDeAhorroes");
                        case 1:
                            retirarSaldo(_monto);                            
                            return RedirectToAction("Index", "CajaDeAhorroes");
                        case 2:                            
                            depositarSaldo(_monto);
                            return RedirectToAction("Index", "CajaDeAhorroes");
                        case 3:
                            AltaTransferencia(cbu, _CbuAt, _monto);                            
                            return RedirectToAction("Index", "CajaDeAhorroes");
                        default:
                            TempData["Errorc"] = "No se pudo realizar la operacion debe ingresar operacion.";
                            //return View();
                            return RedirectToAction("Transacciones", "CajaDeAhorroes");
                    }
            
        }

        public IActionResult retirarSaldo(double monto)
        {
                CajaDeAhorro? ca = CajadeCliente();

                try
                {
                if (afectarSaldoCA(ca._cbu, monto))
                {                    
                        Movimiento mov = new Movimiento(ca._id_caja, "Retiro en cuenta", monto, DateTime.Now);
                        _context.movimientos.Add(mov);
                        ca._movimientos.Add(mov);
                        TempData["okc"] = "Saldo Extraido";
                        return RedirectToAction("Index", "CajaDeAhorroes");
                }
                else { return RedirectToAction("Transacciones", "CajaDeAhorroes"); }

            }
            catch (Exception ex)
            {
                TempData["Errorc"] = ex.Message;
                TempData["Errorc"] = "No se pudo realizar la operacion";
                return RedirectToAction("Transacciones", "CajaDeAhorroes");
            }
        }

        public IActionResult depositarSaldo(double monto)
        {
            try
            {
                aumentarSaldoCA(monto);
                TempData["okc"] = "Saldo Depositado";
                return RedirectToAction("Index", "CajaDeAhorroes");
            }
            catch (Exception ex)
            {
                TempData["Errorc"] = ex.Message;
                TempData["Errorc"] = "No se pudo realizar la operacion";
                return RedirectToAction("Index", "CajaDeAhorroes");
            }
        }

        public bool AltaTransferencia(string cbuOrigen, string cbuDestino, double monto)
        {

            CajaDeAhorro ca = _context.cajas.Where(m => m._cbu == cbuOrigen).FirstOrDefault();

            try
            {
                if (ca._saldo >= monto && monto>0)
                {
                    ca._saldo = ca._saldo - monto;
                    Movimiento mov = new Movimiento(ca._id_caja, "Retiro Transferencia", monto, DateTime.Now);
                    _context.movimientos.Add(mov);
                    ca._movimientos.Add(mov);

                   }
            }
            catch (Exception ex)
            {
                TempData["Errorc"] = ex.Message;
                TempData["Errorc"] = "No se pudo realizar la operacion";
                return false;
            }

            CajaDeAhorro co = _context.cajas.Where(m => m._cbu == cbuDestino).FirstOrDefault();

            try
            {
                co._saldo = co._saldo + monto;

                Movimiento mov = new Movimiento(co._id_caja, "Depósito Transferencia", monto, DateTime.Now);
                _context.movimientos.Add(mov);
                co._movimientos.Add(mov);

                _context.cajas.Update(ca);
                _context.cajas.Update(co);
                _context.SaveChanges();
                TempData["ok"] = "Transferencia Realizada";
                return true;

            }
            catch (Exception ex)
            {
                TempData["Errorc"] = ex.Message;
                return false;
            }


        }

        //+
        public bool aumentarSaldoCA(double monto)
        {
            CajaDeAhorro? ca = CajadeCliente();

            try
            {
                ca._saldo = ca._saldo + monto;


                Movimiento mov = new Movimiento(ca._id_caja, "Deposito en cuenta", monto, DateTime.Now);

                    _context.movimientos.Add(mov);
                ca._movimientos.Add(mov);
                
                _context.cajas.Update(ca);
                _context.SaveChanges();
                

                return true;
            }
            catch
            {
                TempData["Errorc"] = "No se pudo realizar la operacion";
                return false;
            }
        }
        //-
        public bool afectarSaldoCA(string cbu, double monto)
        {
            CajaDeAhorro ca = null;

            CajaDeAhorro? caja = _context.cajas.Where(c => c._cbu.Equals(cbu) && c._saldo >= monto).FirstOrDefault();
            if (caja != null) { ca = caja; }
            else { return false; }

            try
            {
                ca._saldo = ca._saldo - monto;
                _context.cajas.Update(ca);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                TempData["Errorc"] = "No se pudo realizar la operacion";
                return false;
            }

        }



        //Ver
        public IActionResult Transacciones(int id)
        {
            if (id == 0) {id = (int)HttpContext.Session.GetInt32("cajaIdc");}
            else {HttpContext.Session.SetInt32("cajaIdc", id);}
            
            ViewBag.ET = TempData["Errorc"];
            CajaDeAhorro ? caja = _context.cajas.Where(c => c._id_caja == id).FirstOrDefault();            
            return View(caja);
        }





        public IActionResult crearCajaAhorro()
        {
            string cbu = obtieneSecuenciaCA();

            try
            {
                agregarCA(cbu);
                TempData["okc"] = "Caja Creda";
            }
            catch (Exception ex)
            {
                TempData["Errorc"] = ex.Message;
                TempData["Errorc"] = "No se pudo realizar la operacion";
            }
            return RedirectToAction("Index", "CajaDeAhorroes");
        }

        public string obtieneSecuenciaCA()
        {
            var id = HttpContext.Session.GetInt32("UserId");
            var usuarioLogueado = _context.usuarios.Where(m => m._dni == id).FirstOrDefault();
            //Genera secuencia unica de CBU o Tarjeta
            //El usuario se pasa porque el Admin podria crear TJ o CBU
            DateTimeOffset now = (DateTimeOffset)DateTime.UtcNow;
            string fecha = now.ToString("yyyyMMddHHmmssfff");

            return usuarioLogueado._id_usuario + fecha;
        }
        
        public bool agregarCA( string cbu)
        {
            var id = HttpContext.Session.GetInt32("UserId");
            Usuario? usuarioLogueado = _context.usuarios.Where(m => m._dni == id).FirstOrDefault();
                 

            try
            {
                CajaDeAhorro nuevo = new CajaDeAhorro(cbu, 0);
                usuarioLogueado.cajas.Add(nuevo);
                _context.usuarios.Update(usuarioLogueado);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {               
                TempData["Errorc"] = "No se pudo realizar la operacion";
                return false;
            }
        }




    }
}
