using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kek
{
    // виртуальное пространство = VA
    // физическое  пространство = PA

    //виртуальная память - просто записи адресов, которые будут транслированы в реальную(физическую(ОЗУ))

    //arch - разрядность системы(16,32,64) (регистра - максимально возможная виртуальная адресуемая память(до трансляции в физическую) в пределах одной операции регистра 2^16, 2^32, ...)
    // считаем соответственно, что arch = 16 для виртуальной памяти в 8 лабе или 2^16 байт = 65536
    //----
    //
    // _power_of_page - степень(i) в которую возведется 2, чтобы вычислить размер страницы (будем считать 0<i<16, т.е. i={1,2,..,14,15})
    // можно было бы сделать, чтобы были доступны 2^0 = 1 или 2^16 = 65536(64кб)
    // в первом случае всего одна страница на полный объем данных(1 стр - 64кб), во втором страница содержит всего 1 байт(65536 страниц по 1 байту)
    // не вижу смысла в реализации крайних значений, поэтому кол-во страниц будет в 8 лабе от 2 до 32768(2, 4, 8, ..., 16384, 32768)
    //----
    //
    // соответсвенно, если разрядность страницы равна, к примеру, i=6 (2^6 = 64 байта данных) , то количество страниц 2^(arch-i) или 2^(16-6) = 2^10 = 1024 страницы
    // таким образом, если разрядность страницы равна i, то кол-во страниц = 2^(arch-i) 

    

    public class MainConfigurator
    {
        private UInt64 _VA = 0;
        private UInt64 _arch = 0;

        private UInt64 _power_VA_n_of_pages = 0;
        private UInt64 _VA_n_of_pages = 0;

        private UInt64 _power_capacity_per_page = 0;
        private UInt64 _capacity_per_page = 0;

        private UInt64 _RAM = 0;
        private UInt64 _RAM_pages = 0;

        public MainConfigurator(UInt64 page_capacity, UInt64 RAM)
        {
            
            //для 8 лабы 16 бит архитектура
           
            this._arch = 16;
            this._VA = Util._Power(2,16);

            if( !(page_capacity > 1 && page_capacity < this._VA/2)) { throw new Exception("page capacity out of bounds for our arch(16-bit default)"); }

            this._power_capacity_per_page = Util._Get_Closest_Power(page_capacity);
            this._capacity_per_page = Util._Power(2, _power_capacity_per_page);

            this._power_VA_n_of_pages = this._arch - this._power_capacity_per_page;
            this._VA_n_of_pages = Util._Power(2, _power_VA_n_of_pages);


            if (RAM < 2* _capacity_per_page) { throw new Exception("RAM capacity is not sufficient for even 2 pages"); }
            if (RAM > _VA) { this._RAM = _VA; }
            else
            {
                this._RAM = Util._Power(2, Util._Get_Closest_Power(RAM));
            }

            this._RAM_pages = this._RAM/this._capacity_per_page;
           

        }

        public void Show()
        {
            Console.WriteLine($"Arch is: {this._arch}");
            Console.WriteLine($"Page size: {this._capacity_per_page}");
            Console.Write($"VA size: {this._VA}"); Console.WriteLine($" number of VA pages: {this._VA_n_of_pages}");
            Console.Write($"PA size: {this._RAM}"); Console.WriteLine($" number of PA pages: {this._RAM_pages}");
        }

        public UInt64 Get_Page_Size()
        {
            return this._capacity_per_page;
        }
        public UInt64 Get_Number_of_VA_Pages()
        {
            return this._VA_n_of_pages;
        }

        public UInt64 Get_Number_of_RAM_Pages()
        {
            return this._RAM_pages;
        }

        //получить степень 2 снизу для числа


    }
}
