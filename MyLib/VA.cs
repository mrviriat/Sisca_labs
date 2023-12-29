using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kek
{
    public struct VA_record {

        public static UInt64 page_size = 0;
        public static UInt64 n_of_pages = 0;


        UInt64 abs_addr=0;
        UInt64 current_page=0;
        UInt64 in_page_addr=0;

        
        public VA_record(UInt64 abs_addr)
        {
            this.abs_addr = abs_addr;
            this.current_page = abs_addr / VA_record.page_size;
            this.in_page_addr = this.current_page != 0 ? this.abs_addr % (this.current_page * VA_record.page_size) : this.abs_addr;
        }

        public static void config_VA_record(UInt64 page_size, UInt64 n_of_pages)
        {
            VA_record.page_size = page_size;
            VA_record.n_of_pages = n_of_pages;
        }
        public UInt64 get_Abs_Addr()
        {
            return this.abs_addr;
        }
        public UInt64 get_Current_VA_Page()
        {
            return this.current_page;
        }
        public UInt64 get_Current_VA_Page_Address()
        {
            return this.in_page_addr;
        }


    }

    public class VA
    {
        UInt64 _page_size = 0;
        UInt64 _n_of_pages = 0;
        UInt64 _max_adress = 0;

        Random rnd = new Random();

        public List<VA_record> VA_addr_arr = new List<VA_record>();

        public VA(UInt64 page_size, UInt64 max_pages) { 
            this._page_size = page_size;
            this._n_of_pages=max_pages;
            _max_adress = max_pages * page_size - 1;

            VA_record.config_VA_record(this._page_size, this._n_of_pages);
            _Generate_VA_Adresses();
        }

        private void _Generate_VA_Adresses()
        {
            UInt64 k = 0;
            for(int i = 0; i < 100; i++)
            {                
                k += (UInt64)(rnd.Next() %300 + rnd.Next() % 200 + rnd.Next() % 300);

                if(k >= this._max_adress - 1)
                {
                    break;
                }
                var _temp = new VA_record(k);

                VA_addr_arr.Add(_temp);
            }
        }

        public void show_VA_Records()
        {
            Console.WriteLine("\nVA slice records:\n");
            foreach(var el in VA_addr_arr)
            {
                Console.WriteLine($"AA = {el.get_Abs_Addr(),6} In-Page addr = {el.get_Current_VA_Page_Address(),5}");
            }
            Console.WriteLine();
        }
        /*public void show_VA_FULL()
        {
            bool isCurrentEmpty = true;
            Console.WriteLine("\nVA:\n");
            for (UInt64 i = 0; i< this._n_of_pages; i++)
            {
                foreach(var el in VA_addr_arr)
                {
                    if(el.get_Current_VA_Page() == i)
                    {
                        Console.WriteLine($"VA_page={i} AA = {el.get_Abs_Addr(),6}  In-Page addr = {el.get_Current_VA_Page_Address(),5}");
                        isCurrentEmpty = false;
                    }                   

                }
                if (isCurrentEmpty)
                {
                    Console.WriteLine($"VA_page={i} AA = {"-",6}  In-Page addr = {"-",5}");
                }
                isCurrentEmpty = true;
            }
        }*/

        public List<VA_record> Get_VA_Array()
        {
            return this.VA_addr_arr;
        }

        public UInt64 getNofPages()
        {
            return this._n_of_pages;
        }

        
    }

}
