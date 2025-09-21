using System;
using System.Net;
using System.Net.Http;
using  System.Threading.Tasks;
using MauiAppTempoAgora.Models;
using Newtonsoft.Json.Linq;

namespace MauiAppTempoAgora.Services
{
    public class DataService
    {
        public static async Task<Tempo?> GetPrevisao(string cidade)
        {
            Tempo? t = null;

            string chave = "1e503eb395ab4df1cf94fb71bfd7900f";

            string url = $"https://api.openweathermap.org/data/2.5/weather?" +
                         $"q={cidade}&appid={chave}&units=metric&lang=pt_br";


            using (HttpClient client = new HttpClient())
            {
                try // tenta fazer a requisição 
                {

                    HttpResponseMessage resp = await client.GetAsync(url); // enquanto a url for responder  a resposta do json=cabeçalho do http ficará armazenada na variável 'resp' de maneira assíncrona

                    if (resp.IsSuccessStatusCode)
                    {

                        string json = await resp.Content.ReadAsStringAsync();

                        var rascunho = JObject.Parse(json);

                        DateTime time = new();
                        DateTime sunrise = time.AddSeconds((double)rascunho["sys"]["sunrise"]).ToLocalTime();
                        DateTime sunset = time.AddSeconds((double)rascunho["sys"]["sunset"]).ToLocalTime();

                        t= new()
                        {
                            lat= (double)rascunho["coord"]["lat"],
                            lon= (double)rascunho["coord"]["lon"],
                            description= (string)rascunho["weather"][0]["description"],
                            main = (string)rascunho["weather"][0]["main"],
                            temp_min =  (double)rascunho["main"]["temp_min"],
                            temp_max= (double)rascunho["main"]["temp_max"],
                            speed = (double)rascunho["wind"]["speed"],
                            visibility = (int)rascunho["visibility"],
                            sunrise = sunrise.ToString(),
                            sunset = sunset.ToString(),
                        };// Fecha objeto do Tempo                  
                    } // Fecha 'if' se o status do servidor foi de sucesso

                    //------------------------------------------------- Parte 02- 01: 

                    else if (resp.StatusCode == HttpStatusCode.NotFound)
                    {
                        // Cidade não encontrada
                        throw new Exception("Cidade não encontrada. Verifique o nome e tente novamente. ");

                        //  Se o status for 404, significa que a cidade não foi encontrada na API. 

                    } // Fecha 'else if para cidade nao encontrada.
                      //------------------------------------------------
                } //Fecha laço using 

                //------------------------------------------------- Parte 02- 02: 

                catch (HttpRequestException)  // captura erros de rede (sem internet...) 
                {
                    //erro de rede/sem conexão
                    throw new Exception(" Sem conexão com a internet. Por favor verifique sua rede. "); // Envia a msg para o 'MainPage.xaml.cs' que já mostra com DisplayAlert.
                }
            }
            //------------------------------------------------
            return t; 
        }
    }
}
 