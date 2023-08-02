using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalBanco.Models
{
    public class Pago
	{
		public Pago() { }
		public Pago(int id, Usuario usuario, double monto, string metodo, string detalle, long id_metodo)
		{
			_id_pago = id;
			_usuario = usuario;
			_monto = monto;
			_pagado = false;
			_metodo = metodo;
			_detalle = detalle;
			_id_metodo = id_metodo;

		}

        public Pago(double monto, bool pagado, String metodo, String detalle, long id_metodo)
        {
            _monto = monto;
            _metodo = metodo;
            _detalle=detalle;
            _id_metodo = id_metodo;
            _pagado = pagado;

        }

        public Pago(int id, int id_usuario, double monto,bool pagado,String metodo, String detalle, long id_metodo)
		{
            _id_pago = id;
			_id_usuario=id_usuario;
			_monto = monto;
			_metodo = metodo;
			_detalle=detalle;
			_id_metodo = id_metodo;
		    _pagado = pagado;

		}

		private int id;

		public int _id_pago
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

		private Usuario usuario;

		public Usuario _usuario
		{
			get { return usuario; }
			set { usuario = value; }
		}

		private double monto;

		public double _monto
		{
			get { return monto; }
			set { monto = value; }
		}

		private bool pagado;

		public bool _pagado
		{
			get { return pagado; }
			set { pagado = value; }
		}

		private string metodo;

		public string _metodo
		{
			get { return metodo; }
			set { metodo = value; }
		}

		private string detalle;

		public string _detalle
		{
			get { return detalle; }
			set { detalle = value; }
		}

		 private long id_metodo;

		public long _id_metodo
		{
			get { return id_metodo; }
			set { id_metodo = value; }
		}
		


	}
}
