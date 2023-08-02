using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FinalBanco.Data;
using FinalBanco.Models;
using Usuario = FinalBanco.Models.Usuario;
using System.Diagnostics;
using FinalBanco.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Net.Http;
using Microsoft.AspNetCore.Components.Routing;
using System.Text;
using System.Security.Cryptography;

namespace FinalBanco.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly MyContext _context;

        public UsuariosController(MyContext context)
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
        
        
       
        //vistas index
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetInt32("UserAdm") == 1)
            {
                ViewBag.Info = TempData["info"];
                ViewBag.ErrorU = TempData["ErrorU"];
                return View(await _context.usuarios.ToListAsync());
                
            }
            else
            {
                var userName = HttpContext.Session.GetString("UserName");
                ViewBag.User = userName;
                ViewBag.Info = TempData["info"];
                ViewBag.ErrorU = TempData["ErrorU"];
                return RedirectToAction("DetalleUsuarioCliente", "Usuarios");
            }
           
        }


        public IActionResult DetalleUsuarioCliente(int? id)
        {
            if (id != null) {
                var usuarioCopia = _context.usuarios.Where(m => m._id_usuario == id).FirstOrDefault();
                return View(usuarioCopia);

            }
            else
            {
                var idaux = GetId();
                var usuarioAux = _context.usuarios.Where(m => m._dni == idaux).FirstOrDefault();
                return View(usuarioAux);
            }            
        }

        public int? GetId()
        {
            return HttpContext.Session.GetInt32("UserId");
        }        

        
        
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.usuarios == null)
            {
                return NotFound();
            }

            var usuario = await _context.usuarios
                .FirstOrDefaultAsync(m => m._id_usuario == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.usuarios == null)
            {
                return Problem("Entity set 'MyContext.usuarios'  is null.");
            }
            var usuario = await _context.usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.usuarios.Remove(usuario);
            }
            TempData["info"] = "Usuario eliminado";
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public bool eliminarUsuario(int id)
        {
            Usuario usuarioAux = null;

            foreach (Usuario u in _context.usuarios)
            {
                if (u._id_usuario == id)
                {
                    usuarioAux = u;
                }
            }

            try
            {
                _context.usuarios.Remove(usuarioAux);
                _context.SaveChanges();
                TempData["info"] = "Usuario eliminado";
                return true;
            }
            catch (Exception ex)
            {
                TempData["ErrorU"] = "Ocurrió un error eliminando el usuario";
                return false;
            }
        }

       
        
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.usuarios == null)
            {
                return NotFound();
            }

            var usuario = await _context.usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }


        [Route("Usuarios/Editar")]
        public IActionResult Editar(int _id_usuario,int _dni,string _nombre,string _apellido,string _mail,int _esUsuarioAdmin)
        {
            Usuario? usuario = _context.usuarios.Where(u => u._id_usuario == _id_usuario).FirstOrDefault();

            if (usuario._id_usuario == null)
            {
                return RedirectToAction("Index", "Usuarios");
            }
            else
            { 

                usuario._esUsuarioAdmin = (_esUsuarioAdmin != 0);
                usuario._nombre = _nombre;
                usuario._apellido = _apellido;
                usuario._mail = _mail;
                usuario._dni = _dni;


                _context.usuarios.Update(usuario);
                _context.SaveChanges();
                TempData["info"] = "Usuario editado ok";
                return RedirectToAction("Index", "Usuarios");
            }
        }

        


   



        

        public bool desbloquearUsuario(int id)
        {
            Usuario usuarioAux = null;

            foreach (Usuario u in _context.usuarios)
            {
                if (u._id_usuario == id)
                {
                    usuarioAux = u;
                }
            }

            try
            {
                usuarioAux._bloqueado = false;
                _context.usuarios.Update(usuarioAux);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                TempData["ErrorU"] = "Ocurrió un error eliminando el usuario";
                return false;
            }
        }



        [Route("Usuarios/limpiarIntentosFallidos/{_id_usuario}")]
        public IActionResult limpiarIntentosFallidos(int _id_usuario)
        {
            //var _id_usuario = HttpContext.Session.GetInt32("idaux");
            Usuario? u = _context.usuarios.Where(u => u._id_usuario == _id_usuario).FirstOrDefault();
            u._intentosFallidos = 0;
            u._bloqueado = false;
            _context.usuarios.Update(u);
            _context.SaveChanges();
            TempData["info"] = "Usuario Con intentos en cero";
            return RedirectToAction("Index", "Usuarios");
        }



        //Vistas Nuevo Usuario
        public IActionResult Registrar()
        {
            ViewBag.ErrorU = TempData["ErrorU"];
            return View();
        }

        public IActionResult Create()
        {
            ViewBag.ErrorU = TempData["ErrorU"];
            return View();
        }



        //ALta usuario Registro vista
        [HttpPost]
        public ActionResult registrarse(int _dni, string _nombre, string _apellido, string _mail, string _password, string password2)
        {
            if (_dni != 0 && _nombre != null && _apellido != null && _mail != null && _password != null && password2 != null)
            {
                if (_context.usuarios.Where(u => u._dni == _dni).FirstOrDefault() != null)
                {
                    TempData["ErrorU"] = "USUARIO YA REGISTRADO";
                    return RedirectToAction("Registrar", "Usuarios");

                }
                else
                {
                    if (_password != password2)
                    {
                        TempData["ErrorU"] = "Password erronea";
                        return RedirectToAction("Registrar", "Usuarios");
                    }
                    else
                    {
                        altaUsuario(_dni, _nombre, _apellido, _mail, _password);
                        TempData["info"] = "Usuario registrado";
                        return RedirectToAction("Index", "Acceso");
                    }
                }

            }
            else
            {
                TempData["ErrorU"] = "Campos incompletos";
                return RedirectToAction("Registrar", "Usuarios");
            }
        }


        

        //Registro manual Admin

        [HttpPost]
        public ActionResult registrarUs(int _dni, string _nombre, string _apellido, string _mail, string _password, int _esUsuarioAdmin, int segmento)
        {
            if (_dni != 0 && _nombre != null && _apellido != null && _mail != null && _password != null && _esUsuarioAdmin != null && segmento != null)
            {
                if (_context.usuarios.Where(u => u._dni == _dni).FirstOrDefault() != null)
                {
                    TempData["ErrorU"] = "USUARIO YA REGISTRADO";
                    return RedirectToAction("registrarse", "Usuarios");

                }
                else
                {
                    bool Admin = (_esUsuarioAdmin != 0);

                    agregarUsuario(_dni, _nombre, _apellido, _mail, _password, false, Admin, 0, segmento);
                    TempData["info"] = "Usuario creado";
                    return RedirectToAction("Index", "Usuarios");//bool bloqueado, bool admin, int intentosFallidos, int segmento)

                }

            }
            else
            {
                TempData["ErrorU"] = "Campos incompletos";
                return RedirectToAction("Index", "Usuarios");
            }
        }

        public bool altaUsuario(int dni, string nombre, string apellido, string mail, string password)
        {
            return agregarUsuario(dni, nombre, apellido, mail, password, false, false, 0, 0);
        }




        public bool agregarUsuario(int dni, string nombre, string apellido, string mail, string password, bool bloqueado, bool admin, int intentosFallidos, int segmento)
        {
            var passEnc = GetSHA256(password);

            try
            {
                Usuario nuevo = new Usuario(dni, nombre, apellido, mail, passEnc, bloqueado, admin, intentosFallidos);
                _context.usuarios.Add(nuevo);
                _context.SaveChanges();
                if (segmento == 1)
                {
                    string cbu = obtieneSecuencia(nuevo);
                    TempData["info"] = "Usuario creado con Caja";
                    agregarCA(nuevo, cbu);
                }
                if (segmento == 2)
                {
                    string cbu = obtieneSecuencia(nuevo);
                    agregarCA(nuevo, cbu);
                    string idNuevaTarjeta = obtieneSecuencia(nuevo);
                    TarjetaDeCredito tc = new TarjetaDeCredito(idNuevaTarjeta, 1, 500000, 0);
                    TempData["info"] = "Usuario creado con Tarjeta";
                    return agregarTarjeta(nuevo, tc);
                }
                return true;
            }
            catch (Exception)
            {
                ViewData["ErrorU"] = "Error crear usuario";
                return false;
            }
        }

        public string obtieneSecuencia(Usuario usuario)
        {
            //Genera secuencia unica de CBU o Tarjeta
            //El usuario se pasa porque el Admin podria crear TJ o CBU
            DateTimeOffset now = (DateTimeOffset)DateTime.UtcNow;
            string fecha = now.ToString("yyyyMMddHHmmssfff");

            return usuario._id_usuario + fecha;

        }

        public static string GetSHA256(string str)
        {
            SHA256 sha256 = SHA256.Create();
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] stream = null;
            StringBuilder sb = new StringBuilder();
            stream = sha256.ComputeHash(encoding.GetBytes(str));
            for (int i = 0; i < stream.Length; i++) sb.AppendFormat("{0:x2}", stream[i]);
            return sb.ToString();
        }

        public bool agregarCA(Usuario usuario, string cbu)
        {

            try
            {
                CajaDeAhorro nuevo = new CajaDeAhorro(cbu, 0);
                usuario.cajas.Add(nuevo);
                _context.usuarios.Update(usuario);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex) { ViewData["mesaje"] = ("agregarCA: " + ex.Message + ex.InnerException.Message); return false; }
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
            catch (Exception ex) { ViewData["mesaje"] = ("agregarTC: " + ex.Message + ex.InnerException.Message); return false; }

        }



    }
}
