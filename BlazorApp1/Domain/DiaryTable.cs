using Microsoft.AspNetCore.Identity;

namespace BlazorApp1.Domain;

public class DiaryTable
{
    public DiaryTable() { } 
    public Guid Id { get; set; }
    public string DiaryName { get; set; }
    public byte[] Key { get; set; }
    public byte[] IV { get; set; }
    public string ApplicationUserId { get; set; }
    public ApplicationUser ApplicationUser { get; set; }
}
