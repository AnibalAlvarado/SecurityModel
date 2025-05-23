using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity.Model;
using Microsoft.EntityFrameworkCore;

namespace Entity.Contexts
{
    internal class DataInitial
    {
        public static void Data(ModelBuilder modelBuilder)
        {
            // Roles
            modelBuilder.Entity<Rol>().HasData(
                new Rol { Id = 1, Name = "Administrador", Description = "Rol de administrador" },
                new Rol { Id = 2, Name = "Usuario", Description = "Rol de usuario estándar" }
            );

            // Permisos
            modelBuilder.Entity<Permission>().HasData(
                new Permission { Id = 1, Name = "Ver", Description = "Permiso para ver" },
                new Permission { Id = 2, Name = "Editar", Description = "Permiso para editar" },
                new Permission { Id = 3, Name = "Eliminar", Description = "Permiso para eliminar" }
            );

            // Personas
            modelBuilder.Entity<Person>().HasData(
                new Person { Id = 1, Firstname = "Admin", Lastname = "Principal", PhoneNumber = "111111111" },
                new Person { Id = 2, Firstname = "Usuario", Lastname = "Demo", PhoneNumber = "222222222" }
            );

            // Usuarios
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Username = "admin", Email = "anibalalvaradoandrade@gmail.com", Password = "123", PersonId = 1 },
                new User { Id = 2, Username = "usuario", Email = "usuario@demo.com", Password = "usuario123", PersonId = 2 }
            );

            // Módulos
            modelBuilder.Entity<Module>().HasData(
                new Module { Id = 1, Name = "Gestión", Description = "Módulo de gestión" }
            );

            // Formularios
            modelBuilder.Entity<Form>().HasData(
                new Form { Id = 1, Name = "Principal", Description = "Formulario principal" }
            );

            // FormModule (relación muchos a muchos)
            modelBuilder.Entity<FormModule>().HasData(
                new FormModule { Id = 1, FormId = 1, ModuleId = 1 }
            );

            // RolUser (relación muchos a muchos)
            modelBuilder.Entity<RolUser>().HasData(
                new RolUser { Id = 1, RolId = 1, UserId = 1 },
                new RolUser { Id = 2, RolId = 2, UserId = 2 }
            );

            // RolFormPermission (relación muchos a muchos)
            modelBuilder.Entity<RolFormPermission>().HasData(
                new RolFormPermission { Id = 1, RolId = 1, FormId = 1, PermissionId = 1 },
                new RolFormPermission { Id = 2, RolId = 1, FormId = 1, PermissionId = 2 },
                new RolFormPermission { Id = 3, RolId = 2, FormId = 1, PermissionId = 1 }
            );
        }
    }
}
