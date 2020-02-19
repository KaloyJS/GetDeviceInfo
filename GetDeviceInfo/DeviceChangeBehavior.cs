using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using GetDeviceInfo.Enum;
using GetDeviceInfo.EventArgs;
using Microsoft.Xaml.Behaviors;

namespace GetDeviceInfo
{
    public class DeviceChangeBehavior : Behavior<Window>
    {
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(DeviceChangeBehavior), new PropertyMetadata(null));
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Initialized += AssociatedObject_Initialized;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.Initialized -= AssociatedObject_Initialized;
            base.OnDetaching();
        }

        private void AssociatedObject_Initialized(object sender, System.EventArgs e)
        {
            (sender as MainWindow).DeviceChanged += DeviceChangeBehavior_DeviceChanged;

        }

        private void DeviceChangeBehavior_DeviceChanged(object sender, DeviceChange e)
        {
            ExecuteCommand(new DeviceChangeEventArgs {DeviceChange = e});
        }




        // ReSharper disable once UnusedMember.Local
        // ReSharper disable once UnusedParameter.Local
        private void ExecuteCommand(DeviceChangeEventArgs e)
        {
            if (Command?.CanExecute(e) == true)
            {
                Command.Execute(e);
            }
        }
    }

  
}
