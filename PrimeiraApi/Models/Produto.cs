using System.ComponentModel.DataAnnotations;

namespace PrimeiraApi.Models
{
    public class Produto
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public string? Nome { get; set; }
       
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [Range(1, int.MaxValue, ErrorMessage = "O campo {0} precisa ter o valor maior que Zero")]
        public decimal Preco { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public int QtEstoque { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public string? Descricao { get; set; }
    }
}
