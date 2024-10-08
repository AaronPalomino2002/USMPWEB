﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace USMPWEB.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<USMPWEB.Models.Login> DataHome { get;set;}
    public DbSet<USMPWEB.Models.Alumnos> DataAlumnos { get;set;}
    public DbSet<USMPWEB.Models.Inscripciones> DataInscripciones { get;set;}
    public DbSet<USMPWEB.Models.Eventos> DataEventos { get;set;}
    public DbSet<USMPWEB.Models.Talleres> DataTalleres { get;set;}
    public DbSet<USMPWEB.Models.Certificados> DataCertificados { get;set;}
    public DbSet<USMPWEB.Models.Contacto> DataContacto { get;set;}
}
