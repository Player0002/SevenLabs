using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.ObjectModel;
using System.Windows.Data;
using System.ComponentModel;
using System.Windows;

namespace Sevenlabs
{
    public class FilamentViewModel : INotifyPropertyChanged
    {
        private readonly object _lock = new object();

        private FilamentRepository repository = new FilamentRepositoryImpl();

        private ObservableCollection<FilamentModel> _filament;

        public ObservableCollection<FilamentModel> Filament {
            get { return _filament;  }
            set {
                _filament = value;
                Creators = new ObservableCollection<string>(Filament.Select(a => a.Creator).Distinct());
                Types = new ObservableCollection<string>(Filament.Select(a => a.Type).Distinct());
                Colors = new ObservableCollection<string>(Filament.Select(a => a.Color).Distinct());
                Codes = new ObservableCollection<string>(Filament.Select(a => a.Code).Distinct());
                onPropertyChanged("Filament");
            }
        }

        private ObservableCollection<string> creators;

        private ObservableCollection<string> types;

        private ObservableCollection<string> colors;

        private ObservableCollection<string> codes;

        public ObservableCollection<string> Creators { 
            get
            {
                return creators;
            }
            set {
                creators = value;
                onPropertyChanged("Creators");
            }
        }
        public ObservableCollection<string> Types
        {
            get
            {
                return types;
            }
            set
            {
                types = value;
                onPropertyChanged("Types");
            }
        }
        public ObservableCollection<string> Colors
        {
            get
            {
                return colors;
            }
            set
            {
                colors = value;
                onPropertyChanged("Colors");
            }
        }
        public ObservableCollection<string> Codes
        {
            get
            {
                return codes;
            }
            set
            {
                codes = value;
                onPropertyChanged("Codes");
            }
        }


        private FilamentModel selectedFilament = new FilamentModel("","", "", "", 0, "", 0);
        public FilamentModel SelectedFilament {
            get { return selectedFilament; }
            set {
                selectedFilament = value;
                InputColor = value.Color;
                InputCount = value.Count;
                InputCreator = value.Creator;
                InputType = value.Type;
                InputWeight = value.Weight;
                onPropertyChanged("SelectedFilament"); 
            }
        }

        private double selectedWeight;

        public double SelectedWeight {
            get { return selectedWeight; }
            set { selectedWeight = value; onPropertyChanged("SelectedWeight"); }
        }

        private string selectedCode;

        public string SelectedCode {
            get { return selectedCode; }
            set { 
                selectedCode = value;
                SelectedCreator = value.Substring(0, 3);
                SelectedType = value.Substring(3, 3);
                SelectedColor = value.Substring(6, 2);
                onPropertyChanged("SelectedCode");
            }
        }

        private int selectedCount;
        
        public int SelectedCount {
            get { return selectedCount; }
            set { selectedCount = value; onPropertyChanged("SelectedCount"); }
        }

        private string selectedCreator;
        public string SelectedCreator {
            get {
                return selectedCreator;
            }
            set {
                selectedCreator = value;
                onPropertyChanged("SelectedCreator");
            }
        }

        private string selectedType = "";
        public string SelectedType
        {
            get
            {
                return selectedType;
            }
            set
            {
                selectedType = value;
                onPropertyChanged("SelectedType");
            }
        }

        private string selectedColor;
        public string SelectedColor
        {
            get
            {
                return selectedColor;
            }
            set
            {
                selectedColor = value;
                onPropertyChanged("SelectedColor");
            }
        }


        //Input values
        private string inputCreator;
        private string inputType;
        private string inputColor;
        private double inputWeight;
        private int inputCount;

        public string InputCreator { get { return inputCreator; } set { inputCreator = value; onPropertyChanged("inputCreator"); } }
        public string InputType { get { return inputType; } set { inputType = value; onPropertyChanged("inputType"); } }
        public string InputColor { get { return inputColor; } set { inputColor = value; onPropertyChanged("inputColor"); } }
        public double InputWeight { get { return inputWeight; } set { inputWeight = value; onPropertyChanged("inputWeight"); } }
        public int InputCount{ get { return inputCount; } set { inputCount = value; onPropertyChanged("inputCount"); } }



        public event PropertyChangedEventHandler PropertyChanged;

        protected void onPropertyChanged(string name) {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        public void addFilament() {
            Task.Run(async () => {
                var addRequest = await repository.AddFilaments(new
                {
                    creator = InputCreator,
                    type = InputType,
                    color = InputColor,
                    weight = InputWeight,
                    count = InputCount
                }) ;
                if (addRequest == null)
                {
                    MessageBox.Show("Error to add request");
                }
                else {
                    requestFilaments();
                }
            }).Wait();
        }

        private bool typeCheck(string type) {
            if (InputType.Equals("PLA+")) return type.Equals("PLA+") || type.Equals("PLP");
            else if (InputType.Equals("FLIXABLE")) return type.Equals("PLIXABLE") || type.Equals("TPU");
            return type.Equals(InputType);
        }
        public void removeFilament() {
            Task.Run(async () => {

                var item = Filament.Where(i => (i.Creator.Equals(InputCreator) && i.Color.Equals(InputColor) && typeCheck(i.Type) && i.Weight == InputWeight)).FirstOrDefault();
                if (item == null) {
                    MessageBox.Show("Error to add request");
                    return;
                }
                item.Count = InputCount;

                var addRequest = await repository.DeleteFilaments(item) ;
                if (addRequest == null)
                {
                    MessageBox.Show("Error to add request");
                }
                else
                {
                    requestFilaments();
                }
            }).Wait();
        }

        public void requestFilaments() {
            Task.Run(async () =>
            {
                var result = await repository.GetFilaments(selectedCreator, selectedType, selectedColor);
                if (result == null) result = new List<FilamentModel>();
                result.Add(new FilamentModel("","", "", "", 0, "", 0));
                Filament = new ObservableCollection<FilamentModel>(result);

            }).Wait();
        }

        public FilamentViewModel()
        {
            Filament = new ObservableCollection<FilamentModel>();

            //BindingOperations.EnableCollectionSynchronization(_filament, _lock);

            requestFilaments();
        }
    }
}
