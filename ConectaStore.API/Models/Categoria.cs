using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConectaStore.API.Models;

[Table("Categorias")]
    public class Categoria
    {
        [Key]
        public int Id { get; set; }

        [Required] 
        [StringLength(50)]
        public string Nome { get; set; }
        
        [StringLength(300)]
        public string Foto { get; set; } 
        
        [StringLength(26)]
        public string Cor { get; set; } = "rgba(0,0,0,1)";
    }
