namespace Model
{
    internal class ReaderCategory
    {
        public int ID { get; set; }
        public string Denomination { get; set; }
        public decimal Discount { get; set; }

        public ReaderCategory(int iD, string denomination, decimal discount)
        {
            ID = iD;
            Denomination = denomination;
            Discount = discount;
        }
    }

}
