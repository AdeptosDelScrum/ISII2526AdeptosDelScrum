using Microsoft.AspNetCore.Identity;

namespace AppForSEII2526.API.Models;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser 
{
    public ApplicationUser()
    {
    }
    public ApplicationUser(string nombre, string apellido1, string apellido2 )
    {
        NombreCliente = nombre;
        Apellido1_Cliente = apellido1;
        Apellido2_Cliente = apellido2;
    }

    public string NombreCliente { get; set; }
    public string Apellido1_Cliente { get; set; }
    public string? Apellido2_Cliente { get; set; }
}