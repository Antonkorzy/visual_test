using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataBaseStore;

namespace Лабораторная_14
{
    class Program
    {
        static public uint uniqueNum = 1;
        static Random rnd = new Random();
        static Func<Document, Document> Select_all = delegate (Document doc) { return doc; };
        static Func<Document, bool> average = delegate (Document doc) { return (doc is Check) && (((Check)doc).Seller == "Semya"); };
        static Func<Document, bool> count_delegate = delegate (Document doc) { return ((doc is Nakladnaya) && ((Nakladnaya)doc).ProductName == "Nutella"); };

        static void Main(string[] args)
        {
            Console.WriteLine("HELLO!");
            //заполнение документов
            Queue<List<Document>> documents = new Queue<List<Document>>();
            int count = rnd.Next(3, 10);
            for (int i = 0; i < count; i++)
            {
                int insideCount = rnd.Next(2, 8);
                documents.Enqueue(FillFolder(insideCount));
            }
            //запрос на выборку
           List<string> selectItems = SelectProductInStore(documents);
           Console.WriteLine("LINQ запрос: Вывести все товары в магазине Семья:");
           foreach(string item in selectItems)
            {
                Console.WriteLine(item);
            }
            Console.WriteLine();
            Console.WriteLine("LINQ запрос: Количество товара Nutella магазинах: " + Counter_product(documents));
            Console.WriteLine("Расширение с анонимным делегатом: Количество товара Nutella магазинах: " + Counter_product_expand(documents));
            Console.WriteLine("LINQ запрос: Количество накладных на сумму превышающую заданную: " + Counter_check(documents));
            Console.WriteLine("LINQ запрос: Суммарная стоимость товара заданного наименования: " + product_sum(documents));
            Console.WriteLine("Расширение с анонимным делегатом: Минимальная стоимость накладной: " + min_naklad_expand(documents));

        }
        static List<Document> FillFolder(int count)
        {
            List<Document> folder = new List<Document>();
            for (int i = 0; i < count; i++)
            {
                switch (rnd.Next(1, 3))
                {
                    case 1:
                        Nakladnaya doc = new Nakladnaya();
                        doc.RandomInit(Program.uniqueNum++);
                        folder.Add(doc);
                        break;
                    case 2:
                        Check doc1 = new Check();
                        doc1.RandomInit(Program.uniqueNum++);
                        folder.Add(doc1);
                        break;
                    case 3:
                        Kvitancia doc2 = new Kvitancia();
                        doc2.RandomInit(Program.uniqueNum++);
                        folder.Add(doc2);
                        break;
                }
            }
            return folder;
        }
        //Вывести все товары в магазине Семья

        static List<string> SelectProductInStore(Queue<List<Document>> documents)
        {
            string sellerName = "Semya";
            List<string> finalStr = new List<string>();
            List<Document> fold = null;
            for (uint i = 0; i < documents.Count; i++)
            {
                fold = documents.Peek();

                var subset = from seller in documents.First() where ((seller is Check) && ((Check)seller).Seller == sellerName) select seller;
                documents.Enqueue(documents.Dequeue());
                for (int j=1; i<fold.Count;i++)
                {
                    subset =  subset.Union(from seller in documents.Peek() where ((seller is Check) && ((Check)seller).Seller == sellerName) select seller);
                    documents.Enqueue(documents.Dequeue());
                }

                foreach (var seller_product in subset)
                {
                    finalStr.Add("В магазине: " + ((Check)seller_product).Seller + " Есть товар: " + ((Check)seller_product).ProductName);
                }
            }
            return finalStr;
            }
       // Количество товара заданного наименования.
        static int Counter_product(Queue<List<Document>> documents)
        {
            int numb = 0;
            string product_mask = "Nutella";
            for (uint i = 0; i < documents.Count; i++)
            {
                numb += (from product in documents.Peek() where ((product is Nakladnaya) && ((Nakladnaya)product).ProductName == product_mask) select product).Count<Document>();
                documents.Enqueue(documents.Dequeue());
            }
            return numb;
        }
        static int Counter_product_expand(Queue<List<Document>> documents)
        {
            int numb = 0;
            for (uint i = 0; i < documents.Count; i++)
            {
                numb += documents.Peek().Select(Select_all).Where(count_delegate).Count<Document>();
                    documents.Enqueue(documents.Dequeue());
            }
            return numb;
        }
        //35.	Количество накладных на сумму превышающую заданную.
        static int Counter_check(Queue<List<Document>> documents)
        {
            int numb = 0;
            int ideal_sum = 30000;
            for (uint i = 0; i < documents.Count; i++)
            {
                numb += documents.Peek().Where(product => ((product is Nakladnaya) && ((Nakladnaya)product).Price > ideal_sum)).Select(product => product).Count<Document>();
                documents.Enqueue(documents.Dequeue());
            }
            return numb;
        }

        //36.	Суммарная стоимость товара заданного наименования.
        static long product_sum(Queue<List<Document>> documents)
        {
            string product_mask = "Nutella";
            long sum = 0;
            for (uint i = 0; i < documents.Count; i++)
            {
                sum += documents.Peek().Where(product => ((product is Nakladnaya) && (((Nakladnaya)product).ProductName == product_mask))).Sum<Document>(product => ((Nakladnaya)product).Price);
                documents.Enqueue(documents.Dequeue());
            }


            return sum;
        }
        //49.	Минимальная стоимость накладной.
        static long min_naklad_expand(Queue<List<Document>> documents)
        {
            long min = 0;
            for (uint i = 0; i < documents.Count; i++)
            {
                min += documents.Peek().Select(Select_all).Min(doc => ((Nakladnaya)doc).Price);
                documents.Enqueue(documents.Dequeue());
            }
            return min;
        }
    }
}
