namespace MEIAdmin.Models
{
    public class Colaborador
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Cargo { get; set; }
        public string Telefone { get; set; }
        public string Email { get; set; }

        // Endereço do Colaborador
        public string Endereco { get; set; }
        public string Numero { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string CEP { get; set; }

        // Datas de contratação e demissão
        public DateTime DataContratacao { get; set; }
        public DateTime? DataDemissao { get; set; }  // Pode ser nulo caso o colaborador ainda esteja ativo
    }
}