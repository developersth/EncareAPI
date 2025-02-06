namespace EncareAPI.Models
{
public class UserInfo
{
    public string? Email { get; set; }  = null!;
    public string? FullName { get; set; } = null!;
    public string? Gender { get; set; }= null!;
    public string? DateOfBirth { get; set; } = null!;
    public int? Age { get; set; } = null!;
}
}