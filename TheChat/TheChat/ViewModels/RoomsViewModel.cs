using Acr.UserDialogs;
using FreshMvvm;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using TheChat.Core.Models;
using TheChat.Core.Services;
using Xamarin.Forms;

namespace TheChat.ViewModels
{
    public class RoomsViewModel : FreshBasePageModel
    {
        IChatService ChatService;
        IUserDialogs dialogs;
        bool IsBusy = false;
        string UserName;
        public List<Room> Rooms { get; set; }
        public Room CurrentRoom { get; set; }
        public ICommand EnterRoomCommand { get; set; }

        public RoomsViewModel(IChatService _chatservice, IUserDialogs _dialogs)
        {
            ChatService = _chatservice;
            dialogs = _dialogs;
        }
        public override void Init(object initData)
        {
            base.Init(initData);
            UserName = initData as string;

            EnterRoomCommand = new Command(async () => {

                if (!IsBusy)
                {
                    IsBusy = true;

                    if (CurrentRoom != null)
                    {
                        Tuple<string, string> data = 
                            new Tuple<string, string>(UserName, CurrentRoom.Name);

                        await CoreMethods.PushPageModel<ChatViewModel>(data);
                        
                        CurrentRoom = null;
                    }
                    IsBusy = false;
                }
            });
        }
        protected override async void ViewIsAppearing(object sender, EventArgs e)
        {
            base.ViewIsAppearing(sender, e);
            dialogs.ShowLoading("Loading");
            
            Rooms = await ChatService.GetRooms();

            dialogs.HideLoading();
        }
    }
}
