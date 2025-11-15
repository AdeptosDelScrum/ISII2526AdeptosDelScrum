using Microsoft.AspNetCore.Identity;

namespace AppForSEII2526.API.Models;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser {
    public ApplicationUser()
    {
    }
    public ApplicationUser(int id, string nombre, string apellido1, string apellido2)
    {
        Id = id;
        NombreCliente = nombre;
        Apellido1_Cliente = apellido1;
        Apellido2_Cliente = apellido2;
    }
    public int Id { get; set; }
    [Required]
    public string NombreCliente { get; set; }
    [Required]
    public string Apellido1_Cliente { get; set; }
    public string? Apellido2_Cliente { get; set; }
}