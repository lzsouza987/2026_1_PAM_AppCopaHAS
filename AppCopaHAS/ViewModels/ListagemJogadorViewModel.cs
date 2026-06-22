using AppCopaHAS.Models;
using AppCopaHAS.Services;
using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace AppCopaHAS.ViewModels
{
    public class ListagemJogadorViewModel : BaseViewModel
    {
        private JogadorService _jogadorService;
        public ObservableCollection<Jogador> Jogadores { get; set; }

        public ListagemJogadorViewModel()
        {
            _jogadorService = new JogadorService();
            Jogadores = new ObservableCollection<Jogador>();

            _ = ObterJogadores();
        }

        public async Task ObterJogadores()
        {
            try
            {
                Jogadores = await _jogadorService.GetJogadoresAsync();                
                OnPropertyChanged(nameof(Jogadores));
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage
                    .DisplayAlertAsync("Ops", ex.Message, "Detalhes" + ex.InnerException, "Ok");
            }
        }



    }
}
