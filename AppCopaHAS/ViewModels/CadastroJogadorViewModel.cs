using AppCopaHAS.Models;
using AppCopaHAS.Services;
using CopaHAS.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;

namespace AppCopaHAS.ViewModels
{
    public class CadastroJogadorViewModel : BaseViewModel
    {
        private JogadorService _jogadorService;
        private SelecaoService _selecaoService;
        public ObservableCollection<Selecao> Selecoes { get; set; }

        public CadastroJogadorViewModel()
        {
            _jogadorService = new JogadorService();
            _selecaoService = new SelecaoService();

            Selecoes = new ObservableCollection<Selecao>();
            _ = ObterSelecoes();
            SalvarCommand = new Command(async () => { await SalvarJogador(); });
        }
        public ICommand SalvarCommand { get; set; }

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

        private Selecao selecaoSelecionada;
        public Selecao SelecaoSelecionada
        {
            get => selecaoSelecionada;
            set
            {
                if (value != null)
                {
                    selecaoSelecionada = value;
                    OnPropertyChanged();
                }
            }
        }

        private int id;
        public int Id
        {
            get => id;
            set
            {
                id = value;
                OnPropertyChanged();
            }
        }

        private string nome;
        public string Nome
        {
            get => nome;
            set
            {
                nome = value;
                OnPropertyChanged();
            }
        }

        private int numeroCamisa;
        public int NumeroCamisa
        {
            get => numeroCamisa;
            set
            {
                numeroCamisa = value;
                OnPropertyChanged();
            }
        }

        private string posicao;
        public string Posicao
        {
            get => posicao;
            set
            {
                posicao = value;
                OnPropertyChanged();
            }
        }

        public async Task SalvarJogador()
        {
            try
            {
                Jogador j = new Jogador();
                j.SelecaoId = selecaoSelecionada.Id;
                j.Nome = this.Nome;
                j.Posicao = this.Posicao;
                j.NumeroCamisa = this.NumeroCamisa;                

                if (j.Id == 0)
                {
                    Jogador jogadorRetorno = await _jogadorService.PostJogadorAsync(j);
                    await Application.Current.MainPage.DisplayAlertAsync("Mensagem", "Dados salvos com sucesso!", "Ok");
                }                
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlertAsync("Ops", ex.Message + " Detalhes: " + ex.InnerException, "Ok");
            }
        }







    }
}


