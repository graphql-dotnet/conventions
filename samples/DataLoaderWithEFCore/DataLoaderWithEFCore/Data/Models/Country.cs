using System.ComponentModel.DataAnnotations;

namespace DataLoaderWithEFCore.Data.Models
{
    public class Country
    {
        [Key, MaxLength(2)]
        public string Code { get; set; }

        public string Name { get; set; }
    }
}
