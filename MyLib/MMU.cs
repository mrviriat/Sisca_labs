using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kek
{

    public struct ColorOut
    {
        public UInt64 VA_addr;
        public MappedRecord mr;
        public UInt16 colorInd; 

        public ColorOut(UInt64 VA_addr, MappedRecord mr, UInt16 colorInd)
        {
            this.VA_addr = VA_addr;
            this.mr = mr;
            this.colorInd = colorInd;   
        }
    }

    public struct MappedRecord
    {
        public static UInt64 current_RAM_Page = 0;

        public UInt64 RAM_ADDR;
        public UInt16 bit_of_presence;
        public MappedRecord(UInt64 RAM_ADDR, UInt16 bit_of_presence)
        {
            this.RAM_ADDR = RAM_ADDR;
            this.bit_of_presence = bit_of_presence;
        }
        public string toString()
        {
            return $" ram_page_map = {RAM_ADDR} isPresent = {bit_of_presence}";
        }
    }

    public class MMU
    {
        public static bool SHOW_PAGE_TABLE_WO_EMPTY = true;
        //public static bool SHOW_PAGE_TABLE_FULL = false;

        UInt64 size_page = 0;
        UInt64 n_RAM_page = 0;
        UInt64 n_VA_page = 0;

        UInt64 power_RAM_page = 0;
        UInt64 power_VA_page = 0;


        List<VA_record> VA_Records;
        List<RAM_record> RAM_Records;

        List<ColorOut> _l_colors = new List<ColorOut>();
        Dictionary<UInt64, MappedRecord> MMU_dict = new Dictionary<UInt64, MappedRecord>(); 
        List<MappedRecord> PAGE_TABLE = new List<MappedRecord>();

        UInt64 dict_size = 0;
        //UInt64 max_dict_size = 0;
        UInt64 dict_iterator = 0;

        public MMU(UInt64 n_RAM_page, UInt64 n_VA_page, UInt64 size_page, List<VA_record> VA_Records) {        
            this.n_VA_page=n_VA_page;

            this.n_RAM_page=n_RAM_page;
            //this.max_dict_size = n_RAM_page;

            this.size_page = size_page;
            this.VA_Records = VA_Records;

            this.power_RAM_page = Util._Get_Closest_Power(n_RAM_page);
            this.power_VA_page = Util._Get_Closest_Power(n_VA_page);

            createLinearMapping();
            create_PAGE_TABLE();
        }

        public void createColorRelation()
        {
            UInt16 i = 0;
            foreach(var item in MMU_dict)
            {
                if(item.Value.bit_of_presence == 1)
                {
                    i++;
                    var temp = new ColorOut(item.Key, item.Value, i);
                    _l_colors.Add(temp);
                }
            }
        }

        public Boolean addAddr(UInt64 _a)
        {
            if (_a > (this.size_page * this.n_VA_page - 1)) {
                Console.WriteLine("out of VA bounds");
                return false;
            }
            foreach(VA_record v in VA_Records)
            {
                if(v.get_Abs_Addr() == _a)
                {
                    Console.WriteLine("addr already exists");
                    return false;
                }
            }

            VA_Records.Add(new VA_record(_a));
            return true;
        }

        private void createLinearMapping()
        {

            for (int i = 0; i < this.VA_Records.Count; i++ ) {
                UInt64 _VA_rec_page = this.VA_Records.ElementAt(i).get_Current_VA_Page();
                

                if (MMU_dict.ContainsKey(_VA_rec_page))
                {
                    continue;
                }

                //итератор в конце равен n_RAM_page 
                // 0 1 2 3 - iterator = 4
                if(!(MMU_dict.ContainsKey(_VA_rec_page)) && (dict_size < n_RAM_page))
                {
                    var temp = new MappedRecord(MappedRecord.current_RAM_Page, 1);

                    MMU_dict.Add(dict_iterator, temp);
                    dict_size++;
                    dict_iterator++;
                    MappedRecord.current_RAM_Page++;
                    continue;
                }

                //не содержит и полный словарь
                if (!(MMU_dict.ContainsKey(_VA_rec_page)) && (dict_size == n_RAM_page))
                {
                    
                    //по факту тут идет swapping из-за недостатка страниц в RAM
                    //if(dict_iterator == dict_size)

                    if(MappedRecord.current_RAM_Page == n_RAM_page)
                    {
                        MappedRecord.current_RAM_Page = 0;
                    }

                    for(int k = 0; k < MMU_dict.Count; k++)
                    {
                        if (MMU_dict[(UInt64)k].RAM_ADDR == MappedRecord.current_RAM_Page)
                        {
                            var erase = new MappedRecord(0, 0);
                            MMU_dict[(UInt64)k] = erase;
                        }
                    }

                    var temp = new MappedRecord(MappedRecord.current_RAM_Page, 1);
                    MMU_dict[dict_iterator] = temp;

                    MappedRecord.current_RAM_Page++;
                    dict_iterator++;

                }
            }
        }
        public void show_dict()
        {
            Console.WriteLine("Dictionary show");
            foreach(var item in MMU_dict)
            {
                Console.WriteLine($"VA_addr: {item.Key}, RAM_addr: {item.Value.RAM_ADDR}, B: {item.Value.bit_of_presence}");
            }
            Console.WriteLine();
        }
        private void create_PAGE_TABLE()
        {
            
            for(UInt64 i = 0; i < n_VA_page; i++)
            {
                if (MMU_dict.ContainsKey(i))
                {

                    PAGE_TABLE.Add(MMU_dict[i]);
                }
                else
                {
                    var temp = new MappedRecord(0,0);
                    PAGE_TABLE.Add(temp);
                }
                
            }
        }
        public void show_PAGE_TABLE(bool showWOEmpty)
        {

            StreamWriter streamWriter = new StreamWriter("table.txt");
            for (int o = 0; o < PAGE_TABLE.Count; o++){
                streamWriter.Write($"VA_Page {o}; ");
                streamWriter.WriteLine($"RAM_page {PAGE_TABLE.ElementAt(o).RAM_ADDR} bit {PAGE_TABLE.ElementAt(o).bit_of_presence}");
            }
            streamWriter.Close();   
            
            Console.WriteLine("SHOW PAGE TABLE:");
            for (UInt64 i= 0; i< (UInt64)PAGE_TABLE.Count; i++)
            {
                if (showWOEmpty)
                {
                    if (PAGE_TABLE[(int)i].bit_of_presence == 0)
                        continue;                  
                    
                }
                Console.WriteLine($"{i,3} - {PAGE_TABLE[(int)i].toString()}");

            }
        }
        public List<MappedRecord> get_Page_table()
        {
            return this.PAGE_TABLE;
        }

        public void show_VA_FULL()
        {

            bool isCurrentEmpty = true;
            Console.WriteLine("\nVA:\n");
            for (UInt64 i = 0; i < this.n_VA_page; i++)
            {
                foreach (var el in VA_Records)
                {
                    if (el.get_Current_VA_Page() == i)
                    {
                        var colorReal = isPresentVA_Page(el.get_Current_VA_Page(), _l_colors);
                        if(colorReal != -1)
                        {
                            //Console.BackgroundColor = (ConsoleColor)(colorReal % 15);
                            Console.ForegroundColor = (ConsoleColor)((colorReal) % 15);
                        }
                        Console.WriteLine($"VA_page={i} AA = {el.get_Abs_Addr(),6}  In-Page addr = {el.get_Current_VA_Page_Address(),5}");
                        Console.ResetColor();
                        isCurrentEmpty = false;
                    }

                }
                if (isCurrentEmpty)
                {
                    Console.WriteLine($"VA_page={i} AA = {"-",6}  In-Page addr = {"-",5}");
                }
                isCurrentEmpty = true;
            }
            Console.ResetColor();
        }

        public void setRAMAddr(List<RAM_record> _r_addr)
        {
            this.RAM_Records = _r_addr;
        }

        public void show_RAM()
        {
            Console.WriteLine("\nRAM:\n");

            bool isCurrentEmpty = true;
            Console.WriteLine("\nRAM:\n");
            for (UInt64 ii = 0; ii < this.n_RAM_page; ii++)
            {
                foreach (var el in this.RAM_Records)
                {
                    if (el.get_Current_RAM_Page() == ii)
                    {
                        var colorReal = isPresentRAM_Page(el.get_Current_RAM_Page(), _l_colors);
                        if (colorReal != -1)
                        {
                            //Console.BackgroundColor = (ConsoleColor)(colorReal % 15);
                            Console.ForegroundColor = (ConsoleColor)((colorReal) % 15);
                        }
                        Console.WriteLine($"RAM_page={ii} AA = {el.get_Abs_RAM_Addr(),6}  In-Page addr = {el.get_Current_RAM_Page_Address(),5}");
                        isCurrentEmpty = false;
                        Console.ResetColor();

                    }

                }
                if (isCurrentEmpty)
                {
                    Console.WriteLine($"RAM_page={ii} AA = {"-",6}  In-Page addr = {"-",5}");
                }
                isCurrentEmpty = true;
            }
            Console.ResetColor();

        }


        public static int isPresentVA_Page(UInt64 _va_a, List<ColorOut> _l)
        {
            foreach (var item in _l)
            {
                if (item.VA_addr == _va_a)
                {
                    return item.colorInd;
                }
            }
            return -1;
        }
        public static int isPresentRAM_Page(UInt64 _ram_a, List<ColorOut> _l)
        {
            foreach (var item in _l)
            {
                if (item.mr.RAM_ADDR == _ram_a)
                {
                    return item.colorInd;
                }
            }
            return -1;
        }
    }       
}
