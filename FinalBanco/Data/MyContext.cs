using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FinalBanco.Models;

namespace FinalBanco.Data
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions<MyContext> optionsBuilder) :base(optionsBuilder) { }
        
        public DbSet<Usuario> usuarios { get; set; }
        public DbSet<CajaDeAhorro> cajas { get; set; }

        public DbSet<TarjetaDeCredito> tarjetas { get; set; }
        public DbSet<Pago> pagos { get; set; }

        public DbSet<PlazoFijo> plazosFijos { get; set; }
        public DbSet<Movimiento> movimientos { get; set; }

        public DbSet<UsuarioCajaDeAhorro> UsuarioCajaDeAhorro { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            var connectionString = configuration.GetConnectionString("Context");
            optionsBuilder.UseSqlServer(connectionString);

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //nombre de la tabla
            modelBuilder.Entity<Usuario>()
                .ToTable("Usuario")
                .HasKey(u => u._id_usuario);
            //propiedades de los datos
            modelBuilder.Entity<Usuario>(
            usr =>
                {
                    usr.Property(u => u._dni).HasColumnType("int");
                    usr.Property(u => u._dni).IsRequired(true);
                    usr.Property(u => u._nombre).HasColumnType("varchar(50)");
                    usr.Property(u => u._apellido).HasColumnType("varchar(50)");
                    usr.Property(u => u._mail).HasColumnType("varchar(512)");
                    usr.Property(u => u._password).HasColumnType("varchar(200)");
                    usr.Property(u => u._esUsuarioAdmin).HasColumnType("bit");
                    usr.Property(u => u._bloqueado).HasColumnType("bit");
                    usr.Property(u => u._intentosFallidos).HasColumnType("int");
                    usr.Property(u => u._segmento).HasColumnType("int");
                    usr.Property(u => u._segmento).IsRequired(true);
                })
            ;
            //Ignoro, no agrego UsuarioManager a la base de datos
            //modelBuilder.Ignore<Banco>();

            //nombre de la tabla
            modelBuilder.Entity<CajaDeAhorro>()
                .ToTable("Caja_ahorro")
                .HasKey(c => c._id_caja);
            //propiedades de los datos
            modelBuilder.Entity<CajaDeAhorro>(
                caja =>
                {
                    caja.Property(c => c._cbu).HasColumnType("nvarchar(200)");
                    caja.Property(c => c._cbu).IsRequired(true);
                    caja.Property(c => c._saldo).HasColumnType("float");
                });



            //DEFINICIÓN DE LA RELACIÓN MANY TO MANY USUARIO <-> PAIS
            modelBuilder.Entity<Usuario>()
                .HasMany(U => U.cajas)
                .WithMany(P => P._titulares)
                .UsingEntity<UsuarioCajaDeAhorro>(
                    eup => eup.HasOne(up => up.caja).WithMany(p => p.usuarioCajas).HasForeignKey(u => u._id_caja),
                    eup => eup.HasOne(up => up.user).WithMany(u => u.usuarioCajas).HasForeignKey(u => u._id_usuario),
                    eup => eup.HasKey(k => new { k._id_caja, k._id_usuario })
                );

            //nombre de la tabla
            modelBuilder.Entity<PlazoFijo>()
                .ToTable("Plazo_fijo")
                .HasKey(c => c._id_plazoFijo);
            //propiedades de los datos
            modelBuilder.Entity<PlazoFijo>(
                plazo =>
                {
                    plazo.Property(c => c._id_usuario).HasColumnType("int");
                    plazo.Property(c => c._monto).HasColumnType("float");
                    plazo.Property(c => c._fechaIni).HasColumnType("datetime");
                    plazo.Property(c => c._fechaFin).HasColumnType("datetime");
                    plazo.Property(c => c._tasa).HasColumnType("float");
                    plazo.Property(c => c._pagado).HasColumnType("bit");
                    plazo.Property(c => c._cbu).HasColumnType("int");
                });

            //DEFINICIÓN DE LA RELACIÓN ONE TO MANY USUARIO -> DOMICILIO
            modelBuilder.Entity<PlazoFijo>()
            .HasOne(D => D._titular)
            .WithMany(U => U._plazosFijos)
            .HasForeignKey(D => D._id_usuario)
            .OnDelete(DeleteBehavior.Cascade);


            //nombre de la tabla
            modelBuilder.Entity<TarjetaDeCredito>()
                .ToTable("Tarjeta_credito")
                .HasKey(c => c._id_tarjeta);
            //propiedades de los datos
            modelBuilder.Entity<TarjetaDeCredito>(
                plazo =>
                {
                    plazo.Property(c => c._id_usuario).HasColumnType("int");
                    plazo.Property(c => c._numero).HasColumnType("nvarchar(200)");
                    plazo.Property(c => c._codigoV).HasColumnType("int");
                    plazo.Property(c => c._limite).HasColumnType("float");
                    plazo.Property(c => c._consumos).HasColumnType("float");
                });

            //DEFINICIÓN DE LA RELACIÓN ONE TO MANY USUARIO -> DOMICILIO
            modelBuilder.Entity<TarjetaDeCredito>()
            .HasOne(D => D._titular)
            .WithMany(U => U._tarjetas)
            .HasForeignKey(D => D._id_usuario)
            .OnDelete(DeleteBehavior.Cascade);

            //nombre de la tabla
            modelBuilder.Entity<Pago>()
                .ToTable("Pago")
                .HasKey(c => c._id_pago);
            //propiedades de los datos
            modelBuilder.Entity<Pago>(
                plazo =>
                {
                    plazo.Property(c => c._id_usuario).HasColumnType("int");
                    plazo.Property(c => c._monto).HasColumnType("float");
                    plazo.Property(c => c._pagado).HasColumnType("bit");
                    plazo.Property(c => c._metodo).HasColumnType("nvarchar(200)");
                    plazo.Property(c => c._detalle).HasColumnType("nvarchar(200)");
                    plazo.Property(c => c._id_metodo).HasColumnType("bigint");
                });

            //DEFINICIÓN DE LA RELACIÓN ONE TO MANY USUARIO -> DOMICILIO
            modelBuilder.Entity<Pago>()
            .HasOne(D => D._usuario)
            .WithMany(U => U._pagos)
            .HasForeignKey(D => D._id_usuario)
            .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Movimiento>()
                       .ToTable("Movimiento")
                .HasKey(M => M._id_Movimiento);

            //DEFINICIÓN DE LA RELACIÓN ONE TO MANY Movimiento>>CajaDeAhorro
            modelBuilder.Entity<Movimiento>()
            .HasOne(M => M._cajaDeAhorro)
            .WithMany(C => C._movimientos)
            .HasForeignKey(M => M._id_CajaDeAhorro)
            .OnDelete(DeleteBehavior.Cascade);

            //seed ojo que se incorporan a las migraciones.

            /*
            modelBuilder.Entity<Usuario>().HasData(
    new { _id_usuario = 1, _dni = 111, _nombre = "MATIAS", _apellido = "GREGO", _mail = "M@G", _password = "111", _bloqueado = false, _esUsuarioAdmin = false, _intentosFallidos = 0 },
    new { _id_usuario = 2, _dni = 222, _nombre = "ALAN", _apellido = "RIVA", _mail = "A@R", _password = "222", _bloqueado = false, _esUsuarioAdmin = false, _intentosFallidos = 0 },
    new { _id_usuario = 3, _dni = 333, _nombre = "NICOLAS", _apellido = "VILLEGAS", _mail = "N@V", _password = "333", _bloqueado = false, _esUsuarioAdmin = false, _intentosFallidos = 0 },
    new { _id_usuario = 4, _dni = 444, _nombre = "WALTER", _apellido = "GOMEZ", _mail = "W@G", _password = "444", _bloqueado = false, _esUsuarioAdmin = true, _intentosFallidos = 0 });
            modelBuilder.Entity<CajaDeAhorro>().HasData(
                  new { _id_caja = 1, _cbu = "11120221121", _saldo = 0.00 },
                  new { _id_caja = 2, _cbu = "22220221122", _saldo = 0.00 },
                  new { _id_caja = 3, _cbu = "33320221123", _saldo = 0.00 },
                  new { _id_caja = 4, _cbu = "44420221124", _saldo = 0.00 },
                  new { _id_caja = 5, _cbu = "55520221125", _saldo = 0.00 },
                  new { _id_caja = 6, _cbu = "66620221125", _saldo = 0.00 });
            modelBuilder.Entity<UsuarioCajaDeAhorro>().HasData(
                new { _id_caja = 1, _id_usuario = 1 },
                new { _id_caja = 2, _id_usuario = 2 },
                new { _id_caja = 3, _id_usuario = 3 },
                new { _id_caja = 4, _id_usuario = 4 },
                new { _id_caja = 5, _id_usuario = 1 },
                new { _id_caja = 6, _id_usuario = 1 });
            modelBuilder.Entity<TarjetaDeCredito>().HasData(
                new { _id_tarjeta = 1, _id_usuario = 1, _numero = "11120221121", _codigoV = 0, _limite = 500000.00, _consumos = 100.00 },
                new { _id_tarjeta = 2, _id_usuario = 2, _numero = "22220221121", _codigoV = 0, _limite = 400000.00, _consumos = 900.00 },
                new { _id_tarjeta = 3, _id_usuario = 3, _numero = "33320221121", _codigoV = 0, _limite = 600000.00, _consumos = 400.00 },
                new { _id_tarjeta = 4, _id_usuario = 4, _numero = "44420221121", _codigoV = 0, _limite = 200000.00, _consumos = 600.00 });
            modelBuilder.Entity<PlazoFijo>().HasData(
                new { _id_plazoFijo = 1, _id_usuario = 1, _monto = 1000.00, _fechaIni = DateTime.Now, _fechaFin = DateTime.Now.AddMonths(1), _tasa = 1.5, _pagado = false , _cbu = "11120221110" },
                new { _id_plazoFijo = 2, _id_usuario = 2, _monto = 2000.00, _fechaIni = DateTime.Now, _fechaFin = DateTime.Now.AddMonths(1), _tasa = 1.5, _pagado = false , _cbu = "11120221115" },
                new { _id_plazoFijo = 3, _id_usuario = 3, _monto = 3000.00, _fechaIni = DateTime.Now, _fechaFin = DateTime.Now.AddMonths(1), _tasa = 1.5, _pagado = false , _cbu = "11120221125" },
                new { _id_plazoFijo = 4, _id_usuario = 4, _monto = 4000.00, _fechaIni = DateTime.Now, _fechaFin = DateTime.Now.AddMonths(1), _tasa = 1.5, _pagado = false , _cbu = "11120221135" });

            

            modelBuilder.Entity<Usuario>().HasData(
        new { _id_usuario = 100, _dni = 1110, _nombre = "luisAdm", _apellido = "Carvalhosa", _mail = "l@dc", _password = "321", _bloqueado = false, _esUsuarioAdmin = true, _intentosFallidos = 0 });
        
            */

        
        }

       

    }
}
