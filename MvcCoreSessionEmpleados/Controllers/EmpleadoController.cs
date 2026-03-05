using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MvcCoreSessionEmpleados.Extensions;
using MvcCoreSessionEmpleados.Models;
using MvcCoreSessionEmpleados.Repositories;
using System.Threading.Tasks;

namespace MvcCoreSessionEmpleados.Controllers
{
    public class EmpleadoController : Controller
    {
        RepositoryEmpleados repo;
        IMemoryCache memory;

        #region INDEX
        public IActionResult Index()
        {
            return View();
        }
        #endregion

        #region VERSION 1
        public EmpleadoController(RepositoryEmpleados repo, IMemoryCache memory)
        {
            this.repo = repo;
            this.memory = memory;
        }

        public async Task<IActionResult> SessionSalarios(int? salario)
        {
            if(salario != null)
            {
                // QUEREMOS ALMACENAR LA SUMA TOTAL DE SALARIOS
                int sumaTotal = 0;
                if(HttpContext.Session.GetString("SUMASALARIAL") != null)
                {
                    sumaTotal = HttpContext.Session.GetObject<int>("SUMASALARIAL");
                }
                sumaTotal += salario.Value;
                HttpContext.Session.SetObject("SUMASALARIAL", sumaTotal);
                ViewBag.Mensaje = "Salario almacenado";
            }
            List<Empleado> empleados = await this.repo.GetEmpleadosAsync();
            return View(empleados);
        }

        public IActionResult SumaSalarial()
        {
            return View();
        }
        #endregion

        #region VERSION 2
        public async Task<IActionResult> SessionEmpleados(int? idEmpleado)
        {
            if(idEmpleado != null)
            {
                Empleado e = await this.repo.GetEmpleadoByIdAsync(idEmpleado.Value);
                // EN SESSION TENDREMOS UN CONJUNTO DE EMPLEADOS
                List<Empleado> empleadosList;
                if(HttpContext.Session.GetObject<List<Empleado>>("EMPLEADOS") != null)
                {
                    empleadosList = HttpContext.Session.GetObject<List<Empleado>>("EMPLEADOS");
                }
                else
                {
                    // CREAMOS UNA NUEVA LISTA
                    empleadosList = new List<Empleado>();
                }
                empleadosList.Add(e);
                HttpContext.Session.SetObject("EMPLEADOS", empleadosList);
                ViewBag.Mensaje = "Empleado almacenado";
            }
            List<Empleado> empleados = await this.repo.GetEmpleadosAsync();
            return View(empleados);
        }

        public IActionResult EmpleadosAlmacenados()
        {
            return View();
        }
        #endregion

        #region VERSION 3
        public async Task<IActionResult> SessionEmpleadosBueno(int? idEmpleado)
        {
            if (idEmpleado != null)
            {
                // EN SESSION TENDREMOS UN CONJUNTO DE EMPLEADOS
                List<int> empleadosList;
                if (HttpContext.Session.GetObject<List<int>>("EMPLEADOSBUENOS") != null)
                {
                    empleadosList = HttpContext.Session.GetObject<List<int>>("EMPLEADOSBUENOS");
                }
                else
                {
                    // CREAMOS UNA NUEVA LISTA
                    empleadosList = new List<int>();
                }
                empleadosList.Add(idEmpleado.Value);
                HttpContext.Session.SetObject("EMPLEADOSBUENOS", empleadosList);
                ViewBag.Mensaje = "Empleado almacenado";
            }
            List<Empleado> empleados = await this.repo.GetEmpleadosAsync();
            return View(empleados);
        }



        public async Task<IActionResult> EmpleadosAlmacenadosBueno()
        {
            List<Empleado> empleados = await this.repo.GetEmpleadosByIds(HttpContext.Session.GetObject<List<int>>("EMPLEADOSBUENOS"));
            if(empleados != null)
            {
                return View(empleados);
            }
            else
            {
                ViewBag.Mensaje = "No hay empleados";
                return View();
            }
        }
        #endregion

        #region VERSION 4
        public async Task<IActionResult> SessionEmpleadosBuenoV4(int? idEmpleado)
        {
            if (idEmpleado != null)
            {
                List<int> idsEmpleadosList;
                if (HttpContext.Session.GetObject<List<int>>("EMPLEADOSBUENOSV4") != null)
                {
                    idsEmpleadosList = HttpContext.Session.GetObject<List<int>>("EMPLEADOSBUENOSV4");
                }
                else
                {
                    idsEmpleadosList = new List<int>();
                }
                idsEmpleadosList.Add(idEmpleado.Value);
                HttpContext.Session.SetObject("EMPLEADOSBUENOSV4", idsEmpleadosList);
                ViewData["MENSAJE"] = "Empleados almacenados: " +idsEmpleadosList.Count();
            }
            List<int> idsEmpleados = HttpContext.Session.GetObject<List<int>>("EMPLEADOSBUENOSV4");
            if (idsEmpleados == null)
            {
                List<Empleado> empleados = await this.repo.GetEmpleadosAsync();
                return View(empleados);
            }
            else
            {
                List<Empleado> empleados = await
                    this.repo.GetEmpleadosExceptIds(idsEmpleados);
                return View(empleados);
            }
        }

        public async Task<IActionResult> EmpleadosAlmacenadosBuenoV4()
        {
            List<int> idsEmpleados = HttpContext.Session.GetObject<List<int>>("EMPLEADOSBUENOSV5");
            if (idsEmpleados == null)
            {
                return View();
            }
            else
            {
                List<Empleado> empleados = await this.repo.GetEmpleadosByIds(idsEmpleados);
                return View(empleados);
            }
        }
        #endregion

        #region VERSION 5 Y 6 
        /* --- VERSION 5 --- */
        public async Task<IActionResult> SessionEmpleadosBuenoV5(int? idEmpleado, int? idFavorito)
        {
            if (idFavorito != null)
            {
                /* COMO ESTOY ALMACENANDO EN CAHCE, VAMOS A GUARDAR DIRCTAMENTE LOS OBJETOS EN LUGAR DE LOS IDS */
                List<Empleado> empleadosFavoritos;
                if (this.memory.Get("FAVORITOS") == null)
                {
                    // NO EXITE NADA EN CACHE
                    empleadosFavoritos = new List<Empleado>();
                }
                else
                {
                    empleadosFavoritos = this.memory.Get<List<Empleado>>("FAVORITOS");
                }
                // BUSCAMOS AL EMPLEADO PARA GUARDARLO
                Empleado empleadoFavorito = await this.repo.GetEmpleadoByIdAsync(idFavorito.Value);
                empleadosFavoritos.Add(empleadoFavorito);
                this.memory.Set("FAVORITOS", empleadosFavoritos);
            }

            if (idEmpleado != null)
            {
                List<int> idsEmpleadosList;
                if (HttpContext.Session.GetObject<List<int>>("EMPLEADOSBUENOSV5") != null)
                {
                    idsEmpleadosList =  HttpContext.Session.GetObject<List<int>>("EMPLEADOSBUENOSV5");
                }
                else
                {
                    idsEmpleadosList = new List<int>();
                }
                idsEmpleadosList.Add(idEmpleado.Value);
                HttpContext.Session.SetObject("EMPLEADOSBUENOSV5", idsEmpleadosList);
                ViewData["MENSAJE"] = "Empleados almacenados: " + idsEmpleadosList.Count;
            }
            List<Empleado> empleados = await this.repo.GetEmpleadosAsync();
            return View(empleados);
        }

        public async Task<IActionResult> EmpleadosFavoritos(int? idEliminar)
        {
            if (idEliminar != null)
            {
                List<Empleado> favoritos = this.memory.Get<List<Empleado>>("FAVORITOS");
                // BUSCAMOS AL EMPLEADO PARA ELIMINAR POR SU ID
                Empleado emp = favoritos.Find(e => e.EmpNo == idEliminar.Value);
                favoritos.Remove(emp);
                if (favoritos.Count == 0)
                {
                    this.memory.Remove("FAVORITOS");
                }
                else
                {
                    this.memory.Set("FAVORITOS", favoritos);
                }
            }
            return View();
        }

        public async Task<IActionResult> EmpleadosAlmacenadosBuenoV5(int? idEliminar)
        {
            List<int> idsEmpleados = HttpContext.Session.GetObject<List<int>>("EMPLEADOSBUENOSV5");
            if(idsEmpleados == null)
            {
                return View();
            }
            else
            {
                // PREGUNTAMOS SI HEMOS RECIBIDO ALGUN DATO PARA ELIMINAR
                if(idEliminar != null)
                {
                    idsEmpleados.Remove(idEliminar.Value);
                    if(idsEmpleados.Count() == 0)
                    {
                        HttpContext.Session.Remove("EMPLEADOSBUENOSV5");
                    }
                    else
                    {
                        HttpContext.Session.SetObject("EMPLEADOSBUENOSV5", idsEmpleados);
                    }
                }
                List<Empleado> empleados = await this.repo.GetEmpleadosByIds(idsEmpleados);
                return View(empleados);
            }
        }
        #endregion
    }
}