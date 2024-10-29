using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace USMPWEB.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<USMPWEB.Models.RegisterViewModel> DataRegistro{ get;set;}
    public DbSet<USMPWEB.Models.Login> DataHome { get;set;}
    public DbSet<USMPWEB.Models.Alumno> DataAlumnos { get;set;}
    public DbSet<USMPWEB.Models.Inscripciones> DataInscripciones { get;set;}
    public DbSet<USMPWEB.Models.Talleres> DataTalleres { get;set;}
    public DbSet<USMPWEB.Models.Certificados> DataCertificados { get;set;}
    public DbSet<USMPWEB.Models.Contacto> DataContacto { get;set;}
    public DbSet<USMPWEB.Models.Carrera> DataCarrera{ get;set;}
    public DbSet<USMPWEB.Models.Facultad> DataFacultad{ get;set;}
    public DbSet<USMPWEB.Models.Campanas> DataCampanas{ get;set;}
    public DbSet<USMPWEB.Models.Categoria> DataCategoria{ get;set;}
    public DbSet<USMPWEB.Models.SubCategoria> DataSubCategoria{ get;set;}
    public DbSet<USMPWEB.Models.EventosInscripciones> DataEventosInscripciones{ get;set;}
}
