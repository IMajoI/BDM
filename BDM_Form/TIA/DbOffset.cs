namespace TIAIFCNS
{
    public class DbOffset
    {
        private int Address = 0;
        private int nextAddress = 0;

        public double Value
        {
            get
            {
                if (Address % 16 != 0)
                    Address = nextAddress + (16 - Address % 16);

                return Address / 8.0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public DbOffset()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        private void FinalCalc()
        {
            //Debug.WriteLine(Address / 8 + "." + Address % 8 + " >>> " + nextAddress/8 + "." + nextAddress%8);
            Address = nextAddress;
            //Debug.WriteLine(Address/8 + "." +Address%8);
        }

        /// <summary>
        /// 
        /// </summary>
        public void AddBit()
        {
            Address = nextAddress;
            nextAddress = Address + 1;
            FinalCalc();
        }

        /// <summary>
        /// 
        /// </summary>
        public void AddByte()
        {
            if (Address % 8 != 0)
                Address = nextAddress + (8 - Address % 8);

            nextAddress = Address + 8;
            FinalCalc();
        }

        /// <summary>
        /// 
        /// </summary>
        public void AddWord()
        {
            if (Address % 16 != 0)
                Address = nextAddress + (16 - Address % 16);

            nextAddress = Address + 16;
            FinalCalc();
        }


        public void AddDWord()
        {
            if (Address % 16 != 0)
                Address = nextAddress + (16 - Address % 16);

            nextAddress = Address + 32;
            FinalCalc();
        }
    }
}
