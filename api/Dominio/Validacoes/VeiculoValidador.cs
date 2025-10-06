using MinimalApi.Dominio.DTOs;
using MinimalApi.Dominio.ModelViews;

namespace MinimalApi.Dominio.Validacoes
{
    public static class VeiculoValidador
    {
        public static ErrosDeValidacao ValidaDTO(VeiculoDTO veiculoDTO)
        {
            var validacao = new ErrosDeValidacao
            {
                Mensagens = new List<string>()
            };

            if (string.IsNullOrEmpty(veiculoDTO.Nome))
                validacao.Mensagens.Add("O nome não pode estar em branco.");

            if (string.IsNullOrEmpty(veiculoDTO.Marca))
                validacao.Mensagens.Add("A marca não pode estar em branco.");

            if (veiculoDTO.Ano < 1950)
                validacao.Mensagens.Add("Veículo muito antigo. Somente são aceitos veículos com ano igual ou superior a 1950.");

            return validacao;
        }
    }
}
