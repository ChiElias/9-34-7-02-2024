namespace API.Entities;
using Microsoft.AspNetCore.Identity;

#nullable disable 
public class AppRole : IdentityRole<int>
{
    public ICollection<AppUserRole> UserRoles { get; set; }
}