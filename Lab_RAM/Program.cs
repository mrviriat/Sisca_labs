using System.IO;

namespace kek
{

class Program
    {
        public static void Main(string[] args)
        {
            /* Console.WriteLine("pages from RAM and Vritual spaces are of the same sizes");
             Console.WriteLine("realised model is: 1 bit of presence in MMU mapper");
             Console.WriteLine("so RAM is BY MODEL is mapped to size x0.5 of virtual space");
             Console.WriteLine("so if RAM is more than 0.5 size of Virtual Space it is ignored(used only 0.5)");
             Console.WriteLine("64kb of VA => 32kb of PA");
             Console.WriteLine("if the size of RAM is less than 2 pages of Virtual memory - I assume that algo has incorrect input data");
             Console.WriteLine("Any Ram size would be resized to be exactly proportional to the size of page");
             Console.WriteLine("-------------\n");*/
            //Console.ForegroundColor = ConsoleColor.r


            var mainconfig = new MainConfigurator(4100, 17000);
            mainconfig.Show();

            var VA_space = new VA(mainconfig.Get_Page_Size(), mainconfig.Get_Number_of_VA_Pages());

            var mmu = new MMU(mainconfig.Get_Number_of_RAM_Pages(), mainconfig.Get_Number_of_VA_Pages(), mainconfig.Get_Page_Size(), VA_space.Get_VA_Array());
            mmu.addAddr(20000);

            mmu.show_PAGE_TABLE(true);
            mmu.createColorRelation();

            var ram = new RAM(mainconfig.Get_Page_Size(), mainconfig.Get_Number_of_RAM_Pages(), mmu.get_Page_table(), VA_space.Get_VA_Array());
            mmu.setRAMAddr(ram.getRAMAddr());

            mmu.show_VA_FULL();
            mmu.show_RAM();

        }
    }
}