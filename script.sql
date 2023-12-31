USE [master]
GO
/****** Object:  Database [TP2_PLATAFORMAS]    Script Date: 1/8/2023 23:56:44 ******/
CREATE DATABASE [TP2_PLATAFORMAS]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'TP2_PLATAFORMAS', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS01\MSSQL\DATA\TP2_PLATAFORMAS.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'TP2_PLATAFORMAS_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.SQLEXPRESS01\MSSQL\DATA\TP2_PLATAFORMAS_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO
ALTER DATABASE [TP2_PLATAFORMAS] SET COMPATIBILITY_LEVEL = 150
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [TP2_PLATAFORMAS].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [TP2_PLATAFORMAS] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [TP2_PLATAFORMAS] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [TP2_PLATAFORMAS] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [TP2_PLATAFORMAS] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [TP2_PLATAFORMAS] SET ARITHABORT OFF 
GO
ALTER DATABASE [TP2_PLATAFORMAS] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [TP2_PLATAFORMAS] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [TP2_PLATAFORMAS] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [TP2_PLATAFORMAS] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [TP2_PLATAFORMAS] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [TP2_PLATAFORMAS] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [TP2_PLATAFORMAS] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [TP2_PLATAFORMAS] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [TP2_PLATAFORMAS] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [TP2_PLATAFORMAS] SET  ENABLE_BROKER 
GO
ALTER DATABASE [TP2_PLATAFORMAS] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [TP2_PLATAFORMAS] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [TP2_PLATAFORMAS] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [TP2_PLATAFORMAS] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [TP2_PLATAFORMAS] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [TP2_PLATAFORMAS] SET READ_COMMITTED_SNAPSHOT ON 
GO
ALTER DATABASE [TP2_PLATAFORMAS] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [TP2_PLATAFORMAS] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [TP2_PLATAFORMAS] SET  MULTI_USER 
GO
ALTER DATABASE [TP2_PLATAFORMAS] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [TP2_PLATAFORMAS] SET DB_CHAINING OFF 
GO
ALTER DATABASE [TP2_PLATAFORMAS] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [TP2_PLATAFORMAS] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [TP2_PLATAFORMAS] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [TP2_PLATAFORMAS] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [TP2_PLATAFORMAS] SET QUERY_STORE = OFF
GO
USE [TP2_PLATAFORMAS]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 1/8/2023 23:56:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Caja_ahorro]    Script Date: 1/8/2023 23:56:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Caja_ahorro](
	[_id_caja] [int] IDENTITY(1,1) NOT NULL,
	[_cbu] [nvarchar](200) NOT NULL,
	[_saldo] [float] NOT NULL,
 CONSTRAINT [PK_Caja_ahorro] PRIMARY KEY CLUSTERED 
(
	[_id_caja] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Movimiento]    Script Date: 1/8/2023 23:56:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Movimiento](
	[_id_Movimiento] [int] IDENTITY(1,1) NOT NULL,
	[_id_CajaDeAhorro] [int] NOT NULL,
	[_detalle] [nvarchar](max) NOT NULL,
	[_monto] [float] NOT NULL,
	[_fecha] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_Movimiento] PRIMARY KEY CLUSTERED 
(
	[_id_Movimiento] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Pago]    Script Date: 1/8/2023 23:56:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Pago](
	[_id_pago] [int] IDENTITY(1,1) NOT NULL,
	[_id_usuario] [int] NOT NULL,
	[_monto] [float] NOT NULL,
	[_pagado] [bit] NOT NULL,
	[_metodo] [nvarchar](200) NOT NULL,
	[_detalle] [nvarchar](200) NOT NULL,
	[_id_metodo] [bigint] NOT NULL,
 CONSTRAINT [PK_Pago] PRIMARY KEY CLUSTERED 
(
	[_id_pago] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Plazo_fijo]    Script Date: 1/8/2023 23:56:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Plazo_fijo](
	[_id_plazoFijo] [int] IDENTITY(1,1) NOT NULL,
	[_id_usuario] [int] NOT NULL,
	[_monto] [float] NOT NULL,
	[_fechaIni] [datetime] NOT NULL,
	[_fechaFin] [datetime] NOT NULL,
	[_tasa] [float] NOT NULL,
	[_pagado] [bit] NOT NULL,
	[_cbu] [int] NOT NULL,
 CONSTRAINT [PK_Plazo_fijo] PRIMARY KEY CLUSTERED 
(
	[_id_plazoFijo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Tarjeta_credito]    Script Date: 1/8/2023 23:56:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tarjeta_credito](
	[_id_tarjeta] [int] IDENTITY(1,1) NOT NULL,
	[_id_usuario] [int] NOT NULL,
	[_numero] [nvarchar](200) NOT NULL,
	[_codigoV] [int] NOT NULL,
	[_limite] [float] NOT NULL,
	[_consumos] [float] NOT NULL,
 CONSTRAINT [PK_Tarjeta_credito] PRIMARY KEY CLUSTERED 
(
	[_id_tarjeta] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Usuario]    Script Date: 1/8/2023 23:56:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Usuario](
	[_id_usuario] [int] IDENTITY(1,1) NOT NULL,
	[_dni] [int] NOT NULL,
	[_nombre] [varchar](50) NOT NULL,
	[_apellido] [varchar](50) NOT NULL,
	[_mail] [varchar](512) NOT NULL,
	[_password] [varchar](200) NOT NULL,
	[_intentosFallidos] [int] NOT NULL,
	[_esUsuarioAdmin] [bit] NOT NULL,
	[_bloqueado] [bit] NOT NULL,
	[_segmento] [int] NOT NULL,
 CONSTRAINT [PK_Usuario] PRIMARY KEY CLUSTERED 
(
	[_id_usuario] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UsuarioCajaDeAhorro]    Script Date: 1/8/2023 23:56:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UsuarioCajaDeAhorro](
	[_id_caja] [int] NOT NULL,
	[_id_usuario] [int] NOT NULL,
 CONSTRAINT [PK_UsuarioCajaDeAhorro] PRIMARY KEY CLUSTERED 
(
	[_id_caja] ASC,
	[_id_usuario] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Index [IX_Movimiento__id_CajaDeAhorro]    Script Date: 1/8/2023 23:56:44 ******/
CREATE NONCLUSTERED INDEX [IX_Movimiento__id_CajaDeAhorro] ON [dbo].[Movimiento]
(
	[_id_CajaDeAhorro] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Pago__id_usuario]    Script Date: 1/8/2023 23:56:44 ******/
CREATE NONCLUSTERED INDEX [IX_Pago__id_usuario] ON [dbo].[Pago]
(
	[_id_usuario] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Plazo_fijo__id_usuario]    Script Date: 1/8/2023 23:56:44 ******/
CREATE NONCLUSTERED INDEX [IX_Plazo_fijo__id_usuario] ON [dbo].[Plazo_fijo]
(
	[_id_usuario] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Tarjeta_credito__id_usuario]    Script Date: 1/8/2023 23:56:44 ******/
CREATE NONCLUSTERED INDEX [IX_Tarjeta_credito__id_usuario] ON [dbo].[Tarjeta_credito]
(
	[_id_usuario] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_UsuarioCajaDeAhorro__id_usuario]    Script Date: 1/8/2023 23:56:44 ******/
CREATE NONCLUSTERED INDEX [IX_UsuarioCajaDeAhorro__id_usuario] ON [dbo].[UsuarioCajaDeAhorro]
(
	[_id_usuario] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Movimiento]  WITH CHECK ADD  CONSTRAINT [FK_Movimiento_Caja_ahorro__id_CajaDeAhorro] FOREIGN KEY([_id_CajaDeAhorro])
REFERENCES [dbo].[Caja_ahorro] ([_id_caja])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Movimiento] CHECK CONSTRAINT [FK_Movimiento_Caja_ahorro__id_CajaDeAhorro]
GO
ALTER TABLE [dbo].[Pago]  WITH CHECK ADD  CONSTRAINT [FK_Pago_Usuario__id_usuario] FOREIGN KEY([_id_usuario])
REFERENCES [dbo].[Usuario] ([_id_usuario])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Pago] CHECK CONSTRAINT [FK_Pago_Usuario__id_usuario]
GO
ALTER TABLE [dbo].[Plazo_fijo]  WITH CHECK ADD  CONSTRAINT [FK_Plazo_fijo_Usuario__id_usuario] FOREIGN KEY([_id_usuario])
REFERENCES [dbo].[Usuario] ([_id_usuario])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Plazo_fijo] CHECK CONSTRAINT [FK_Plazo_fijo_Usuario__id_usuario]
GO
ALTER TABLE [dbo].[Tarjeta_credito]  WITH CHECK ADD  CONSTRAINT [FK_Tarjeta_credito_Usuario__id_usuario] FOREIGN KEY([_id_usuario])
REFERENCES [dbo].[Usuario] ([_id_usuario])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Tarjeta_credito] CHECK CONSTRAINT [FK_Tarjeta_credito_Usuario__id_usuario]
GO
ALTER TABLE [dbo].[UsuarioCajaDeAhorro]  WITH CHECK ADD  CONSTRAINT [FK_UsuarioCajaDeAhorro_Caja_ahorro__id_caja] FOREIGN KEY([_id_caja])
REFERENCES [dbo].[Caja_ahorro] ([_id_caja])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UsuarioCajaDeAhorro] CHECK CONSTRAINT [FK_UsuarioCajaDeAhorro_Caja_ahorro__id_caja]
GO
ALTER TABLE [dbo].[UsuarioCajaDeAhorro]  WITH CHECK ADD  CONSTRAINT [FK_UsuarioCajaDeAhorro_Usuario__id_usuario] FOREIGN KEY([_id_usuario])
REFERENCES [dbo].[Usuario] ([_id_usuario])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UsuarioCajaDeAhorro] CHECK CONSTRAINT [FK_UsuarioCajaDeAhorro_Usuario__id_usuario]
GO
USE [master]
GO
ALTER DATABASE [TP2_PLATAFORMAS] SET  READ_WRITE 
GO
