using Entity.Dtos;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Interfaces;

namespace Utilities.Implementation
{
    public class CryptoApiService : ICryptoApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _endpoint;

        public CryptoApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _endpoint = configuration["CryptoApi:Endpoint"];
        }

        public async Task<List<ExternalCryptoDto>> GetTopCryptosAsync(int count)
        {
            try
            {
                // Reemplazamos el per_page por el parámetro count para ser dinámico
                var url = _endpoint.Replace("per_page=5", $"per_page={count}");

                var response = await _httpClient.GetStringAsync(url);
                return JsonConvert.DeserializeObject<List<ExternalCryptoDto>>(response);
            }
            catch (HttpRequestException httpEx)
            {
                // Aquí puedes loguear el error con un logger si lo usas
                throw new ApplicationException("Error al hacer la solicitud HTTP a la API de CoinGecko.", httpEx);
            }
            catch (JsonException jsonEx)
            {
                throw new ApplicationException("Error al deserializar la respuesta de la API de CoinGecko.", jsonEx);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error inesperado al obtener datos de criptomonedas.", ex);
            }
        }
    }
 }
