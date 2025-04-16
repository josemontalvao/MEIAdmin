using MEIAdmin.Models;
namespace MEIAdmin.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class ContaReceber
{
    [Key]
    public int Id { get; set; }

    [Required]
    public decimal Valor { get; set; }

    [Required]
    public DateTime DataVencimento { get; set; }

    public DateTime? DataRecebimento { get; set; }

    [Required]
    public string Status { get; set; } = "Pendente"; // Pendente, Recebido, Atrasado

    public string? Descricao { get; set; }

    [Required]
    public int ClienteId { get; set; }

    public Cliente? Cliente { get; set; }
}
