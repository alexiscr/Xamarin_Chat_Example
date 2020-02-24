using Acr.UserDialogs;
using FreshMvvm;
using System;
using TheChat.Core.Services;
using TheChat.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TheChat
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            //MainPage = new MainPage();

            ConfigureContainer();

            var loginPage = FreshPageModelResolver.ResolvePageModel<LoginViewModel>();

            var navPage = new FreshNavigationContainer(loginPage);

            MainPage = navPage;
        }

        /// <summary>
        ///  Método par la configuarción del contenedor IoC de FreshMVVM
        /// </summary>
        private void ConfigureContainer()
        {
            // Registro de la servicio a inyectar para el manejo del chat
            FreshIOC.Container.Register<IChatService, ChatService>();
            // Registrar una instancia de User Dialog
            FreshIOC.Container.Register<IUserDialogs>(UserDialogs.Instance); 
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
