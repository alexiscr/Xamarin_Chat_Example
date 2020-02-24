using Acr.UserDialogs;
using FreshMvvm;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using TheChat.Core.Services;
using Xamarin.Forms;

namespace TheChat.ViewModels
{
    public class LoginViewModel : FreshBasePageModel
    {
        // Propiedades del viewmodel
        public string UserName { get; set; }
        public bool IsBusy { get; set; }
        public ICommand ConnectCommand => new Command(async () => {
            if (!IsBusy)
            {
                IsBusy = true;

                dialogs.ShowLoading("Connecting");

                // Tareas a procesar
                await ChatService.InitAsync(UserName);

                await CoreMethods.PushPageModel<RoomsViewModel>(UserName);
                // Fin Tareas
                dialogs.HideLoading();

                IsBusy = false;
            }
        });

        IChatService ChatService;
        IUserDialogs dialogs;
        public LoginViewModel(IChatService _chatService, IUserDialogs _userDialogs)
        {
            ChatService = _chatService;
            dialogs = _userDialogs;
        }
    }
}
