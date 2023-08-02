using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalBanco.Models
{
    public class PlazoFijo
    {
		public PlazoFijo() { }
        public PlazoFijo(double monto, DateTime fechaFin, double tasa)
        {
			_monto = monto;
			_fechaIni= DateTime.Now;
            _fechaFin = fechaFin;
			_tasa = tasa;
			_pagado = false;
        }
		
		

        public PlazoFijo(int id, int id_usuario, double monto, DateTime fechaFin, double tasa, bool pagado)
        {
            _id_plazoFijo = id;
            _id_usuario = id_usuario;
            _monto = monto;
            _fechaIni = DateTime.Now;
            _fechaFin = fechaFin;
            _tasa = tasa;
            _pagado = pagado;
        }

        public PlazoFijo(double monto, DateTime fechaFin, double tasa, int cbu)
        {
            _monto = monto;
            _fechaIni = DateTime.Now;
            _fechaFin = fechaFin;
            _tasa = tasa;
            _pagado = false;
            _cbu = cbu;
        }


 

        private int id;

		public int _id_plazoFijo
		{
			get { return id; }
			set { id = value; }
		}

		private int id_usuario;

		public int _id_usuario
		{
			get { return id_usuario; }
			set { id_usuario = value; }
		} 


		private Usuario titular;

		public Usuario _titular
		{
			get { return titular; }
			set { titular = value; }
		}

		private double monto;

		public double _monto
		{
			get { return monto; }
			set { monto = value; }
		}

		private DateTime fechaIni;

		public DateTime _fechaIni
		{
			get { return fechaIni; }
			set { fechaIni = value; }
		}

		private DateTime fechaFin;

		public DateTime _fechaFin
		{
			get { return fechaFin; }
			set { fechaFin = value; }
		}

		private double tasa;

		public double _tasa
		{
			get { return tasa; }
			set { tasa = value; }
		}

		private bool pagado;

		public bool _pagado
		{
			get { return pagado; }
			set { pagado = value; }
		}


        private int cbu;

        public int _cbu
        {
            get { return cbu; }
            set { cbu = value; }
        }


    }
}
