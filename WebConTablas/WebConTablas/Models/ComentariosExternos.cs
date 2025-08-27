
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebConTablas.Models
{
	public class ComentariosExternos
	{
		[Key]
		public int ID_ComentarioExterno { get; set; }

		[Required]
		[ForeignKey(nameof(UsuarioExterno))]
		public int IdUsuarioExterno { get; set; }

		public UsuarioExterno UsuarioExterno { get; set; }

		[Required]
		public string Comentario { get; set; }

		public DateTime Fecha { get; set; } = DateTime.UtcNow;
	}
}
