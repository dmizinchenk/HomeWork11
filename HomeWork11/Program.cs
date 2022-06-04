using System;
using System.Collections.Generic;
using System.Xml;

namespace HomeWork11
{
    public interface IProduct
    {
        string Name { get; }
    }

    public abstract class Food : IProduct
    {
        public string Name { get; }

        public Food(string name)
        {
            Name = name;
        }
    }
    public abstract class Chemical : IProduct
    {
        public string Name { get; }

        public Chemical(string name)
        {
            Name = name;
        }
    }

    public class Powder : Chemical
    {
        public Powder(string name) : base(name) { }
    }
    public class Soap : Chemical
    {
        public Soap(string name) : base(name) { }
    }
    public class Meat : Food
    {
        public Meat(string name) : base(name) { }
    }
    public class Fruit : Food
    {
        public Fruit(string name) : base(name) { }
    }

    public class Orders
    {
        List<(IProduct, int)> bascket;

        public Orders()
        {
            bascket = new List<(IProduct, int)>();
        }
        public void AddProduct(IProduct p, int count = 1)
        {
            if (p == null || count < 1)
                return;
            int element = bascket.FindIndex(e => e.Item1.Name == p.Name);
            if (element < 0)
                bascket.Add((p, count));
            else
                bascket[element] = (p, bascket[element].Item2 + count);
        }
        public IEnumerator<(IProduct, int)> GetEnumerator()
        {
            return bascket.GetEnumerator();
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            //Создание заказов
            IProduct duru = new Soap("Duru");
            IProduct biolan = new Powder("Biolan");
            IProduct sorti = new Powder("Sorti");
            IProduct chicken = new Meat("Chicken");
            IProduct pig = new Meat("Pig");
            IProduct apple = new Fruit("Apple");

            Orders order1 = new Orders();
            order1.AddProduct(pig);
            order1.AddProduct(apple, 10);
            order1.AddProduct(biolan);
            order1.AddProduct(biolan);

            Orders order2 = new Orders();
            order2.AddProduct(sorti, 3);
            order2.AddProduct(chicken, 2);
            order2.AddProduct(duru, 4);

            Orders order3 = new Orders();
            order3.AddProduct(apple);
            order3.AddProduct(apple);
            order3.AddProduct(apple);
            order3.AddProduct(duru, 2);
            order3.AddProduct(biolan);
            order3.AddProduct(chicken);

            //Запись данных в xml
            string document = "orders.xml";
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            using (XmlWriter writer = XmlWriter.Create(document, settings))
            {
                writer.WriteStartElement("orders");

                //первый заказ
                writer.WriteStartElement("order");
                string attr = "";
                foreach ((IProduct, int) product in order1)
                {
                    writer.WriteStartElement("product");
                    if (product.Item1 is Soap)
                        attr = "soap";
                    else if (product.Item1 is Powder)
                        attr = "powder";
                    else if (product.Item1 is Meat)
                        attr = "meat";
                    else if (product.Item1 is Fruit)
                        attr = "fruit";
                    writer.WriteAttributeString("type", attr);

                    writer.WriteElementString("name", product.Item1.Name);
                    writer.WriteElementString("count", product.Item2.ToString());
                    writer.WriteEndElement(); //для product
                }
                writer.WriteEndElement(); //для order

                //второй заказ
                writer.WriteStartElement("order");
                foreach ((IProduct, int) product in order2)
                {
                    writer.WriteStartElement("product");
                    if (product.Item1 is Soap)
                        attr = "soap";
                    else if (product.Item1 is Powder)
                        attr = "powder";
                    else if (product.Item1 is Meat)
                        attr = "meat";
                    else if (product.Item1 is Fruit)
                        attr = "fruit";
                    writer.WriteAttributeString("type", attr);

                    writer.WriteElementString("name", product.Item1.Name);
                    writer.WriteElementString("count", product.Item2.ToString());
                    writer.WriteEndElement(); //для product
                }
                writer.WriteEndElement(); //для order

                //третий заказ
                writer.WriteStartElement("order");
                foreach ((IProduct, int) product in order3)
                {
                    writer.WriteStartElement("product");
                    if (product.Item1 is Soap)
                        attr = "soap";
                    else if (product.Item1 is Powder)
                        attr = "powder";
                    else if (product.Item1 is Meat)
                        attr = "meat";
                    else if (product.Item1 is Fruit)
                        attr = "fruit";
                    writer.WriteAttributeString("type", attr);

                    writer.WriteElementString("name", product.Item1.Name);
                    writer.WriteElementString("count", product.Item2.ToString());
                    writer.WriteEndElement(); //для product
                }
                writer.WriteEndElement(); //для order

                writer.WriteEndElement(); //для orders (корневой)
            }


            //Чтение данных
            List<Orders> list = new List<Orders>();
            using (XmlReader reader = XmlReader.Create(document))
            {
                reader.Read();

                reader.ReadStartElement(); //чтение корня
                while (reader.IsStartElement())
                {
                    Orders order = new Orders();

                    reader.ReadStartElement(); //начинаем чтение заказа
                    while (reader.IsStartElement())
                    {
                        string name;
                        int count;
                        IProduct p = null;

                        string attr = reader.GetAttribute("type");
                        reader.ReadStartElement();//начинаем читать продукт
                        reader.ReadStartElement(); //название
                        name = reader.ReadContentAsString();
                        reader.ReadEndElement(); //название
                        reader.ReadStartElement(); //кол-во
                        count = Convert.ToInt32(reader.ReadContentAsString());
                        switch(attr)
                        {
                            case "powder":
                                p = new Powder(name); break;
                            case "soap":
                                p = new Soap(name); break;
                            case "meat":
                                p = new Meat(name); break;
                            case "fruit":
                                p = new Fruit(name); break;
                            default: break;
                        }
                        order.AddProduct(p, count);
                        reader.ReadEndElement(); //кол-во
                        reader.ReadEndElement(); //заканчиваем чтение продукта
                    }
                    list.Add(order);
                    reader.ReadEndElement(); //заканчиваем чтение заказа
                }
                reader.ReadEndElement(); //заканчиваем чтение корня

            }

            int i = 1;
            foreach (Orders order in list)
            {
                Console.WriteLine($"Состав {i++}-го заказа: ");
                foreach ((IProduct, int) item in order)
                {
                    Console.WriteLine($"{item.Item1.Name} - {item.Item2} шт.");
                }
                Console.WriteLine();
            }
        }
    }
}
