using AppCopaHAS.Models;
using AppCopaHAS.Services;
using Azure.Storage.Blobs;
using CopaHAS.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;

namespace AppCopaHAS.ViewModels
{
    public class AlbumViewModel : BaseViewModel
    {
        private SelecaoService _selecaoService;
        private JogadorService _jogadorService;
        public ObservableCollection<Selecao> Selecoes { get; set; }
        public ObservableCollection<Jogador> Jogadores { get; set; }
        public AlbumViewModel()
        {

            _selecaoService = new SelecaoService();
            _jogadorService = new JogadorService();

            Selecoes = new ObservableCollection<Selecao>();
            Jogadores = new ObservableCollection<Jogador>();

            _ = ObterSelecoes();

            /*SelecionarFotoCommand = new Command<Jogador>(async (jogador) => 
            { 
                await SelecionarFoto(jogador); 
            });*/
        }

        //public ICommand SelecionarFotoCommand;



        public async Task ObterSelecoes()
        {
            try
            {
                Selecoes = await _selecaoService.GetSelecoesAsync();
                OnPropertyChanged(nameof(Selecoes));
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage
                    .DisplayAlertAsync("Ops", ex.Message, "Detalhes" + ex.InnerException, "Ok");
            }
        }

        public async Task ObterJogadores(int selecaoId)
        {
            try
            {
                var jogadoresApi = await _jogadorService.GetJogadoresAsync();
                Jogadores.Clear();

                foreach (var jogador in jogadoresApi.Where(x => x.SelecaoId == selecaoId))
                {
                    string fileName = $"{SelecaoSelecionada.Pais}-{jogador.Nome}.png";
                    var blobClient = new BlobClient(_conexaoAzureBlobStorage, _container, fileName);
                    if (blobClient.Exists())
                    {
                        Byte[] fileBytes;
                        using (MemoryStream ms = new MemoryStream())
                        {
                            blobClient.OpenRead().CopyTo(ms);
                            fileBytes = ms.ToArray();
                        }
                        jogador.Foto = fileBytes;
                    }
                    Jogadores.Add(jogador);
                }
                OnPropertyChanged(nameof(Jogadores));
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage
                    .DisplayAlertAsync("Ops", ex.Message, "Detalhes" + ex.InnerException, "Ok");
            }
        }

        private Selecao selecaoSelecionada;
        public Selecao SelecaoSelecionada
        {
            get => selecaoSelecionada;
            set
            {
                selecaoSelecionada = value;
                OnPropertyChanged();

                if (value != null)
                    _ = ObterJogadores(value.Id);
            }
        }
       
        private static string _conexaoAzureBlobStorage = "DefaultEndpointsProtocol=https;AccountName=etecstorageluiz2;AccountKey=iIWyQJGVBeBGJFqUOW7LlfeWo8XUUhlh1TRM5XlamYQZ4EcxDlG1Q/LaOaKjfqmg7dViqzlsmvyD+AStYcm2CA==;EndpointSuffix=core.windows.net";
        //private static string _conexaoAzureBlobStorage = "COLE a chave de acesso da conta de armazenamento";
        private static string _container = "arquivos";
        private async Task SelecionarFoto(Jogador jogador)
        {
            try
            {
                var fotos = await MediaPicker.Default.PickPhotosAsync();
                var foto = fotos?.FirstOrDefault();
                if (foto == null)
                    return;

                var extensao = Path.GetExtension(foto.FileName);

                if (!string.Equals(extensao, ".png", StringComparison.OrdinalIgnoreCase))
                {
                    await Application.Current.MainPage.DisplayAlertAsync(
                        "Formato inválido", "Selecione uma imagem PNG.", "OK");
                    return;
                }

                string msg = $"Deseja salvar a imagem para {jogador.Nome} - {selecaoSelecionada.Pais}";
                if (!await Application.Current.MainPage.DisplayAlertAsync("Mensagem", msg, "Sim", "Não"))
                    return;

                await using var stream = await foto.OpenReadAsync();        
                string fileName = $"{SelecaoSelecionada.Pais}-{jogador.Nome}.png";//Criando nome da imagem
                var blobClient = new BlobClient(_conexaoAzureBlobStorage, _container, fileName);

                if (blobClient.Exists())//Verifica se tem arquivo com nome igual e remove caso exista.
                    blobClient.Delete();

                blobClient.Upload(stream);//Salvando a imagem no container de imagem do Azure
                OnPropertyChanged(nameof(Jogadores));
                await Application.Current.MainPage
                    .DisplayAlertAsync("Mensagem", "Imagem salva com sucesso.", "Ok");

                _ = ObterJogadores(selecaoSelecionada.Id);//Atualizará os cards com a imagem salva.
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage
                    .DisplayAlertAsync("Ops", ex.Message, "Detalhes" + ex.InnerException, "Ok");
            }
        }

        public ICommand SelecionarFotoCommand => new Command<Jogador>(async (jogador) =>
        {
            await SelecionarFoto(jogador);
        });









    }
}
