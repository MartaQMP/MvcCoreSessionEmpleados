using Microsoft.AspNetCore.Mvc;
using MvcCoreSessionEmpleados.Extensions;
using MvcCoreSessionEmpleados.Models;
using MvcCoreSessionEmpleados.Repositories;
using System.Threading.Tasks;

namespace MvcCoreSessionEmpleados.Controllers
{
    public class EmpleadoController : Controller
    {
        RepositoryEmpleados repo;

        public EmpleadoController(RepositoryEmpleados repo)
        {
            this.repo = repo;
        }

        public IActionResult Index()
        {
            return View();
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

        public async Task<IActionResult> SessionEmpleadosBuenoV4(int? idEmpleado)
        {
            List<Empleado> empleados = await this.repo.GetEmpleadosAsync();
            if(HttpContext.Session.GetObject<List<int>>("EMPLEADOSBUENOSV4") != null)
            {
                empleados = await this.repo.GetEmpleadosExceptIds(HttpContext.Session.GetObject<List<int>>("EMPLEADOSBUENOSV4"));
            }
            else if (idEmpleado != null)
            {
                List<int> empleadosList;
                if (HttpContext.Session.GetObject<List<int>>("EMPLEADOSBUENOSV4") != null)
                {
                    empleadosList = HttpContext.Session.GetObject<List<int>>("EMPLEADOSBUENOSV4");
                }
                else
                {
                    empleadosList = new List<int>();
                }
                empleadosList.Add(idEmpleado.Value);
                HttpContext.Session.SetObject("EMPLEADOSBUENOSV4", empleadosList);
                empleados = await this.repo.GetEmpleadosExceptIds(empleadosList);
                ViewBag.Mensaje = "Empleado almacenado";
            }
            return View(empleados);
        }

        public async Task<IActionResult> EmpleadosAlmacenadosBuenoV4()
        {
            List<Empleado> empleados = await this.repo.GetEmpleadosByIds(HttpContext.Session.GetObject<List<int>>("EMPLEADOSBUENOSV4"));
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
    }
}
