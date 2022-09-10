
using System.ComponentModel.DataAnnotations;


namespace ApiRestAlchemy.Database

{
    public class UserInfo
    {
        [Key]
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
