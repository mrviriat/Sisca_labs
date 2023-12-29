using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kek
{
    public struct RAM_record
    {

        public static UInt64 page_size = 0;
        public static UInt64 n_of_pages = 0;

        //бит присутствия спускается из MMU
        UInt64 abs_addr = 0;
        UInt64 current_page = 0;
        UInt64 in_page_addr = 0;


        public RAM_record(UInt64 current_page, UInt64 in_page_addr)
        {
            this.current_page = current_page;
            this.in_page_addr=in_page_addr;
            this.abs_addr = current_page*page_size + in_page_addr;
            
        }

        public static void config_RAM_record(UInt64 page_size, UInt64 n_of_pages)
        {
            RAM_record.page_size = page_size;
            RAM_record.n_of_pages = n_of_pages;
        }
        public UInt64 get_Abs_RAM_Addr()
        {
            return this.abs_addr;
        }
        public UInt64 get_Current_RAM_Page()
        {
            return this.current_page;
        }
        public UInt64 get_Current_RAM_Page_Address()
        {
            return this.in_page_addr;
        }


    }
    public class RAM
    {

        UInt64 _page_size = 0;
        UInt64 _n_of_pages = 0;
        UInt64 _max_adress = 0;

        public List<MappedRecord> _m_recs;
        public List<RAM_record> RAM_addr_arr = new List<RAM_record>();
        List<VA_record> _v_recs;

        public RAM(UInt64 page_size, UInt64 max_pages, List<MappedRecord> _m_recs, List<VA_record> _v_recs)
        {
            this._page_size = page_size;
            this._n_of_pages = max_pages;
            _max_adress = max_pages * page_size - 1;

            RAM_record.config_RAM_record(this._page_size, this._n_of_pages);
            this._m_recs = _m_recs;
            this._v_recs = _v_recs;

            conf();
        }


        private void conf()
        {
            for(UInt64 i = 0; i < (UInt64)_v_recs.Count; i++)
            {
                bool isPresentInMMU = false;
                for(UInt64 j = 0; j<(UInt64)_m_recs.Count; j++)
                {
                    if(_m_recs.ElementAt((int)j).bit_of_presence == 1 && j == _v_recs.ElementAt((int)i).get_Current_VA_Page())
                    {
                        var _t = new RAM_record( _m_recs.ElementAt((int)j).RAM_ADDR, _v_recs.ElementAt((int)i).get_Current_VA_Page_Address());
                        RAM_addr_arr.Add(_t);
                    }
                }
            }
        }

        public void qaz()
        {
            foreach(var _p in RAM_addr_arr)
            {
                Console.WriteLine($"{_p.get_Current_RAM_Page()}  -  {_p.get_Current_RAM_Page_Address()}");
            }
        }

        public List<RAM_record> getRAMAddr()
        {
            return this.RAM_addr_arr;
        }

        /*public void show_RAM()
        {
            Console.WriteLine("\nRAM:\n");

                bool isCurrentEmpty = true;
                Console.WriteLine("\nRAM:\n");
                for (UInt64 ii = 0; ii < this._n_of_pages; ii++)
                {
                    foreach (var el in RAM_addr_arr)
                    {
                        if (el.get_Current_RAM_Page() == ii)
                        {
                            Console.WriteLine($"RAM_page={ii} AA = {el.get_Abs_RAM_Addr(),6}  In-Page addr = {el.get_Current_RAM_Page_Address(),5}");
                            isCurrentEmpty = false;
                        }

                    }
                    if (isCurrentEmpty)
                    {
                        Console.WriteLine($"RAM_page={ii} AA = {"-",6}  In-Page addr = {"-",5}");
                    }
                    isCurrentEmpty = true;
                }
            
        }*/

        //заполняет линейно память в соответствии с VA в обход MMU
        //потом конфиг MMU в соответствии с заполненным пр-ством
        //чтение уже происходит через MMU
        /*private void LinearFill()
        {

        }

        public bool IsPagePresentInRAM()
        {

        }
        public bool IsAddressNotPresent()
        {

        }

        public bool RAM_Alloc(RAM_record _q)
        {

            counter_of_allocated_pages
        }*/






    }
}
