using Microsoft.EntityFrameworkCore;
using MvcCoreSessionEmpleados.Data;
using MvcCoreSessionEmpleados.Models;
using NuGet.Packaging.Signing;

namespace MvcCoreSessionEmpleados.Repositories
{
    public class RepositoryEmpleados
    {
        HospitalContext context;

        public RepositoryEmpleados(HospitalContext context)
        {
            this.context = context;
        }

        public async Task<List<Empleado>> GetEmpleadosAsync()
        {
            return await this.context.Empleados.ToListAsync();
        }

        public async Task<Empleado> GetEmpleadoByIdAsync(int idEmpleado)
        {
            return await this.context.Empleados.Where(e => e.EmpNo == idEmpleado).FirstOrDefaultAsync();
        }

        public async Task<List<Empleado>> GetEmpleadosByIds(List<int> ids)
        {
            return await this.context.Empleados.Where(e => ids.Contains(e.EmpNo)).ToListAsync();
        }

        public async Task<List<Empleado>> GetEmpleadosExceptIds(List<int> ids)
        {
            return await this.context.Empleados.Where(e => !ids.Contains(e.EmpNo)).ToListAsync();
        }
    }
}
