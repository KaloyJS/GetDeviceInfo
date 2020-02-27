using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using GetDeviceInfo;
using Microsoft.Xaml.Behaviors.Core;

namespace GetDeviceInfo
{
    public class MainViewModel
    {

        //public ICommand Command { get; }=new Command();


    }

    public class Command : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            MessageBox.Show((parameter as DeviceChangeEventArgs)?.DeviceChange.ToString());
        }
    }
}
