using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinalBanco.Migrations
{
    /// <inheritdoc />
    public partial class actualizacionPassword : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Caja_ahorro",
                columns: table => new
                {
                    idcaja = table.Column<int>(name: "_id_caja", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    cbu = table.Column<string>(name: "_cbu", type: "nvarchar(200)", nullable: false),
                    saldo = table.Column<double>(name: "_saldo", type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Caja_ahorro", x => x.idcaja);
                });

            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    idusuario = table.Column<int>(name: "_id_usuario", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    dni = table.Column<int>(name: "_dni", type: "int", nullable: false),
                    nombre = table.Column<string>(name: "_nombre", type: "varchar(50)", nullable: false),
                    apellido = table.Column<string>(name: "_apellido", type: "varchar(50)", nullable: false),
                    mail = table.Column<string>(name: "_mail", type: "varchar(512)", nullable: false),
                    password = table.Column<string>(name: "_password", type: "varchar(200)", nullable: false),
                    intentosFallidos = table.Column<int>(name: "_intentosFallidos", type: "int", nullable: false),
                    esUsuarioAdmin = table.Column<bool>(name: "_esUsuarioAdmin", type: "bit", nullable: false),
                    bloqueado = table.Column<bool>(name: "_bloqueado", type: "bit", nullable: false),
                    segmento = table.Column<int>(name: "_segmento", type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario", x => x.idusuario);
                });

            migrationBuilder.CreateTable(
                name: "Movimiento",
                columns: table => new
                {
                    idMovimiento = table.Column<int>(name: "_id_Movimiento", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    idCajaDeAhorro = table.Column<int>(name: "_id_CajaDeAhorro", type: "int", nullable: false),
                    detalle = table.Column<string>(name: "_detalle", type: "nvarchar(max)", nullable: false),
                    monto = table.Column<double>(name: "_monto", type: "float", nullable: false),
                    fecha = table.Column<DateTime>(name: "_fecha", type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movimiento", x => x.idMovimiento);
                    table.ForeignKey(
                        name: "FK_Movimiento_Caja_ahorro__id_CajaDeAhorro",
                        column: x => x.idCajaDeAhorro,
                        principalTable: "Caja_ahorro",
                        principalColumn: "_id_caja",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pago",
                columns: table => new
                {
                    idpago = table.Column<int>(name: "_id_pago", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    idusuario = table.Column<int>(name: "_id_usuario", type: "int", nullable: false),
                    monto = table.Column<double>(name: "_monto", type: "float", nullable: false),
                    pagado = table.Column<bool>(name: "_pagado", type: "bit", nullable: false),
                    metodo = table.Column<string>(name: "_metodo", type: "nvarchar(200)", nullable: false),
                    detalle = table.Column<string>(name: "_detalle", type: "nvarchar(200)", nullable: false),
                    idmetodo = table.Column<long>(name: "_id_metodo", type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pago", x => x.idpago);
                    table.ForeignKey(
                        name: "FK_Pago_Usuario__id_usuario",
                        column: x => x.idusuario,
                        principalTable: "Usuario",
                        principalColumn: "_id_usuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Plazo_fijo",
                columns: table => new
                {
                    idplazoFijo = table.Column<int>(name: "_id_plazoFijo", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    idusuario = table.Column<int>(name: "_id_usuario", type: "int", nullable: false),
                    monto = table.Column<double>(name: "_monto", type: "float", nullable: false),
                    fechaIni = table.Column<DateTime>(name: "_fechaIni", type: "datetime", nullable: false),
                    fechaFin = table.Column<DateTime>(name: "_fechaFin", type: "datetime", nullable: false),
                    tasa = table.Column<double>(name: "_tasa", type: "float", nullable: false),
                    pagado = table.Column<bool>(name: "_pagado", type: "bit", nullable: false),
                    cbu = table.Column<int>(name: "_cbu", type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plazo_fijo", x => x.idplazoFijo);
                    table.ForeignKey(
                        name: "FK_Plazo_fijo_Usuario__id_usuario",
                        column: x => x.idusuario,
                        principalTable: "Usuario",
                        principalColumn: "_id_usuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tarjeta_credito",
                columns: table => new
                {
                    idtarjeta = table.Column<int>(name: "_id_tarjeta", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    idusuario = table.Column<int>(name: "_id_usuario", type: "int", nullable: false),
                    numero = table.Column<string>(name: "_numero", type: "nvarchar(200)", nullable: false),
                    codigoV = table.Column<int>(name: "_codigoV", type: "int", nullable: false),
                    limite = table.Column<double>(name: "_limite", type: "float", nullable: false),
                    consumos = table.Column<double>(name: "_consumos", type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tarjeta_credito", x => x.idtarjeta);
                    table.ForeignKey(
                        name: "FK_Tarjeta_credito_Usuario__id_usuario",
                        column: x => x.idusuario,
                        principalTable: "Usuario",
                        principalColumn: "_id_usuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioCajaDeAhorro",
                columns: table => new
                {
                    idcaja = table.Column<int>(name: "_id_caja", type: "int", nullable: false),
                    idusuario = table.Column<int>(name: "_id_usuario", type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioCajaDeAhorro", x => new { x.idcaja, x.idusuario });
                    table.ForeignKey(
                        name: "FK_UsuarioCajaDeAhorro_Caja_ahorro__id_caja",
                        column: x => x.idcaja,
                        principalTable: "Caja_ahorro",
                        principalColumn: "_id_caja",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsuarioCajaDeAhorro_Usuario__id_usuario",
                        column: x => x.idusuario,
                        principalTable: "Usuario",
                        principalColumn: "_id_usuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Movimiento__id_CajaDeAhorro",
                table: "Movimiento",
                column: "_id_CajaDeAhorro");

            migrationBuilder.CreateIndex(
                name: "IX_Pago__id_usuario",
                table: "Pago",
                column: "_id_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_Plazo_fijo__id_usuario",
                table: "Plazo_fijo",
                column: "_id_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_Tarjeta_credito__id_usuario",
                table: "Tarjeta_credito",
                column: "_id_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioCajaDeAhorro__id_usuario",
                table: "UsuarioCajaDeAhorro",
                column: "_id_usuario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Movimiento");

            migrationBuilder.DropTable(
                name: "Pago");

            migrationBuilder.DropTable(
                name: "Plazo_fijo");

            migrationBuilder.DropTable(
                name: "Tarjeta_credito");

            migrationBuilder.DropTable(
                name: "UsuarioCajaDeAhorro");

            migrationBuilder.DropTable(
                name: "Caja_ahorro");

            migrationBuilder.DropTable(
                name: "Usuario");
        }
    }
}
