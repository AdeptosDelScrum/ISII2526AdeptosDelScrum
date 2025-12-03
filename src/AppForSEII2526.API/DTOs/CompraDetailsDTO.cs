using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
// alias para el enum DataType de DataAnnotations
using DT = System.ComponentModel.DataAnnotations.DataType;

namespace AppForSEII2526.API.DTOs
{
    public class CompraDetailsDTO
    {
        public long CompraId { get; set; }
        public string NombreCompleto { get; set; } = "";
        public string Apellidos { get; set; } = "";
        public string MetodoPago { get; set; } = "";
        // ERROR RESUELTO: Se especifica el namespace completo
        [System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.DateTime)]
        public DateTime Fecha { get; set; }
        // ERROR RESUELTO: Se especifica el namespace completo
        [System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.Currency)]
        public decimal PrecioTotal { get; set; }

        public IList<CompraBonoItemDTO> Items { get; set; } = new List<CompraBonoItemDTO>();
    }

    public class CompraBonoItemDTO
    {
        public long BonoId { get; set; }
        public string Nombre { get; set; } = "";
        public string? Tipo { get; set; }       // vegano | vegetariano | sin gluten | normal
        // ERROR RESUELTO: Se especifica el namespace completo
        [System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.Currency)]
        public decimal Pvp { get; set; }        // precio individual
        public int Cantidad { get; set; }
    }
}
