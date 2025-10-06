# 🧩 Diagrama de Clases – Caso de Uso: Compra de Bono de Bocadillo

Este diagrama representa las clases del caso de uso **"Compra de Bono de Bocadillo"**, sus atributos principales y las relaciones entre ellas según el modelo implementado en Entity Framework Core.

```mermaid
classDiagram
    class TipoBocadillo {
        +int IdTipo
        +string NombreTipo
    }

    class BonoBocadillo {
        +int BonoId
        +string Nombre
        +int NBocadillos
        +int CantidadDisponible
        +decimal PVP
    }

    class BonosComprados {
        +int BonoId
        +int CompraId
        +int Cantidad
        +decimal PrecioBono
    }

    class CompraBono {
        +int CompraBonoId
        +string NombreCliente
        +string ApellidoBono1
        +string? ApellidoBono2
        +DateTime FechaCompraBono
        +int NBonos
        +decimal PrecioTotalBono
        +int MetodoPagoId
    }

    class MetodoPago {
        <<enumeration>>
        Tarjeta
        Paypal
        GooglePay
    }

    %% Relaciones
    TipoBocadillo "1" --> "N" BonoBocadillo : contiene >
    BonoBocadillo "1" --> "N" BonosComprados : incluye >
    CompraBono "1" --> "N" BonosComprados : agrupa >
    CompraBono "N" --> "1" MetodoPago : usa >
