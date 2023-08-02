using Microsoft.AspNetCore.Mvc;
using FinalBanco.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FinalBanco.Data;
using System.Diagnostics;
using System.Net;
using TP1;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Usuario = FinalBanco.Models.Usuario;
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

namespace FinalBanco.Controllers
{
    public class AccesoController : Controller
    {

        private readonly MyContext _context;

        public Usuario usuarioLogueado;
        public Usuario usuarroO;


        public AccesoController(MyContext context)
        {
            _context = context;


        }



        //Vista Login
        public IActionResult index()
        {
            ViewBag.Info = TempData["info"];
            ViewBag.ErrorU = TempData["ErrorU"];
            return View();
        }        


        //Login
        [HttpPost]
        public ActionResult verificarLogueo(int _dni, string _password)
        {
            if (_password != null) { 
                var passEnc = GetSHA256(_password);
                var user = _context.usuarios.Where(u => u._dni == _dni && u._password == passEnc).FirstOrDefault();

                if (user != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user._dni.ToString()),
                        new Claim(ClaimTypes.Name, user._nombre),
                        // Agregar más claims según se necesiten
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        //IsPersistent = model.RememberMe,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30) // Tiempo de expiración de la cookie
                    };

                    HttpContext.SignInAsync(
                                     CookieAuthenticationDefaults.AuthenticationScheme,
                                     new ClaimsPrincipal(claimsIdentity),
                                     authProperties);

                    HttpContext.Session.SetInt32("UserId", user._dni);
                    HttpContext.Session.SetString("UserName", user._nombre);

                    int valorInt = user._esUsuarioAdmin ? 1 : 0;
                    HttpContext.Session.SetInt32("UserAdm", valorInt);

                    if (valorInt == 1)
                    { return RedirectToAction("VistaAdmin", "Home"); }
                    else
                    { return RedirectToAction("Index", "Home"); }
                }

                else
                {
                    var user2 = _context.usuarios.Where(u => u._dni == _dni).FirstOrDefault();
                    if (user2 != null)
                    {
                        TempData["ErrorU"] = "Contraseña erronea";
                        agregarIntentoFallido(_dni);
                        return RedirectToAction("Index", "Acceso");
                    }
                    else
                    {
                        TempData["ErrorU"] = "Usuario erroneo";
                        return RedirectToAction("Index", "Acceso");
                    }
                }

            }
            else
            {
                TempData["ErrorU"] = "Contraseña incorrecta ";
                return RedirectToAction("Index", "Acceso");
            }

        
        }

        public bool agregarIntentoFallido(int dni)
        {
            bool salida = false;
            Usuario? u = _context.usuarios.Where(u => u._dni == dni).FirstOrDefault();

            if (u != null)
            {
                u._intentosFallidos = u._intentosFallidos + 1;
                if (u._intentosFallidos >= 3)
                {
                    u._bloqueado = true;
                    TempData["Error"] = "Usuario bloqueado ";
                }
                else
                { TempData["Error"] = "Contraseña Erronea"; }
                _context.usuarios.Update(u);
                salida = true;
            }
            if (salida)
                _context.SaveChanges();
            return salida;
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
        
        
        
        
        //otro
        [ResponseCache(Duration = 30, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {

            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    
    }
}
