using System.ComponentModel.DataAnnotations;

namespace ConectaStore.API.Models;

public class Venda
{
    [Key]
    public int Id { get; set; }

    [Required]
    public DateTime Data { get; set; } = DateTime.Now;

    [Required]
    [StringLength(200)]
    public string Cliente { get; set; }

    [Required]
    public decimal Total { get; set; }

    [Required]
    [StringLength(50)]
    public string Status { get; set; } = "Concluída";

    public List<ItemVenda> Itens { get; set; } = new();
}
