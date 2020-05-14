using GestaoAvaliacao.FGVIntegration.Business;
using GestaoAvaliacao.FGVIntegration.Logging;
using GestaoAvaliacao.FGVIntegration.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace GestaoAvaliacao.FGVIntegration.Data
{
    public class EolRepository : LoggingBase, IEolRepository
    {

        private readonly string urlApiEol;

        public EolRepository()
        {
            this.urlApiEol = ConfigurationHelper.BuscarConfiguracaoObrigatoria("ApiFgvEnsinoMedio_UrlApiEol");
        }

        public async Task<ICollection<Escola>> BuscarDiretoresEscolas(ICollection<Escola> pEscolas)
        {
            ICollection<Escola> escolas = new List<Escola>(pEscolas.Count());
            foreach (var escola in pEscolas)
            {
                var diretores = await BuscarFuncionariosCargo(escola.CodigoDaEscola, (int)Enums.CargoEscola.DIRETOR);
                var diretor = diretores.Single();//deve ser apenas 1

                escola.CargoDoResponsavel = diretor.Cargo.Trim();
                escola.RfDoResponsavel = diretor.CodigoRF;
                escola.NomeDoResponsavel = diretor.NomeServidor;
                escola.Tipo = (int)Enums.TipoEscola.MUNICIPAL;
                escola.UF = Enums.UnidadeFederativa.SP.ToString();
                escola.Cidade = (int)Enums.CidadeIBGE.SP_SAO_PAULO;
                escola.CNPJ = "46392114000125";
                escola.WebSite = "https://educacao.sme.prefeitura.sp.gov.br/";

                escolas.Add(escola);
            }
            return escolas;
        }

        public async Task<ICollection<Coordenador>> BuscarCoordenadoresEscolas(ICollection<Escola> pEscolas)
        {
            ICollection<Coordenador> coordenadores = new List<Coordenador>(pEscolas.Count());
            foreach (var escola in pEscolas)
            {
                var coordenadoresEscola = await BuscarFuncionariosCargo(escola.CodigoDaEscola, (int)Enums.CargoEscola.COORDENADOR);
                foreach (var coordenadorEscola in coordenadoresEscola)
                {
                    var coordenador = new Coordenador()
                    {
                        CodigoDaEscola = escola.CodigoDaEscola,
                        EmailDaEscola = escola.Email,
                        RfDoCoordenador = coordenadorEscola.CodigoRF,
                        Nome = coordenadorEscola.NomeServidor,
                        Tipo = (int) Enums.TipoCoordenador.COORDENADOR_ESCOLA,
                    };

                    coordenadores.Add(coordenador);
                }
            }
            return coordenadores;
        }

        private async Task<ICollection<FuncionarioCargo>> BuscarFuncionariosCargo(string pCodigoEscola, int pCodigoCargo)
        {
            HttpClient httpClient = new HttpClient()
            {
                BaseAddress = new Uri(this.urlApiEol)
            };

            string jsonTurmasString = await httpClient.GetStringAsync($"api/escolas/{pCodigoEscola}/funcionarios/cargos/{pCodigoCargo}");
            return JsonConvert.DeserializeObject<ICollection<FuncionarioCargo>>(jsonTurmasString);
        }

    }
}