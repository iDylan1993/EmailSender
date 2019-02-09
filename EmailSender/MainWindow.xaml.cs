using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Net.Mail;
using System.Windows;
using System.Windows.Media;

namespace EmailSender
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

       OpenFileDialog FileD = new OpenFileDialog();
       BackgroundWorker BackgroundWorker = new BackgroundWorker();
       string Host;
       string EmailFrom;
       string EmailTo;
       string Password;
       string Subject;
       string Message;
       string AttachmentPath;
       string ErrorSmtp;
       string ErrorFormat;
        private void SendEmail_Click(object sender, RoutedEventArgs e)
        {
            if (HostTextbox.Text == string.Empty)
            {
                System.Windows.MessageBox.Show("Please fill in the hosting provider.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                HostTextbox.Focus();
            }
            else if (EmailTextbox.Text == string.Empty)
            {
                System.Windows.MessageBox.Show("Please enter your email address.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                EmailTextbox.Focus();
            }
            else if (EmailTextbox.Text.Contains("@") == false)
                {
                System.Windows.MessageBox.Show("Please enter a valid email address.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                EmailTextbox.Focus();
            }
            else if (PasswordBox.Password == string.Empty)
            {
                System.Windows.MessageBox.Show("Please fill in your password.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                PasswordBox.Focus();
            }
            else if (EmailToTextbox.Text == string.Empty)
            {
                System.Windows.MessageBox.Show("Please Enter the email address where you want to send the mail.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                EmailToTextbox.Focus();
            }
            else if  (EmailTextbox.Text.Contains("@") == false)
                {
                System.Windows.MessageBox.Show("Please enter a valid email address.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                EmailToTextbox.Focus();
            }
            else if (SubjectTextbox.Text == string.Empty)
            {
                System.Windows.MessageBox.Show("Please fill in the subject of your email.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                SubjectTextbox.Focus();
            }
            else if (MessageTextbox.Text == string.Empty)
            {
                System.Windows.MessageBox.Show("Please leave a message.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                MessageTextbox.Focus();
            }
            else
            {
                SendEmail.IsEnabled = false;
                if (HostTextbox.Text.StartsWith("pop3"))
                {
                    HostTextbox.Text = HostTextbox.Text.Replace("pop3", "smtp");
                }
                else if (HostTextbox.Text.StartsWith("imap"))
                {
                    HostTextbox.Text = HostTextbox.Text.Replace("imap", "smtp");
                }
                else if (!(HostTextbox.Text.Contains("smtp.")))
                HostTextbox.Text = "smtp." + HostTextbox.Text;
                ErrorFormat = String.Empty;
                ErrorSmtp = String.Empty;
                Host = HostTextbox.Text;
                EmailFrom = EmailTextbox.Text;
                EmailTo = EmailToTextbox.Text;
                Password = PasswordBox.Password;
                Subject = SubjectTextbox.Text;
                Message = MessageTextbox.Text;
                AttachmentPath = AttachmentTextbox.Text;
                StatusTextBlock.Foreground = Brushes.Black;
                StatusTextBlock.Text = "Sending...";
                BackgroundWorker.RunWorkerAsync();           
            }
        }

        private void HostTextbox_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            HostTextbox.SelectAll();
        }

        private void EmailTextbox_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            EmailTextbox.SelectAll();
        }

        private void PasswordBox_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            PasswordBox.SelectAll();
        }

        private void EmailToTextbox_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            EmailToTextbox.SelectAll();
        }

        private void SubjectTextbox_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SubjectTextbox.SelectAll();
        }

        private void MessageTextbox_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MessageTextbox.SelectAll();
        }

        private void AttachmentAdd_Click(object sender, RoutedEventArgs e)
        {
            FileD.ShowDialog();
            AttachmentTextbox.Text = Convert.ToString(FileD.FileName);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            BackgroundWorker.DoWork += BackgroundWorker_DoWork;
            BackgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
            HostTextbox.Focus();
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                MailMessage Email = new MailMessage();
                SmtpClient Client = new SmtpClient();
                Client.Host = Host;
                Client.Timeout = 10000;
                Client.EnableSsl = true;
                Client.DeliveryMethod = SmtpDeliveryMethod.Network;
                Client.UseDefaultCredentials = false;
                Client.Credentials = new System.Net.NetworkCredential(EmailFrom, Password);
                Email.From = new MailAddress(EmailFrom);
                Email.To.Add(new MailAddress(EmailTo));
                Email.Subject = Subject;
                Email.Body = Message;
                if (AttachmentPath != string.Empty)
                {
                    System.Net.Mail.Attachment Attachment;
                    Attachment = new System.Net.Mail.Attachment(AttachmentPath);
                    Email.Attachments.Add(Attachment);
                    Client.Send(Email);
                }
                if (AttachmentPath == string.Empty)
                {
                    Client.Send(Email);
                }
            }
            catch (SmtpException ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                ErrorSmtp = ex.Message;
            }
            catch (FormatException ex)
            {
                System.Windows.MessageBox.Show("Please enter a valid email address.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                ErrorFormat = ex.Message;
            }
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (ErrorFormat != string.Empty)
            {
                StatusTextBlock.Foreground = Brushes.Red;
                StatusTextBlock.Text = "E-mail not send.";
                SendEmail.IsEnabled = true;
            }
            else if (ErrorSmtp != string.Empty)
            {
                StatusTextBlock.Foreground = Brushes.Red;
                StatusTextBlock.Text = "E-mail not send.";
                SendEmail.IsEnabled = true;
            }
            else
            {
                StatusTextBlock.Foreground = Brushes.Green;
                StatusTextBlock.Text = "Email send.";
                System.Windows.MessageBox.Show("Your mail has been sent successfully.", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                SendEmail.IsEnabled = true;
                HostTextbox.Text = String.Empty;
                EmailTextbox.Text = String.Empty;
                EmailToTextbox.Text = String.Empty;
                SubjectTextbox.Text = String.Empty;
                MessageTextbox.Text = String.Empty;
                PasswordBox.Password = String.Empty;
            }
            }  
    }
    }

