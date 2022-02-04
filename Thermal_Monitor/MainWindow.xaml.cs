
using LibreHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Windows.Threading;

namespace Thermal_Monitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    
    
    
    
    public partial class MainWindow : Window
    {
        DispatcherTimer dtimer_refresh_data = new DispatcherTimer();

        

        private void dtimer_refresh_data_Tick(object sender, EventArgs e)
        {
            dtimer_refresh_data.Stop();
            Computer computer = new Computer
            {
                IsCpuEnabled = true,
                IsGpuEnabled = true,
                IsMemoryEnabled = true,
                IsMotherboardEnabled = true,
                IsControllerEnabled = true,
                IsNetworkEnabled = true,
                IsStorageEnabled = true
            };
            computer.Open();
            computer.Accept(new UpdateVisitor());

            //label_cpu_tem.Content = computer.Hardware.GetType().Name;
            foreach (IHardware hardware in computer.Hardware)
            {
                if (hardware.HardwareType == HardwareType.Cpu)
                {
                    test_out.Text += ("HardwareType:", HardwareType.Cpu, "\n");
                    foreach (IHardware subhardware in hardware.SubHardware)
                    {
                        test_out.Text += ("SubHardware:", hardware.SubHardware, "\n");
                        if (subhardware.HardwareType == HardwareType.Cpu)
                        {
                            test_out.Text += ("\tSubhardware: {0}", subhardware.Name);
                            foreach (ISensor sensor in subhardware.Sensors)
                            {
                                //if(sensor.Name == "CPU Package")
                                test_out.Text += ("\t\tSensor: {0}, value: {1}", sensor.Name, sensor.Value);
                            }
                        }
                    }
                    foreach (ISensor sensor in hardware.Sensors)
                    {
                        if(sensor.SensorType == SensorType.Temperature)
                        test_out.Text += ("\tSensor:", sensor.Value, "\n");
                    }
                }
                

                
            }
            computer.Close();
        }

        public class UpdateVisitor : IVisitor
        {
            public void VisitComputer(IComputer computer)
            {
                computer.Traverse(this);
            }
            public void VisitHardware(IHardware hardware)
            {
                hardware.Update();
                foreach (IHardware subHardware in hardware.SubHardware) subHardware.Accept(this);
            }
            public void VisitSensor(ISensor sensor) { }
            public void VisitParameter(IParameter parameter) { }
        }

        public MainWindow()
        {
            InitializeComponent();

            

            

            dtimer_refresh_data.Tick += new EventHandler(dtimer_refresh_data_Tick);
            dtimer_refresh_data.Interval = new TimeSpan(0, 0, 1);
            dtimer_refresh_data.Start();


        }
    }
}
