using Acr.UserDialogs;
using CoreInvestmentApp.Classes;
using Plugin.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CoreInvestmentApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FeedbackPage : ContentPage
    {
        public FeedbackPage()
        {
            InitializeComponent();

            var emailTask = CrossMessaging.Current.EmailMessenger;
            if (!emailTask.CanSendEmail)
            {
                string message = string.Format("Kindly send an e-mail to {0}, we will get back to your shortly.", Util.ContactEmail);
                UserDialogs.Instance.Alert(message, null, "OK");

                EntrySubject.IsEnabled = false;
                EditorMessage.IsEnabled = false;
                ButtonSend.IsEnabled = false;
            }
            else
            {
                LabelEmailAddress.Text = Util.ContactEmail;
            }
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            var emailTask = CrossMessaging.Current.EmailMessenger;
            var email = new EmailMessageBuilder()
              .To(Util.ContactEmail)
              .Subject(EntrySubject.Text)
              .Body(EditorMessage.Text)
              .Build();

            emailTask.SendEmail(email);
        }
    }
}