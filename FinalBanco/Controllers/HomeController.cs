using FinalBanco.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using FinalBanco.Controllers;
using Usuario = FinalBanco.Models.Usuario;
using UsuarioCaja = FinalBanco.Models.UsuarioCajaDeAhorro;
using Caja = FinalBanco.Models.CajaDeAhorro;
using tarjeta = FinalBanco.Models.TarjetaDeCredito;
using PF = FinalBanco.Models.PlazoFijo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using FinalBanco.Data;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Reflection;
using Newtonsoft.Json.Linq;
using TP1;
using Microsoft.AspNetCore.Identity;

namespace FinalBanco.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MyContext _context;



        public HomeController(ILogger<HomeController> logger, MyContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserName") == null)
            {
                return RedirectToAction("Index", "Acceso");
            }
            else {

                var id = HttpContext.Session.GetInt32("UserId");
                if( id != null)
                {
                    var userName = HttpContext.Session.GetString("UserName");
                    ViewBag.UserName = userName;
                    return View(id);
                }
                else { 
                return RedirectToAction("Index", "Acceso");
                }
            }
        }


        public IActionResult VistaAdmin()
        {
            if (HttpContext.Session.GetString("UserName") == null)
            {
                return RedirectToAction("Index", "Acceso");
            }
            else
            {
                return View();
            }
        }




        public IActionResult Usuarios()
        {
            return View();
        }


        public IActionResult cerrar()
        {
            HttpContext.Session.Remove("UserId");
            HttpContext.Session.Remove("UserName");
            HttpContext.Session.Clear();           

            HttpContext.SignOutAsync(); // Cerrar sesión y limpiar la sesión actual

            _logger.LogInformation("Usuario cerró sesión."); // Registro del evento de cierre de sesión

            return RedirectToAction("Index", "Home"); // Redirigir al inicio u otra página segura
        }

        public IActionResult Privacy()
        {
            return View();
        }






        [ResponseCache(Duration = 30, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}