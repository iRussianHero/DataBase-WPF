using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace WpfApp40
{
    class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        void PropertyChanging(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        ShopContext myShop;
        bool rb1;
        bool rb2;
        bool rb3;
        public static AddPosition addPosition;
        public static UpdatePosition updatePosition;
        List<Product> products;
        List<Product> clientProducts;
        public static Product selectedItem;
        string findText;

        public ViewModel()
        {
            myShop = new ShopContext();
            products = new List<Product>();
            clientProducts = new List<Product>();
            RefreshData();
        }
        public List<Product> Products
        {
            get { return products; }
            set
            {
                products = new List<Product>(products);
                PropertyChanging("Products");
            }
        }
        public List<Product> ClientProducts
        {
            get { return clientProducts; }
            set
            {
                clientProducts = value;
                PropertyChanging("ClientProducts");
            }
        }
        public Product SelectedItem
        {
            get { return selectedItem; }
            set
            {
                selectedItem = value;
                PropertyChanging("SelectedItem");
            }
        }

        public bool Rb1
        {
            get { return rb1; }
            set
            {
                rb1 = value;
                PropertyChanging("Rb1");
                RefreshData(findText);

            }
        }
        public bool Rb2
        {
            get { return rb2; }
            set
            {
                rb2 = value;
                PropertyChanging("Rb2");
                RefreshData(findText);
            }
        }
        public bool Rb3
        {
            get { return rb3; }
            set
            {
                rb3 = value;
                PropertyChanging("Rb3");
                RefreshData(findText);
            }
        }
        public string FindText
        {
            get { return findText; }
            set
            {
                findText = value;
                RefreshData(findText);
                PropertyChanging("FindText");
            }
        }

        public ICommand AddButton
        {
            get
            {
                return new ButtonsCommand(
              () =>
              {
                  addPosition = new AddPosition();
                  addPosition.ShowDialog();
                  RefreshData();
                    //ShopContext shopContext = new ShopContext();
                    //shopContext.Products.Add(new Product() 
                    //{ Name = "Свекла", Price = 50, Category = "Овощь" });
                    //shopContext.SaveChanges();
                    //MessageBox.Show("Выполнено");
                }
              );
            }
        }
        public ICommand UpdateButton
        {
            get
            {
                return new ButtonsCommand(
              () =>
              {
                  if (selectedItem != null)
                  {
                      updatePosition = new UpdatePosition();
                      updatePosition.ShowDialog();
                      RefreshData();
                  }
                  //ShopContext shopContext = new ShopContext();
                  //shopContext.Products.Add(new Product() 
                  //{ Name = "Свекла", Price = 50, Category = "Овощь" });
                  //shopContext.SaveChanges();
                  //MessageBox.Show("Выполнено");
              }
              );
            }
        }
        public ICommand DeleteButton
        {
            get
            {
                return new ButtonsCommand(
              () =>
              {
                  if (selectedItem != null)
                  {
                      MessageBoxResult result =
                       MessageBox.Show("Действительно удалить продукт "
                           + selectedItem.Name + "?", "Удалить?", MessageBoxButton.YesNo,
                           MessageBoxImage.Question);

                      if (result == MessageBoxResult.Yes)
                      {
                          myShop.Products.Remove(selectedItem);
                          myShop.SaveChanges();
                          RefreshData();
                      }
                  }
              });
            }
        }
        public ICommand BuyProduct
        {
            get
            {
                return new ButtonsCommand(
              () =>
              {
                  if (selectedItem != null)
                  {
                      clientProducts.Add(selectedItem);
                      List<Product> clone = new List<Product>(clientProducts.Count);
                      clientProducts.ForEach((index) => { clone.Add(index); });
                      ClientProducts = clone;
                  }
              }
              );
            }
        }
        public ICommand Order
        {
            get
            {
                return new ButtonsCommand(
                    () =>
                    {
                        if(clientProducts.Count==0) return;
                        SortedDictionary<int, double> dictionary = new SortedDictionary<int, double>();

                        for (int i = 0; i < ClientProducts.Count(); ++i)
                        {
                            if (!dictionary.ContainsKey(ClientProducts[i].Id))
                            {
                                dictionary.Add(ClientProducts[i].Id, ClientProducts[i].Price);
                            }
                            else
                            {
                                dictionary[ClientProducts[i].Id] += ClientProducts[i].Price;
                            }
                        }

                        string orderString = string.Empty;
                        double totalPrice = 0;

                        foreach (var diction in dictionary)
                        {
                            var clientPoductOne = ClientProducts.Where(prod => prod.Id == diction.Key).Take(1);
                            var count = ClientProducts.Where(prod => prod.Id == diction.Key).Count();
                            foreach (var product in clientPoductOne)
                            {
                                orderString += $"Продукт: {product.Name}, цена: {product.Price}, ко-во: {count}, итого:{count*product.Price}\n";
                                totalPrice += count*product.Price;
                            }
                        }

                        orderString += $"\n\nОбщая стоимость: {totalPrice}";
                        MessageBox.Show($"{orderString}");
                    }
                    );
            }
        }
        public ICommand Cancel
        {
            get
            {
                return new ButtonsCommand(
                    () =>
                    {
                        if (ClientProducts.Count != 0)
                        {
                            ClientProducts = new List<Product>();
                        }
                    }
                    );
            }
        }
        void RefreshData(string text = "")
        {
            myShop = new ShopContext();
            if (rb1)
            {
                products = (from prod in myShop.Products
                            where prod.Name.Contains(text)
                            orderby prod.Name
                            select prod).ToList();
            }
            else if (rb2)
            {
                products = (from prod in myShop.Products
                            where prod.Name.Contains(text)
                            orderby prod.Price
                            select prod).ToList();
            }
            else
            {
                products = (from prod in myShop.Products
                            where prod.Name.Contains(text)
                            orderby prod.Category
                            select prod).ToList();
            }
            Products = products;
        }
    }
}
