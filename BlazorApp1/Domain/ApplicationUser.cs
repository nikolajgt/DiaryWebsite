using Microsoft.AspNetCore.Identity;

namespace BlazorApp1.Domain;

public class ApplicationUser : IdentityUser
{
    public List<DiaryTable> DiaryConfigs { get; set; }
}
