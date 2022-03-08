using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace WpfApp40
{
    class AddPositionViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        void PropertyChanging(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        string name;
        int price;
        string category;
        List<string> categories;
        ShopContext context;
        
        
        public AddPositionViewModel()
        {
            if (ViewModel.selectedItem != null)
            {
                Name = ViewModel.selectedItem.Name;
                Price = ViewModel.selectedItem.Price;
                Category = ViewModel.selectedItem.Category;
            }
            context = new ShopContext();
            //Categories = (from prod in context.Products
            //              where true
            //              select prod.Category).ToList(); 
            Categories = new List<string>();
            foreach( Product cat in context.Products)
            {
                if (categories.Contains(cat.Category) == false)
                    Categories.Add(cat.Category);
            }
         
            Categories= categories;
        }

        public ICommand CloseButton
        {
            get { return new ButtonsCommand(() => 
            { if (ViewModel.addPosition!=null)
                ViewModel.addPosition.Close();
                if (ViewModel.updatePosition != null)
                    ViewModel.updatePosition.Close();
            }); }
        }
        public ICommand UpdateButton
        {
            get {
                return new ButtonsCommand(
              () =>
              {
                  Product update = context.Products.Find(ViewModel.selectedItem.Id);
                  update.Name = Name;
                  update.Price = price;
                  update.Category = category;
                  context.SaveChanges();
                  ViewModel.updatePosition.Close();
              });
            }
        }
        public ICommand AddButton
        {
            get {
                return new ButtonsCommand(
              () =>
              {
                  context.Products.Add(
                      new Product() { Price = price, Name = name, Category = category });
                  context.SaveChanges();
                  ViewModel.addPosition.Close();
              });
            }
        }
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                PropertyChanging("Name");
            }
        }
        public int Price
        {
            get { return price; }
            set
            {
                price = value;
                PropertyChanging("Price");
            }
        }
        public string Category
        {
            get { return category; }
            set
            {
                category = value;
                PropertyChanging("Category");
            }
        }
        public List<string> Categories
        {
            get { return categories; }
            set
            {
                categories = value;
                categories = new List<string>(categories);
                PropertyChanging("Categories");
            }
        }

    }
}
