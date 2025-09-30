using MauiAppTempoAgora.Models;
using MauiAppTempoAgora.Services;
using System.Threading.Tasks;

namespace MauiAppTempoAgora
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
        }
        private async void Button_Clicked_Previsao(object sender, EventArgs e)
        {
           try
            {
                if (!string.IsNullOrEmpty(txt_cidade.Text)) // Se estiver nulo/vazio, fazer a consulta 
                {
                    Tempo? t = await DataService.GetPrevisao (txt_cidade.Text);

                    if (t != null)
                    {
                        string dados_previsao = "";
                        dados_previsao =
                            $"Latitude: {t.lat} \n " +
                            $"Longitude: {t.lon} \n" +
                            $"Nascer do Sol: {t.sunrise} \n" +
                            $"Por do Sol: {t.sunset} \n" +
                            $"Temp Máx: {t.temp_max} \n" +
                            $"Temp Mín: {t.temp_min} \n" +
                            $"Descrição: {t.description} \n" +
                            $"Velocidade do vento: {t.speed} \n" +
                            $"Visibilidade: {t.visibility} \n";


                        lbl_res.Text = dados_previsao;


                    }
                    else
                    {
                        lbl_res.Text = "Sem dados de previsão";
                    }
                } else
                {
                    lbl_res.Text = "Preencher a cidade"; 
                }
            } catch (Exception ex)
            {

                await DisplayAlert("Ops", ex.Message, "OK");

            }
        }

        private async void Button_Clicked_Localizacao(object sender, EventArgs e)
        {
            try
            {
                GeolocationRequest request = new GeolocationRequest(
                        GeolocationAccuracy.Medium, 
                        TimeSpan.FromSeconds(10) // Tenta Procurar por 10 segundos 
                    );

                Location? local = await Geolocation.Default.GetLocationAsync(request); //Geolocation.Deafult: Acesso ao GPS do disositivo //GetLocationAsync: Tenta pegar a a localização atual//Location? local: guarda os dados, pode ser 'null' se não conseguir 

                if (local != null) // Se o local não for nulo 
                {
                    string local_disp = $"Latitude: {local.Latitude} \n " +
                                        $"Longitude: {local.Longitude} \n";

                    lbl_coords.Text = local_disp; // Indo atrás da localização

                    // Nome da cidade que está nas coordenadas 
                    GetCidade(local.Latitude, local.Longitude);
                }
                else
                {
                    lbl_coords.Text="Nenhuma loclaização";
                }

            }
            catch (FeatureNotSupportedException fnsEx) // Não tem o suporte para a localização 
            {
                await DisplayAlert("Erro: Dispositivo não suporta", fnsEx.Message, "OK");

            }
            catch (FeatureNotEnabledException fneEx)
            {
                await DisplayAlert("Erro: Localização Desabilitada", fneEx.Message, "OK");
            }
            catch (PermissionException pEx)
            {
                await DisplayAlert("Erro: Permissão da Localização", pEx.Message, "OK"); 
            }
            catch(Exception ex) //catch genérico
            {
                await DisplayAlert("Erro", ex.Message, "OK");
            }

        }

        private async void GetCidade(double lat, double lon) // retorna preenchimento automático qual cidade eu estou 
        { //Habilite o try/catch para não crachear!!! - Habilite oo Token em depuração Windows
            try { 
            // Conceito Placemark 
            IEnumerable<Placemark> places = await Geocoding.Default.GetPlacemarksAsync(lat, lon);
            
            Placemark? place = places.FirstOrDefault();  

            if (place != null)
            {
                txt_cidade.Text = place.Locality;
            }
            } catch (Exception ex)
            {
                await DisplayAlert("Erro: Obtenção do nome da cidade", ex.Message, "OK");
            }
        }
    }

}
