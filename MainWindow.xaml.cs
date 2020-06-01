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
using System.IO;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MyFinalProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    
    public partial class MainWindow : Window
    {
        List<ElAppliance> appliances = new List<ElAppliance>();
        List<ElAppliance> cart = new List<ElAppliance>();
        List<ComboBox> FBoxes = new List<ComboBox>();
        public MainWindow()
        {
            int numberOfGoods = File.ReadAllLines("Storage.txt").Length - 1;
            using (StreamReader reader = new StreamReader("Storage.txt"))
            {
                string tmpAppliances;
                string[] separated;
                string[] type = new string[numberOfGoods];

                for (int i = 0; i < numberOfGoods; ++i)
                {
                    tmpAppliances = reader.ReadLine();
                    char splitter = '|';
                    separated = tmpAppliances.Split(splitter);
                    try
                    {
                        if (separated[0].StartsWith("VacuumCleaner"))
                        {
                            appliances.Add(new VacCleaner(separated[1], separated[2], Convert.ToDouble(separated[3]),
                                Convert.ToDouble(separated[4]), separated[5]));
                        }
                        else if (separated[0].StartsWith("WashingMachine"))
                        {
                            appliances.Add(new WashingMachine(separated[1], separated[2], Convert.ToDouble(separated[3]),
                                Convert.ToInt32(separated[4]), Convert.ToDouble(separated[5])));
                        }
                        else
                        {
                            appliances.Add(new FoodProcessor(separated[1], separated[2], Convert.ToDouble(separated[3]),
                                Convert.ToDouble(separated[4]), Convert.ToInt32(separated[5])));
                        }
                        type[i] = separated[0];
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"{ex.Message}");
                    }

                }
            }
            for (int i = 0; i < appliances.Count; i++)
            {
                for (int j = i + 1; j < appliances.Count; j++)
                {
                    if(appliances[i].Name == appliances[j].Name && appliances[j].Number == 1)
                    {
                        appliances[j].Number++;
                        appliances.Remove(appliances[i]);
                    }
                }
            }
            appliances.Sort();
            InitializeComponent();
            foreach (var appliance in appliances)
            {
                if(appliance.GetType() == typeof(VacCleaner))
                    vacuumGrid.Items.Add(appliance);
                if (appliance.GetType() == typeof(FoodProcessor))
                    foodGrid.Items.Add(appliance);
                if (appliance.GetType() == typeof(WashingMachine))
                    washingGrid.Items.Add(appliance);
                cart.Add(appliance);
            }
            FBoxes.Add(VCbBox1);
            FBoxes.Add(VCbBox2);
            FBoxes.Add(FCbBox1);
            FBoxes.Add(FCbBox2);
            FBoxes.Add(WCbBox1);
            FBoxes.Add(WCbBox2);
            int cc = 0;
            foreach (var appliance in appliances)
            {
                if (appliance.GetType() == typeof(VacCleaner))
                {
                    for (int i = 0; i <= appliance.Number; i++)
                    {
                        FBoxes[cc].Items.Add(Convert.ToString(i));
                    }
                    cc++;
                }
                if (appliance.GetType() == typeof(FoodProcessor))
                {
                    for (int i = 0; i <= appliance.Number; i++)
                    {
                        FBoxes[cc].Items.Add(Convert.ToString(i));   
                    }
                    cc++;
                }
                if (appliance.GetType() == typeof(WashingMachine))
                {
                    for (int i = 0; i <= appliance.Number; i++)
                    {
                        FBoxes[cc].Items.Add(Convert.ToString(i));
                    }
                    cc++;
                }
            }
        }
        private void CheckOrder()
        {
            string order = "";
            List<int> numbOfOrder = new List<int>();
            int cartLength = cart.Count;
            string numb;
            double totalPrice = 0;
            for (int i = 0; i < cartLength; ++i)
            {
                numb = Convert.ToString(FBoxes[i].SelectedItem);
                if (numb != "0")
                {
                    order += numb + " " + cart[i].Company +" "+cart[i].Name + "\n";
                    totalPrice += cart[i].Price * Convert.ToInt32(numb);
                }
            }
            if (order != "")
            {
                string message = "Your Order:\n " + order + "\nTotal Price : " + totalPrice + "$";
                const string caption = "Cart";
                MessageBoxButton buttons = MessageBoxButton.YesNo;
                var res = (System.Windows.Forms.DialogResult)MessageBox.Show(message, caption, buttons, MessageBoxImage.Question);
                if (res == System.Windows.Forms.DialogResult.Yes)
                {
                    MessageBox.Show("Our manager will call you soon. Thanks!", "Confirmed", MessageBoxButton.OK);
                    this.Close();
                    using (StreamWriter writer = new StreamWriter("order.txt",false))
                    {
                        writer.WriteLine(order);
                        writer.WriteLine($"\nTotal Price: {totalPrice}");   
                    }
                }
                else
                {
                    MessageBox.Show("Correct your order", "Error",MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("You didn't add any item to cart!", "Empty cart", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CheckOrder();
        }
    }
    public abstract class ElAppliance : IComparable<ElAppliance>
    {

        string name;
        string company;
        double price;
        int number = 1;
        public string Name { get { return this.name; } }
        public string Company { get { return this.company; } }
        public double Price { get { return this.price; } }
        public int Number { get { return this.number; } set { this.number = value; } }
        public ElAppliance(string name, string company, double price)
        {
            this.name = name;
            this.company = company;
            this.price = price;
        }
        public int CompareTo(ElAppliance other)
        {
            return this.name.CompareTo(other.name);
        }
        public override string ToString() => ($"Name: {name} | Company : {company} | Price: {price}");

    }
    public class VacCleaner : ElAppliance
    {

        double power;
        string color;
        public double Power { get { return this.power; } }
        public string Color { get { return this.color; } }
        public VacCleaner(string name, string company, double price, double power, string color) : base(name, company, price)
        {
            this.power = power;
            this.color = color;
        }
        public override string ToString()
        {
            return base.ToString() + ($" | Power: {power} | Color: {color}");
        }

    }
    public class WashingMachine : ElAppliance
    {
        int numbOfPrograms;
        double volume;
        public int NumbOfPrograms { get { return this.numbOfPrograms; } }
        public double Volume { get { return this.volume; } }
        public WashingMachine(string name, string company, double price, int numbOfPrograms, double volume) : base(name, company, price)
        {
            this.numbOfPrograms = numbOfPrograms;
            this.volume = volume;
        }
        public override string ToString()
        {
            return base.ToString() + ($" | Number of Programs: {numbOfPrograms} | Volume: {volume}");
        }
    }
    class FoodProcessor : ElAppliance
    {
        double power;
        int numbOfFuncs;
        public double Power
        {
            get
            {
                return this.power;
            }
        }
        public int NumbOfFuncs
        {
            get
            {
                return this.numbOfFuncs;
            }
        }
        public FoodProcessor(string name, string company, double price, double power, int numbOfFuncs) : base(name, company, price)
        {
            this.power = power;
            this.numbOfFuncs = numbOfFuncs;
        }
        public override string ToString()
        {
            return base.ToString() + ($" | Power: {power} | Number of Functions: {numbOfFuncs}");
        }
    }

}
