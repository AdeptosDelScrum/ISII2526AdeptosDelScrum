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

        // atributo totalmente calificado + alias para el enum
        [System.ComponentModel.DataAnnotations.DataTypeAttribute(DT.DateTime)]
        public DateTime Fecha { get; set; }

        [System.ComponentModel.DataAnnotations.DataTypeAttribute(DT.Currency)]
        public decimal PrecioTotal { get; set; }

        public IList<CompraBonoItemDTO> Items { get; set; } = new List<CompraBonoItemDTO>();
    }

    public class CompraBonoItemDTO
    {
        public long BonoId { get; set; }
        public string Nombre { get; set; } = "";
        public string? Tipo { get; set; } // vegano | vegetariano | sin gluten | normal

        [System.ComponentModel.DataAnnotations.DataTypeAttribute(DT.Currency)]
        public decimal Pvp { get; set; } // precio individual

        public int Cantidad { get; set; }
    }
}
